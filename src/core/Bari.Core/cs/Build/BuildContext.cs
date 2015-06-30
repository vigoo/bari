using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build.Cache;
using Bari.Core.Generic;
using Bari.Core.Generic.Graph;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Search;
using Bari.Core.Build.Statistics;
using Bari.Core.UI;

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

        private readonly ISet<EquatableEdge<IBuilder>> builders = new HashSet<EquatableEdge<IBuilder>>();
        private readonly IDictionary<IBuilder, ISet<TargetRelativePath>> partialResults =
            new Dictionary<IBuilder, ISet<TargetRelativePath>>();
        private readonly ISet<Func<ISet<EquatableEdge<IBuilder>>, bool>> graphTransformations =
            new HashSet<Func<ISet<EquatableEdge<IBuilder>>, bool>>();

        private readonly ICachedBuilderFactory cachedBuilderFactory;
		private readonly IMonitoredBuilderFactory monitoredBuilderFactory;
		private readonly Func<IBuilderStatistics> createBuilderStatistics;

        /// <summary>
        /// Initializes the build context
        /// </summary>
        /// <param name="cachedBuilderFactory">Interface to create new cached builders</param>
		/// <param name="monitoredBuilderFactory">Interface to create new monitored builders</param>
		/// <param name="createBuilderStatistics">Factory function for IBuilderStatistics instances</param>
		public BuildContext(ICachedBuilderFactory cachedBuilderFactory, IMonitoredBuilderFactory monitoredBuilderFactory, Func<IBuilderStatistics> createBuilderStatistics)
        {
            this.cachedBuilderFactory = cachedBuilderFactory;
			this.monitoredBuilderFactory = monitoredBuilderFactory;
			this.createBuilderStatistics = createBuilderStatistics;
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
            builders.Add(new EquatableEdge<IBuilder>(builder, builder));

            foreach (var prerequisite in prerequisites)
            {
                builders.Add(new EquatableEdge<IBuilder>(prerequisite, builder));
            }
        }

        /// <summary>
        /// Adds a new graph transformation which will be executed before the builders
        /// </summary>
        /// <param name="transformation">Transformation function, returns <c>false</c> to cancel the build process</param>
        public void AddTransformation(Func<ISet<EquatableEdge<IBuilder>>, bool> transformation)
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
                var graph = builders.ToAdjacencyGraph<IBuilder, EquatableEdge<IBuilder>>();
                graph.RemoveEdgeIf(edge => edge.IsSelfEdge<IBuilder, EquatableEdge<IBuilder>>());
                
                if (rootBuilder != null)
                    RemoveIrrelevantBranches(graph, rootBuilder);

                if (!HasCycles(graph))
                {
                    var sortedBuilders = graph.TopologicalSort().ToList();

                    log.DebugFormat("Build order:\n {0}\n", String.Join("\n ", sortedBuilders));

					var statistics = createBuilderStatistics();

                    foreach (var builder in sortedBuilders)
                    {
                        log.DebugFormat("===> {0}", builder);

						var wrappedBuilder = WrapBuilder(builder, statistics);

						var builderResult = wrappedBuilder.Run(this);

                        partialResults.Add(builder, builderResult);
                        result.UnionWith(builderResult);
                    }

					statistics.Dump();
                }
                else
                {
                    log.ErrorFormat("Build graph has cycle");
                    result.Clear();
                }
            }
            else
            {
                log.DebugFormat("Build cancelled by graph transformation");
                result.Clear();
            }

            return result;
        }

		private IBuilder WrapBuilder(IBuilder builder, IBuilderStatistics statistics)
		{
			return CreateMonitoredBuilder(CreateCachedBuilder(builder, statistics), statistics);
		}

		private IBuilder CreateMonitoredBuilder(IBuilder builder, IBuilderStatistics statistics)
		{
			return monitoredBuilderFactory.CreateMonitoredBuilder(builder, statistics);
		}

		private IBuilder CreateCachedBuilder(IBuilder builder, IBuilderStatistics statistics)
        {
            if (builder.GetType().GetCustomAttributes(typeof (ShouldNotCacheAttribute), true).Any())
            {
                return builder;
            }
            else
            {
				return cachedBuilderFactory.CreateCachedBuilder(CreateMonitoredBuilder(builder, statistics));
            }
        }

        private bool HasCycles(IVertexListGraph<IBuilder, EquatableEdge<IBuilder>> graph)
        {            
            var dfs = new DepthFirstSearchAlgorithm<IBuilder, EquatableEdge<IBuilder>>(graph);
            Boolean isDag = true;
            EdgeAction<IBuilder, EquatableEdge<IBuilder>> onBackEdge = edge =>
            {
                isDag = false;
                log.DebugFormat("Back edge: {0} -> {1}", edge.Source, edge.Target);
            };

            try
            {
                dfs.BackEdge += onBackEdge;
                dfs.Compute();
            }
            finally
            {
                dfs.BackEdge -= onBackEdge;
            }

            return !isDag;
        }

        private void RemoveIrrelevantBranches(AdjacencyGraph<IBuilder, EquatableEdge<IBuilder>> graph, IBuilder rootBuilder)
        {
            var bfs = new BreadthFirstSearchAlgorithm<IBuilder, EquatableEdge<IBuilder>>(graph);
            bfs.Compute(rootBuilder);
            var toKeep = new HashSet<IBuilder>(bfs.VisitedGraph.Vertices);
            graph.RemoveVertexIf(v => !toKeep.Contains(v));
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
                   where Equals(edge.Target, builder) && !Equals(edge.Source, builder)
                   select edge.Source;
        }

        /// <summary>
        /// Dumps the build context to dot files
        /// </summary>
        /// <param name="builderGraphStream">Stream where the builder graph will be dumped</param>
        /// <param name="rootBuilder">The root builder</param>
        public void Dump(Stream builderGraphStream, IBuilder rootBuilder)
        {
            RunTransformations();

            var graph = builders.ToAdjacencyGraph<IBuilder, EquatableEdge<IBuilder>>();
            graph.RemoveEdgeIf(edge => edge.IsSelfEdge<IBuilder, EquatableEdge<IBuilder>>());

            if (rootBuilder != null)
                RemoveIrrelevantBranches(graph, rootBuilder);

            using (var writer = new DotWriter(builderGraphStream))
            {
                writer.Rankdir = "RL";
                writer.WriteGraph(graph.Edges);
            }
        }

        ///<summary>
        ///Dumps the dependencies of the builder
        ///</summary>
        ///<param name="rootBuilder">The root builder</param>
        ///<param name="output">Output to dump information to</param>
        public void DumpDependencies(IBuilder rootBuilder, IUserOutput output)
        {
            rootBuilder.Dependencies.Dump(output);
        }

        /// <summary>
        /// Checks whether the given builder was already added to the context
        /// </summary>
        /// <param name="builder">Builder to look for</param>
        /// <returns>Returns <c>true</c> if the builder is added to the context</returns>
        public bool Contains(IBuilder builder)
        {
            return builders.Any(edge => Equals(edge.Source, builder) || Equals(edge.Target, builder));
        }
        
        /// <summary>
        /// Gets the registered effective builder instance for a given builder at the given context
        /// </summary>
        /// <param name="builder">Builder to resolve</param>
        /// <returns>Returns the builer itself or its transformed form</returns>
        public IBuilder GetEffectiveBuilder(IBuilder builder)
        {
            if (Contains(builder))
                return builder;
            else
                throw new ArgumentOutOfRangeException("builder", "Builder is not added to context");
        }

        /// <summary>
        /// Gets all the result files under the given subdirectory of target root
        /// </summary>
        /// <param name="targetDir">Subdirectory of target</param>
        /// <returns>An enumeration of target relative paths all pointing to files 
        /// generated under the current build context to the given subdirectory or one of 
        /// its children.</returns>
        public IEnumerable<TargetRelativePath> GetAllResultsIn(TargetRelativePath targetDir)
        {
            var prefix = (string)targetDir;
            if (prefix[prefix.Length - 1] != Path.DirectorySeparatorChar)
                prefix += Path.DirectorySeparatorChar;

            return partialResults.Values.SelectMany(ps => ps.Where(p => ((string)p).StartsWith(prefix)));
        }

        /// <summary>
        /// Gets the root context instance
        /// </summary>
        public IBuildContext RootContext
        {
            get { return this; }
        }
    }
}