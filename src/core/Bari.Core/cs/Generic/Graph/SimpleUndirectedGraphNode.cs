using System.Collections.Generic;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Simple implementation of an undirected graph node
    /// </summary>
    /// <typeparam name="T">The node value type.</typeparam>
    public class SimpleUndirectedGraphNode<T>: IUndirectedGraphNode<T>
    {
        private readonly IList<IUndirectedGraphNode<T>> adjacentNodes = new List<IUndirectedGraphNode<T>>();

        /// <summary>
        /// Gets the node data.
        /// </summary>
        /// <value></value>
        public T Data { get; set; }

        /// <summary>
        /// Gets the set of adjacent nodes, which share an edge with this node.
        /// </summary>
        /// <value></value>
        public IEnumerable<IUndirectedGraphNode<T>> AdjacentNodes
        {
            get { return adjacentNodes; }
        }

        /// <summary>
        /// Adds an adjacent node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Add(IUndirectedGraphNode<T> node)
        {
            adjacentNodes.Add(node);
        }

        /// <summary>
        /// Initializes a leaf node.
        /// </summary>
        /// <param name="data">The data value.</param>
        public SimpleUndirectedGraphNode(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Initializes a node.
        /// </summary>
        /// <param name="data">The data value.</param>
        /// <param name="children">The adjacent nodes.</param>
        public SimpleUndirectedGraphNode(T data, params IUndirectedGraphNode<T>[] children)
        {
            Data = data;

            foreach (IUndirectedGraphNode<T> child in children)            
                adjacentNodes.Add(child);            
        }
    }
}
