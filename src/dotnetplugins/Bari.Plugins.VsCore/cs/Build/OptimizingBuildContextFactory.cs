using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Model;
using QuickGraph;
using YamlDotNet.RepresentationModel.Serialization;

namespace Bari.Plugins.VsCore.Build
{
    public class OptimizingBuildContextFactory: IBuildContextFactory
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (OptimizingBuildContextFactory));

        private readonly IBuildContextFactory originalFactory;

        public OptimizingBuildContextFactory(IBuildContextFactory originalFactory)
        {
            this.originalFactory = originalFactory;
        }

        public IBuildContext CreateBuildContext()
        {
            var buildContext = originalFactory.CreateBuildContext();
            buildContext.AddTransformation(MergeSolutionBuilds);
            return buildContext;
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
                    .Where(edge => edge.Target is MSBuildRunner && edge.Source is SlnBuilder)
                    .ToDictionary(
                        edge => (SlnBuilder) edge.Source,
                        edge => new SolutionBuildPattern((MSBuildRunner) edge.Target, (SlnBuilder) edge.Source));

            foreach (var edge in graph)
            {
                if (edge.Target is SlnBuilder && edge.Source is ISlnProjectBuilder)
                {
                    SolutionBuildPattern pattern;
                    if (patterns.TryGetValue((SlnBuilder)edge.Target, out pattern))
                    {
                        pattern.ProjectBuilders.Add((ISlnProjectBuilder) edge.Source);
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