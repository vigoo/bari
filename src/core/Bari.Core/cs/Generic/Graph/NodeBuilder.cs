using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Graph builder algorithms, extending the interfaces <see cref="IUndirectedGraphEdge{T}"/> and
    /// <see cref="IDirectedGraphEdge{T}"/>.
    /// </summary>
    public static class NodeBuilder
    {
        /// <summary>
        /// Builds the graph node representation of a graph given by its undirected edges.
        /// </summary>
        /// <typeparam name="T">The node value type.</typeparam>
        /// <param name="edges">The set of undirected edges.</param>
        /// <param name="rootData">The root data value.</param>
        /// <returns>Returns a graph node that holds the given root value and represents
        /// the same graph as the set of edges.</returns>
        public static IUndirectedGraphNode<T> BuildNodes<T>(this IEnumerable<IUndirectedGraphEdge<T>> edges, T rootData)
        {
            var map = new Dictionary<T, SimpleUndirectedGraphNode<T>>();

            foreach (IUndirectedGraphEdge<T> edge in edges)
            {
                SimpleUndirectedGraphNode<T> node1;
                if (!map.TryGetValue(edge.Data1, out node1))
                {
                    node1 = new SimpleUndirectedGraphNode<T>(edge.Data1);
                    map.Add(edge.Data1, node1);
                }

                SimpleUndirectedGraphNode<T> node2;
                if (!map.TryGetValue(edge.Data2, out node2))
                {
                    node2 = new SimpleUndirectedGraphNode<T>(edge.Data2);
                    map.Add(edge.Data2, node2);
                }

                Contract.Assume(node1 != null);
                Contract.Assume(node2 != null);

                if (!node1.AdjacentNodes.Contains(node2))
                    node1.Add(node2);
                if (!node2.AdjacentNodes.Contains(node1))
                    node2.Add(node1);
            }

            return map[rootData];
        }

        /// <summary>
        /// Builds the graph node representation of a graph given by its directed edges.
        /// </summary>
        /// <typeparam name="T">The node value type.</typeparam>
        /// <param name="edges">The set of directed edges.</param>
        /// <param name="rootData">The root data value from the result graph.</param>
        /// <returns>Returns a graph node that holds the given root value and represents the same as the set of edges.</returns>
        public static IDirectedGraphNode<T> BuildNodes<T>(this IEnumerable<IDirectedGraphEdge<T>> edges, T rootData)
        {
            return BuildNodes(edges, rootData, false);
        }

        /// <summary>
        /// Builds the graph node representation of a graph given by its directed edges.
        /// </summary>
        /// <typeparam name="T">The node value type.</typeparam>
        /// <param name="edges">The set of directed edges.</param>
        /// <param name="rootData">The root data value.</param>
        /// <param name="removeSelfLoops">if set to <c>true</c> the self-loops will be removed
        /// from the result graph.</param>
        /// <returns>Returns a graph node that holds the given root value and represents the same
        /// (or the same without self-loops) as the set of edges.</returns>
        public static IDirectedGraphNode<T> BuildNodes<T>(this IEnumerable<IDirectedGraphEdge<T>> edges, T rootData, bool removeSelfLoops)
        {
            var map = new Dictionary<T, SimpleDirectedGraphNode<T>>();

            foreach (IDirectedGraphEdge<T> edge in edges)
            {
                SimpleDirectedGraphNode<T> sourceNode;
                if (!map.TryGetValue(edge.Source, out sourceNode))
                {
                    sourceNode= new SimpleDirectedGraphNode<T>(edge.Source);
                    map.Add(edge.Source, sourceNode);
                }

                SimpleDirectedGraphNode<T> targetNode;
                if (!map.TryGetValue(edge.Target, out targetNode))
                {
                    targetNode = new SimpleDirectedGraphNode<T>(edge.Target);
                    map.Add(edge.Target, targetNode);
                }

                Contract.Assume(sourceNode != null);
                Contract.Assume(targetNode != null);

                if (!sourceNode.TargetNodes.Contains(targetNode) && 
                    (!removeSelfLoops || sourceNode != targetNode))
                    sourceNode.AddTarget(targetNode);
                if (!targetNode.SourceNodes.Contains(sourceNode) && 
                    (!removeSelfLoops || sourceNode != targetNode))
                    targetNode.AddSource(sourceNode);
            }

            return map[rootData];
        }

        /// <summary>
        /// Builds the graph node representation of a graph given by its directed edges.
        /// </summary>
        /// <typeparam name="T">The node value type.</typeparam>
        /// <param name="edges">The set of directed edges.</param>
        /// <param name="removeSelfLoops">if set to <c>true</c> the self-loops will be removed
        /// from the result graph.</param>
        /// <returns>Returns the set of nodes represented by the given edges.</returns>
        public static IEnumerable<IDirectedGraphNode<T>> BuildNodes<T>(this IEnumerable<IDirectedGraphEdge<T>> edges, bool removeSelfLoops)
        {
            var map = new Dictionary<T, SimpleDirectedGraphNode<T>>();

            foreach (IDirectedGraphEdge<T> edge in edges)
            {
                SimpleDirectedGraphNode<T> sourceNode;
                if (!map.TryGetValue(edge.Source, out sourceNode))
                {
                    sourceNode = new SimpleDirectedGraphNode<T>(edge.Source);
                    map.Add(edge.Source, sourceNode);
                }

                SimpleDirectedGraphNode<T> targetNode;
                if (!map.TryGetValue(edge.Target, out targetNode))
                {
                    targetNode = new SimpleDirectedGraphNode<T>(edge.Target);
                    map.Add(edge.Target, targetNode);
                }

                Contract.Assume(sourceNode != null);
                Contract.Assume(targetNode != null);

                if (!sourceNode.TargetNodes.Contains(targetNode) &&
                    (!removeSelfLoops || sourceNode != targetNode))
                    sourceNode.AddTarget(targetNode);
                if (!targetNode.SourceNodes.Contains(sourceNode) &&
                    (!removeSelfLoops || sourceNode != targetNode))
                    targetNode.AddSource(sourceNode);
            }

            return map.Values;
        }
    }
}
