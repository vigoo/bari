using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Model;
using QuickGraph;


namespace Bari.Plugins.VsCore.Build
{
    public class OptimizingBuildContextFactory: IBuildContextFactory
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (OptimizingBuildContextFactory));

        private readonly IBuildContextFactory originalFactory;
        private readonly IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory;

        public OptimizingBuildContextFactory(IBuildContextFactory originalFactory, IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory)
        {
            this.originalFactory = originalFactory;
            this.inSolutionReferenceBuilderFactory = inSolutionReferenceBuilderFactory;
        }

        public IBuildContext CreateBuildContext()
        {
            var buildContext = originalFactory.CreateBuildContext();
            buildContext.AddTransformation(CutRedundantSolutionBuilds);
            buildContext.AddTransformation(MergeSolutionBuilds);
            return buildContext;
        }

        private bool CutRedundantSolutionBuilds(ISet<EquatableEdge<IBuilder>> graph)
        {
            var slnBuilders = new HashSet<SlnBuilder>(graph.Select(edge => edge.Target).OfType<SlnBuilder>());

            if (slnBuilders.Any())
            {
                foreach (var slnBuilder in slnBuilders)
                {
                    var projectSet = new HashSet<Project>(slnBuilder.Projects);
                    var childProjectBuilders = new HashSet<ISlnProjectBuilder>(slnBuilder
                        .Prerequisites
                        .OfType<ISlnProjectBuilder>());

                    foreach (var projectBuilder in childProjectBuilders)
                    {
                        foreach (var dep in projectBuilder.Prerequisites.OfType<SuiteReferenceBuilder>().ToList())
                        {
                            if (dep.Reference.Type == ReferenceType.Build && projectSet.Contains(dep.ReferencedProject))
                            {
                                log.DebugFormat("Project {0}'s reference {1} can be replaced to in-solution-reference in {2}", projectBuilder.Project, dep, slnBuilder);

                                // All sln project builders belonging to `slnBuilder` must replace their reference to dep to a 
                                // new in solution reference builder (both in the builder and in the graph)
                                ReplaceWithInSolutionReference(graph, childProjectBuilders, dep);

                                // All edges from dep must be removed and a single new edge to `slnBuilder` added
                                RemoveEdgesWhereSourceIs(graph, dep);

                                var newEdge = new EquatableEdge<IBuilder>(dep, slnBuilder);
                                log.DebugFormat("-> adding edge {0}", newEdge);
                                graph.Add(newEdge);
                            }
                        }
                    }
                }
            }

            return true;
        }

        void RemoveEdgesWhereTargetIs(ISet<EquatableEdge<IBuilder>> graph, IBuilder target)
        {            
            var toRemove = graph.Where(edge => edge.Target == target).ToList();

            foreach (var edge in toRemove)
                log.DebugFormat("-> removing edge: {0}", edge);

            graph.ExceptWith(toRemove);
        }

        void RemoveEdgesWhereSourceIs(ISet<EquatableEdge<IBuilder>> graph, IBuilder source)
        {            
            var toRemove = graph.Where(edge => edge.Source == source).ToList();

            foreach (var edge in toRemove)
                log.DebugFormat("-> removing edge: {0}", edge);

            graph.ExceptWith(toRemove);
        }


        void ReplaceWithInSolutionReference(ISet<EquatableEdge<IBuilder>> graph, HashSet<ISlnProjectBuilder> childProjectBuilders, SuiteReferenceBuilder dep)
        {
            var inSolutionRef = inSolutionReferenceBuilderFactory.CreateInSolutionReferenceBuilder(dep.ReferencedProject);
            inSolutionRef.Reference = dep.Reference;

            foreach (var builder in childProjectBuilders)
            {
                builder.ReplaceReferenceBuilder(dep, inSolutionRef);

                var edgesToModify = graph.Where(edge => edge.Source == builder && edge.Target == dep).ToList();
                graph.ExceptWith(edgesToModify);

                foreach (var edge in edgesToModify)
                {
                    var newEdge = new EquatableEdge<IBuilder>(edge.Source, inSolutionRef);
                    log.DebugFormat("-> adding edge {0}", newEdge);
                    graph.Add(newEdge);
                }
            }
        }

        private class SolutionBuildPattern
        {
            private readonly MSBuildRunner msbuildRunner;
            private readonly SlnBuilder slnBuilder;
            private readonly ISet<ISlnProjectBuilder> projectBuilders = new HashSet<ISlnProjectBuilder>();

            public SolutionBuildPattern(MSBuildRunner msbuildRunner, SlnBuilder slnBuilder)
            {
                this.msbuildRunner = msbuildRunner;
                this.slnBuilder = slnBuilder;
            }

            public MSBuildRunner MsbuildRunner
            {
                get { return msbuildRunner; }
            }

            public SlnBuilder SlnBuilder
            {
                get { return slnBuilder; }
            }

            public ISet<ISlnProjectBuilder> ProjectBuilders
            {
                get { return projectBuilders; }
            }
        }

        private bool MergeSolutionBuilds(ISet<EquatableEdge<IBuilder>> graph)
        {
            // Searching for [MSBuildRunner] -> [SlnBuilder] -> [ISlnProjectBuilder*] patterns
            var patterns =
                graph
                    .Where(edge => edge.Source is MSBuildRunner && edge.Target is SlnBuilder)
                    .ToDictionary(
                        edge => (SlnBuilder) edge.Target,
                        edge => new SolutionBuildPattern((MSBuildRunner) edge.Source, (SlnBuilder) edge.Target));

            foreach (var edge in graph)
            {
                if (edge.Source is SlnBuilder && edge.Target is ISlnProjectBuilder)
                {
                    SolutionBuildPattern pattern;
                    if (patterns.TryGetValue((SlnBuilder)edge.Source, out pattern))
                    {
                        pattern.ProjectBuilders.Add((ISlnProjectBuilder) edge.Target);
                    }
                }
            }

            // Finding involved modules
            var modules = new HashSet<Module>();
            var projects = new HashSet<Project>();

            foreach (var pattern in patterns.Values)
            {
                foreach (var prj in pattern.ProjectBuilders)
                {
                    projects.Add(prj.Project);
                    modules.Add(prj.Project.Module);
                }
            }

            // Creating merge plan
            foreach (var module in modules)
            {
                bool testsCovered = module.TestProjects.All(projects.Contains);
                bool covered = module.Projects.All(projects.Contains);

                if (covered)
                    log.DebugFormat("Merging project builders of module {0} into a single solution", module.Name);
                if (testsCovered)
                    log.DebugFormat("Merging test project builders of module {0} into a single solution", module.Name);
            }

            return true;
        }
    }
}