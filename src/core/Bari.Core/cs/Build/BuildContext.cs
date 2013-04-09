using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build.Cache;
using Bari.Core.Generic;
using Bari.Core.Generic.Graph;

namespace Bari.Core.Build
{
    /// <summary>
    /// The default <see cref="IBuildContext"/> implementation. 
    /// 
    /// <para>Build context collects a set of <see cref="IBuilder"/> instances to be 
    /// executed and ensures that they are started in topological order according
    /// to their dependency constraints.</para>
    /// </summary>
    public class BuildContext : IBuildContext
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BuildContext));

        private readonly List<IDirectedGraphEdge<IBuilder>> builders = new List<IDirectedGraphEdge<IBuilder>>();
        private readonly IDictionary<IBuilder, ISet<TargetRelativePath>> partialResults =
            new Dictionary<IBuilder, ISet<TargetRelativePath>>();
        private readonly ISet<Func<List<IDirectedGraphEdge<IBuilder>>, bool>> graphTransformations =
            new HashSet<Func<List<IDirectedGraphEdge<IBuilder>>, bool>>();

        private readonly ICachedBuilderFactory cachedBuilderFactory;

        /// <summary>
        /// Initializes the build context
        /// </summary>
        /// <param name="cachedBuilderFactory">Interface to create new cached builders</param>
        public BuildContext(ICachedBuilderFactory cachedBuilderFactory)
        {
            this.cachedBuilderFactory = cachedBuilderFactory;
        }

        /// <summary>
        /// Adds a new builder to be executed to the context
        /// </summary>
        /// <param name="builder">The builder to be executed</param>
        /// <param name="prerequisites">Builder's prerequisites. The prerequisites must be added
        /// separately with the <see cref="IBuildContext.AddBuilder"/> method, listing them here only changes the
        /// order in which they are executed.</param>
        public void AddBuilder(IBuilder builder, IEnumerable<IBuilder> prerequisites)
        {
            builders.Add(new SimpleDirectedGraphEdge<IBuilder>(builder, builder));

            foreach (var prerequisite in prerequisites)
                builders.Add(new SimpleDirectedGraphEdge<IBuilder>(builder, prerequisite));
        }

        /// <summary>
        /// Adds a new graph transformation which will be executed before the builders
        /// </summary>
        /// <param name="transformation">Transformation function, returns <c>false</c> to cancel the build process</param>
        public void AddTransformation(Func<List<IDirectedGraphEdge<IBuilder>>, bool> transformation)
        {
            graphTransformations.Add(transformation);
        }

        /// <summary>
        /// Runs all the added builders
        /// </summary>
        /// <param name="rootBuilder">The root builder which represents the final goal of the build process.
        /// If specified, every branch which is not accessible from the root builder will be removed
        /// from the build graph before executing it.</param>
        /// <returns>Returns the union of result paths given by all the builders added to the context</returns>
        public ISet<TargetRelativePath> Run(IBuilder rootBuilder = null)
        {
            var result = new HashSet<TargetRelativePath>();

            partialResults.Clear();

            var cancel = RunTransformations();

            if (!cancel)
            {
                if (rootBuilder != null)
                    RemoveIrrelevantBranches(rootBuilder);

                var nodes = builders.BuildNodes(removeSelfLoops: true);
                var sortedBuilders = nodes.TopologicalSort().ToList();

                log.DebugFormat("Build order: {0}\n", String.Join("\n ", sortedBuilders));

                foreach (var builder in sortedBuilders)
                {
                    var cachedBuilder = cachedBuilderFactory.CreateCachedBuilder(builder);
                    var builderResult = cachedBuilder.Run(this);

                    partialResults.Add(builder, builderResult);
                    result.UnionWith(builderResult);
                }
            }
            else
            {
                log.DebugFormat("Build cancelled by graph transformation");
            }

            return result;
        }

        private void RemoveIrrelevantBranches(IBuilder rootBuilder)
        {
            var rootNode = builders.BuildNodes(rootBuilder, true);
            var toKeep = rootNode.DirectedBreadthFirstTraversal(
                (node, set) =>
                {
                    set.Add(node);
                    return set;
                }, new HashSet<IBuilder>());

            builders.RemoveAll(
                edge => !toKeep.Contains(edge.Source) ||
                        !toKeep.Contains(edge.Target));
        }

        private bool RunTransformations()
        {
            bool cancel = graphTransformations.Any(graphTransformation => !graphTransformation(builders));
            return cancel;
        }

        /// <summary>
        /// Gets the result paths returned by the given builder if it has already ran. Otherwise it throws an
        /// exception.
        /// </summary>
        /// <param name="builder">Builder which was added previously with <see cref="IBuildContext.AddBuilder"/> and was already executed.</param>
        /// <returns>Return the return value of the builder's <see cref="IBuilder.Run"/> method.</returns>
        public ISet<TargetRelativePath> GetResults(IBuilder builder)
        {
            ISet<TargetRelativePath> builderResult;
            if (partialResults.TryGetValue(builder, out builderResult))
                return builderResult;
            else
                throw new InvalidOperationException("Builder has not ran in this context");
        }

        /// <summary>
        /// Gets the dependent builders of a given builder
        /// </summary>
        /// <param name="builder">Builder to get dependencies of</param>
        /// <returns>A possibly empty enumeration of builders</returns>
        public IEnumerable<IBuilder> GetDependencies(IBuilder builder)
        {
            return from edge in builders
                   where edge.Source == builder && edge.Target != builder
                   select edge.Target;
        }

        /// <summary>
        /// Dumps the build context to dot files
        /// </summary>
        /// <param name="builderGraphStream">Stream where the builder graph will be dumped</param>
        /// <param name="rootBuilder">The root builder</param>
        public void Dump(Stream builderGraphStream, IBuilder rootBuilder)
        {
            RunTransformations();
            if (rootBuilder != null)
                RemoveIrrelevantBranches(rootBuilder);

            using (var writer = new DotWriter(builderGraphStream))
            {
                writer.Rankdir = "RL";
                writer.WriteGraph(builders);
            }
        }
    }
}