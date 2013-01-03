using System.Collections.Generic;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Describes a node of a directed graph
    /// </summary>
    /// <typeparam name="T">The node value type.</typeparam>
    public interface IDirectedGraphNode<out T>: IUndirectedGraphNode<T>
    {
        /// <summary>
        /// Gets the set of nodes that are targets of directed edges
        /// which have this node as a source.
        /// </summary>
        IEnumerable<IDirectedGraphNode<T>> TargetNodes { get; }

        /// <summary>
        /// Gets the set of nodes that are sources of directed edges
        /// which have this node as a target.
        /// </summary>
        IEnumerable<IDirectedGraphNode<T>> SourceNodes { get; }
    }
}
