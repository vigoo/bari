using System.Collections.Generic;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Describes a node of an undirected graph
    /// </summary>
    /// <typeparam name="T">The node value type.</typeparam>
    public interface IUndirectedGraphNode<out T>
    {
        /// <summary>
        /// Gets the node data.
        /// </summary>
        T Data { get; }

        /// <summary>
        /// Gets the set of adjacent nodes, which share an edge with this node.
        /// </summary>
        IEnumerable<IUndirectedGraphNode<T>> AdjacentNodes { get; }
    }
}
