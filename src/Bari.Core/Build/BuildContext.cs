using System;
using System.Collections.Generic;
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
    public class BuildContext: IBuildContext
    {
        private readonly List<IDirectedGraphEdge<IBuilder>> builders = new List<IDirectedGraphEdge<IBuilder>>();
        private readonly IDictionary<IBuilder, ISet<TargetRelativePath>> partialResults =
            new Dictionary<IBuilder, ISet<TargetRelativePath>>();

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
        /// Runs all the added builders
        /// </summary>
        /// <returns>Returns the union of result paths given by all the builders added to the context</returns>
        public ISet<TargetRelativePath> Run()
        {
            partialResults.Clear();

            var result = new HashSet<TargetRelativePath>();            
            var nodes = builders.BuildNodes(removeSelfLoops: true);
            foreach (var builder in nodes.TopologicalSort())
            {
                var builderResult = builder.Run(this);

                partialResults.Add(builder, builderResult);
                result.UnionWith(builderResult);
            }

            return result;
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
    }
}