using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.MergingTag;
using Bari.Core.Model;
using QuickGraph;


namespace Bari.Plugins.VsCore.Build
{
    public class OptimizingBuildContextFactory : IBuildContextFactory
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(OptimizingBuildContextFactory));

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
            log.Debug("### Cutting redundant solution builds");

            var slnBuilders = new HashSet<SlnBuilder>(graph.Select(edge => edge.Target).OfType<SlnBuilder>());
            var rootBuilderMap = graph
                .Where(edge => edge.Source is MSBuildRunner && slnBuilders.Contains(edge.Target))
                .ToDictionary(edge => (SlnBuilder)edge.Target, edge => FindSolutionRootBuilder(graph, edge));

            if (slnBuilders.Any())
            {
                foreach (var slnBuilder in slnBuilders)
                {
                    if (rootBuilderMap.ContainsKey(slnBuilder))
                    {
                        var projectSet = new HashSet<Project>(slnBuilder.Projects);
                        var childProjectBuilders = new HashSet<ISlnProjectBuilder>(slnBuilder
                            .Prerequisites
                            .OfType<ISlnProjectBuilder>());

                        foreach (var projectBuilder in childProjectBuilders)
                        {
                            foreach (var dep in projectBuilder.Prerequisites.OfType<SuiteReferenceBuilder>().ToList())
                            {
                                if (dep.Reference.Type == ReferenceType.Build &&
                                    projectSet.Contains(dep.ReferencedProject))
                                {
                                    log.DebugFormat(
                                        "Project {0}'s reference {1} can be replaced to in-solution-reference in {2}",
                                        projectBuilder.Project, dep, slnBuilder);

                                    // All sln project builders belonging to `slnBuilder` must replace their reference to dep to a 
                                    // new in solution reference builder (both in the builder and in the graph)
                                    ReplaceWithInSolutionReference(graph, childProjectBuilders, dep);

                                    // All edges from dep must be removed and a single new edge to the MSBuild runner belonging to this `slnBuilder` added
                                    RemoveEdgesWhereSourceIs(graph, dep);

                                    var newEdge = new EquatableEdge<IBuilder>(dep, rootBuilderMap[slnBuilder]);
                                    AddEdge(graph, newEdge);
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static IBuilder FindSolutionRootBuilder(ISet<EquatableEdge<IBuilder>> graph, EquatableEdge<IBuilder> edge)
        {
            var msbuild = (MSBuildRunner)edge.Source;
            IBuilder result = FollowMergingSources(graph, msbuild);            

            log.DebugFormat("Found solution root builder for {0}: {1}", edge.Target, result);

            return result;
        }

        private static IBuilder FollowMergingSources(ISet<EquatableEdge<IBuilder>> graph, IBuilder target)
        {
            var mergeParentEdge = graph.FirstOrDefault(e => e.Target != e.Source && e.Target == target && e.Source is MergingBuilder && !HasDifferentMergingTag(e.Source, e.Target));
            return mergeParentEdge != null ? FollowMergingSources(graph, mergeParentEdge.Source) : target;
        }

        private static bool HasDifferentMergingTag(IBuilder a, IBuilder b)
        {
            if (a is MergingBuilder && b is MergingBuilder)
            {
                return !((MergingBuilder) a).Tag.Equals(((MergingBuilder) b).Tag);
            }
            else
            {
                return false;
            }
        }

        void RemoveEdgesWhereSourceIs(ISet<EquatableEdge<IBuilder>> graph, IBuilder source)
        {
            var toRemove = new HashSet<EquatableEdge<IBuilder>>(graph.Where(edge => edge.Source == source));
            RemoveEdges(graph, toRemove);
        }


        void ReplaceWithInSolutionReference(ISet<EquatableEdge<IBuilder>> graph, IEnumerable<ISlnProjectBuilder> childProjectBuilders, SuiteReferenceBuilder dep)
        {
            var inSolutionRef = inSolutionReferenceBuilderFactory.CreateInSolutionReferenceBuilder(dep.ReferencedProject);
            inSolutionRef.Reference = dep.Reference;

            foreach (var builder in childProjectBuilders)
            {
                builder.RemovePrerequisite(dep);

                var edgesToModify = new HashSet<EquatableEdge<IBuilder>>(graph.Where(edge => edge.Source == builder && edge.Target == dep));
                RemoveEdges(graph, edgesToModify);

                foreach (var edge in edgesToModify)
                {
                    var newEdge = new EquatableEdge<IBuilder>(edge.Source, inSolutionRef);
                    AddEdge(graph, newEdge);
                }
            }
        }

        private static void AddEdge(ISet<EquatableEdge<IBuilder>> graph, EquatableEdge<IBuilder> newEdge)
        {
            log.DebugFormat("-> adding edge {0}", newEdge);
            newEdge.Source.AddPrerequisite(newEdge.Target);
            graph.Add(newEdge);
        }

        private static void RemoveEdges(ISet<EquatableEdge<IBuilder>> graph, ISet<EquatableEdge<IBuilder>> edges)
        {
            foreach (var edge in edges)
            {
                log.DebugFormat("-> removing edge {0}", edge);
                edge.Source.RemovePrerequisite(edge.Target);
            }
            graph.ExceptWith(edges);
        }


        private class SolutionBuildPattern
        {
            private readonly MSBuildRunner msbuildRunner;
            private readonly SlnBuilder slnBuilder;
            private readonly Lazy<IBuilder> root; 
            private readonly ISet<ISlnProjectBuilder> projectBuilders = new HashSet<ISlnProjectBuilder>();
            private readonly ISet<Project> projects = new HashSet<Project>();

            public SolutionBuildPattern(ISet<EquatableEdge<IBuilder>> graph, MSBuildRunner msbuildRunner, SlnBuilder slnBuilder)
            {
                this.msbuildRunner = msbuildRunner;
                this.slnBuilder = slnBuilder;

                root = new Lazy<IBuilder>(() => FollowMergingSources(graph, msbuildRunner));
            }

            public MSBuildRunner MsbuildRunner
            {
                get { return msbuildRunner; }
            }

            public IBuilder Root
            {
                get { return root.Value; }
            }

            public SlnBuilder SlnBuilder
            {
                get { return slnBuilder; }
            }

            public ISet<ISlnProjectBuilder> ProjectBuilders
            {
                get { return projectBuilders; }
            }

            public ISet<Project> Projects
            {
                get { return projects; }
            }
        }

        private bool MergeSolutionBuilds(ISet<EquatableEdge<IBuilder>> graph)
        {
            log.Debug("### Merging solution builds");

            // Searching for [MSBuildRunner] -> [SlnBuilder] -> [ISlnProjectBuilder*] patterns
            var patterns =
                graph
                    .Where(edge => edge.Source is MSBuildRunner && edge.Target is SlnBuilder)
                    .ToDictionary(
                        edge => (SlnBuilder)edge.Target,
                        edge => new SolutionBuildPattern(graph, (MSBuildRunner)edge.Source, (SlnBuilder)edge.Target));

            foreach (var edge in graph)
            {
                if (edge.Source is SlnBuilder && edge.Target is ISlnProjectBuilder)
                {
                    SolutionBuildPattern pattern;
                    if (patterns.TryGetValue((SlnBuilder)edge.Source, out pattern))
                    {
                        var targetBuilder = (ISlnProjectBuilder)edge.Target;
                        pattern.ProjectBuilders.Add(targetBuilder);
                        pattern.Projects.Add(targetBuilder.Project);
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
                bool testsCovered = module.TestProjects.Any() && module.TestProjects.All(projects.Contains);
                bool covered = module.Projects.Any() && module.Projects.All(projects.Contains);

                if (covered || testsCovered)
                {
                    Project[] coveredProjects;
                    if (covered && !testsCovered)
                        coveredProjects = module.Projects.ToArray();
                    else if (!covered)
                        coveredProjects = module.TestProjects.Cast<Project>().ToArray();
                    else
                        coveredProjects = module.Projects.Concat(module.TestProjects).ToArray();

                    log.DebugFormat("Merging project builders of module {0} into a single solution", module.Name);

                    var existingPattern = patterns.Values.FirstOrDefault(
                        p => coveredProjects.All(p.Projects.Contains));
                    IBuilder mergedRoot;
                    if (existingPattern == null)
                    {
                        // Creating the new [MSBuildRunner] -> [SlnBuilder] -> ... branches
                        mergedRoot = CreateMergedBuild(graph, coveredProjects, String.Format("Merged builders of module {0}", module.Name));
                    }
                    else
                    {
                        mergedRoot = existingPattern.Root;
                    }

                    // Redirecting all * -> [MSBuildRunner] edges to the merged [MSBuildRunner]
                    RerouteEdgesTargeting(
                        graph,
                        new HashSet<IBuilder>(patterns.Values
                            .Where(p => p.Root != mergedRoot)
                            .Where(p => p.Projects.IsSubsetOf(coveredProjects))
                            .Select(p => p.Root)),
                        mergedRoot);

                }
            }

            return true;
        }

        private IBuilder CreateMergedBuild(ISet<EquatableEdge<IBuilder>> graph, IEnumerable<Project> projects, string description)
        {
            var prjs = projects.ToList();

            IBuilder rootBuilder = coreBuilderFactory.Merge(
                projectBuilders
                    .Select(pb => pb.Create(prjs))
                    .Where(b => b != null).ToArray(),
                new ProjectBuilderTag(description, prjs));

            AddNewBranch(graph, rootBuilder);

            return rootBuilder;
        }

        void AddNewBranch(ISet<EquatableEdge<IBuilder>> graph, IBuilder rootBuilder)
        {
            log.DebugFormat("Adding new node: {0}", rootBuilder);
            graph.Add(new EquatableEdge<IBuilder>(rootBuilder, rootBuilder));
            foreach (var prereq in rootBuilder.Prerequisites)
            {
                var newEdge = new EquatableEdge<IBuilder>(rootBuilder, prereq);

                if (!graph.Contains(newEdge))
                {
                    AddEdge(graph, newEdge);
                    AddNewBranch(graph, prereq);
                }
            }
        }

        private void RerouteEdgesTargeting(ISet<EquatableEdge<IBuilder>> graph, ISet<IBuilder> originalTargets, IBuilder replacementTarget)
        {
            log.DebugFormat("-> Rerouting edges targeting {0} to {1}", string.Join(", ", originalTargets), replacementTarget);

            var edgesToRemove = new HashSet<EquatableEdge<IBuilder>>();
            var edgesToAdd = new HashSet<EquatableEdge<IBuilder>>();
            foreach (var edge in graph)
            {
                if (originalTargets.Contains(edge.Target) && edge.Target != edge.Source && edge.Target != replacementTarget)
                {
                    edgesToRemove.Add(edge);

                    var newEdge = new EquatableEdge<IBuilder>(edge.Source, replacementTarget);
                    edgesToAdd.Add(newEdge);
                }
            }

            RemoveEdges(graph, edgesToRemove);
            foreach (var edge in edgesToAdd)
                AddEdge(graph, edge);
        }
    }
}