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
        private readonly ICoreBuilderFactory coreBuilderFactory;
        private readonly IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory;
        private readonly IEnumerable<IProjectBuilderFactory> projectBuilders;

        public OptimizingBuildContextFactory(IBuildContextFactory originalFactory, ICoreBuilderFactory coreBuilderFactory, IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory, IEnumerable<IProjectBuilderFactory> projectBuilders)
        {
            this.originalFactory = originalFactory;
            this.coreBuilderFactory = coreBuilderFactory;
            this.inSolutionReferenceBuilderFactory = inSolutionReferenceBuilderFactory;
            this.projectBuilders = projectBuilders;
        }

        public IBuildContext CreateBuildContext()
        {
            var buildContext = originalFactory.CreateBuildContext();
            buildContext.AddTransformation(MergeSolutionBuilds);
            buildContext.AddTransformation(CutRedundantSolutionBuilds);
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
                {
                    log.DebugFormat("Merging project builders of module {0} into a single solution", module.Name);

                    // Creating the new [MSBuildRunner] -> [SlnBuilder] -> ... branches
                    var mergedRoot = CreateMergedBuild(graph, module.Projects);

                    // Redirecting all * -> [MSBuildRunner] edges to the merged [MSBuildRunner]
                    RerouteEdgesTargeting(
                        graph,
                        new HashSet<IBuilder>(patterns.Values
                            .Where(p => p.ProjectBuilders.Any(pb => pb.Project.Module == module))
                            .Select(p => p.MsbuildRunner)),
                        mergedRoot);
                        
                }
                if (testsCovered)
                {
                    log.DebugFormat("Merging test project builders of module {0} into a single solution", module.Name);

                    // Creating the new [MSBuildRunner] -> [SlnBuilder] -> ... branches
                    var mergedRoot = CreateMergedBuild(graph, module.TestProjects);

                    // Redirecting all * -> [MSBuildRunner] edges to the merged [MSBuildRunner]
                    RerouteEdgesTargeting(
                        graph,
                        new HashSet<IBuilder>(patterns.Values
                            .Where(p => p.ProjectBuilders.Any(pb => pb.Project.Module == module))
                            .Select(p => p.MsbuildRunner)),
                        mergedRoot);

                }
            }

            return true;
        }

        private IBuilder CreateMergedBuild(ISet<EquatableEdge<IBuilder>> graph, IEnumerable<Project> projects)
        {
            IBuilder rootBuilder = coreBuilderFactory.Merge(
                projectBuilders
                .Select(pb => pb.Create(projects))
                .Where(b => b != null).ToArray());

            AddNewBranch(graph, rootBuilder);

            return rootBuilder;
        }

        void AddNewBranch(ISet<EquatableEdge<IBuilder>> graph, IBuilder rootBuilder)
        {
            log.DebugFormat("Adding new branch: {0}", rootBuilder);
            graph.Add(new EquatableEdge<IBuilder>(rootBuilder, rootBuilder));
            foreach (var prereq in rootBuilder.Prerequisites)
            {
                var newEdge = new EquatableEdge<IBuilder>(rootBuilder, prereq);

                if (!graph.Contains(newEdge))
                {
                    log.DebugFormat("-> Adding new edge: {0}", newEdge);
                    graph.Add(newEdge);
                    AddNewBranch(graph, prereq);
                }
            }
        }

        private void RerouteEdgesTargeting(ISet<EquatableEdge<IBuilder>> graph, ISet<IBuilder> originalTargets, IBuilder replacementTarget)
        {
            var edgesToRemove = new HashSet<EquatableEdge<IBuilder>>();
            var edgesToAdd = new HashSet<EquatableEdge<IBuilder>>();
            foreach (var edge in graph)
            {
                if (originalTargets.Contains(edge.Target) && edge.Target != edge.Source && edge.Target != replacementTarget)
                {
                    log.DebugFormat("-> Removing edge {0}", edge);
                    edgesToRemove.Add(edge);

                    var newEdge = new EquatableEdge<IBuilder>(edge.Source, replacementTarget);
                    log.DebugFormat("-> Replacing with {0}", newEdge);
                    edgesToAdd.Add(newEdge);
                }
            }

            graph.ExceptWith(edgesToRemove);
            graph.UnionWith(edgesToAdd);
        }
    }
}