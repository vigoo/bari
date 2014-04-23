using System;
using System.Collections.Generic;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Graph traversal algorithms which extend the <see cref="IDirectedGraphNode{T}"/> and
    /// <see cref="IUndirectedGraphNode{T}"/> interfaces.
    /// </summary>
    public static class GraphTraversal
    {
        /// <summary>
        /// Traverses an undirected connected graph given by its nodes in breadth-first
        /// ordering, and applies an accumulator function to each node.
        /// </summary>
        /// <typeparam name="TData">The type of the node data.</typeparam>
        /// <typeparam name="TResult">The type of the accumulated result.</typeparam>
        /// <param name="root">The root node, where the search starts.</param>
        /// <param name="function">The accumulator function, called for each node with the 
        /// current accumulated value.</param>
        /// <param name="initial">The initial accumulated value.</param>
        /// <returns>Returns the final accumulated value.</returns>
        public static TResult UndirectedBreadthFirstTraversal<TData, TResult>(this IUndirectedGraphNode<TData> root,
            Func<TData, TResult, TResult> function, TResult initial)
        {
            var queue = new Queue<IUndirectedGraphNode<TData>>();
            var visited = new HashSet<IUndirectedGraphNode<TData>>();
            queue.Enqueue(root);
            TResult acc = initial;

            while (queue.Count > 0)
            {
                var elem = queue.Dequeue();

                if (!visited.Contains((elem)))
                {
                    acc = function(elem.Data, acc);

                    visited.Add(elem);

                    foreach (IUndirectedGraphNode<TData> adjacentNode in elem.AdjacentNodes)
                        queue.Enqueue(adjacentNode);
                }
            }

            return acc;
        }

        /// <summary>
        /// Traverses a directed connected graph given by its nodes in breadth-first
        /// ordering, and applies an accumulator function to each node.
        /// 
        /// <para>
        /// The search is directed, which means that transitions are only performed 
        /// from source to target nodes.
        /// </para>
        /// </summary>
        /// <typeparam name="TData">The type of the node data.</typeparam>
        /// <typeparam name="TResult">The type of the accumulated result.</typeparam>
        /// <param name="root">The root node, where the search starts.</param>
        /// <param name="function">The accumulator function, called for each node with the 
        /// current accumulated value.</param>
        /// <param name="initial">The initial accumulated value.</param>
        /// <returns>Returns the final accumulated value.</returns>
        public static TResult DirectedBreadthFirstTraversal<TData, TResult>(this IDirectedGraphNode<TData> root,
            Func<TData, TResult, TResult> function, TResult initial)
        {
            var queue = new Queue<IDirectedGraphNode<TData>>();
            var visited = new HashSet<IUndirectedGraphNode<TData>>();
            queue.Enqueue(root);
            TResult acc = initial;

            while (queue.Count > 0)
            {
                var elem = queue.Dequeue();
                acc = function(elem.Data, acc);

                visited.Add(elem);

                foreach (IDirectedGraphNode<TData> targetNode in elem.TargetNodes)
                    if (!visited.Contains(targetNode))
                        queue.Enqueue(targetNode);
            }

            return acc;
        }

        /// <summary>
        /// Traverses an undirected connected graph given by its nodes in depth-first
        /// ordering, and applies an accumulator function to each node.
        /// </summary>
        /// <typeparam name="TData">The type of the node data.</typeparam>
        /// <typeparam name="TResult">The type of the accumulated result.</typeparam>
        /// <param name="root">The root node, where the search starts.</param>
        /// <param name="function">The accumulator function, called for each node with the 
        /// current accumulated value.</param>
        /// <param name="initial">The initial accumulated value.</param>
        /// <returns>Returns the final accumulated value.</returns>
        public static TResult UndirectedDepthFirstTraversal<TData, TResult>(this IUndirectedGraphNode<TData> root,
            Func<TData, TResult, TResult> function, TResult initial)
        {
            var stack = new Stack<IUndirectedGraphNode<TData>>();
            var visited = new HashSet<IUndirectedGraphNode<TData>>();
            TResult acc = initial;

            stack.Push(root);

            while (stack.Count > 0)
            {
                var elem = stack.Pop();
                acc = function(elem.Data, acc);
                visited.Add(elem);

                foreach (IUndirectedGraphNode<TData> adjacentNode in elem.AdjacentNodes)
                {
                    if (!visited.Contains(adjacentNode))
                    {
                        stack.Push(adjacentNode);
                    }
                }
            }

            return acc;
        }


        /// <summary>
        /// Traverses a directed connected graph given by its nodes in depth-first
        /// ordering, and applies an accumulator function to each node.
        /// 
        /// <para>
        /// The search is directed, which means that transitions are only performed 
        /// from source to target nodes.
        /// </para>
        /// </summary>
        /// <typeparam name="TData">The type of the node data.</typeparam>
        /// <typeparam name="TResult">The type of the accumulated result.</typeparam>
        /// <param name="root">The root node, where the search starts.</param>
        /// <param name="function">The accumulator function, called for each node with the 
        /// current accumulated value.</param>
        /// <param name="initial">The initial accumulated value.</param>
        /// <returns>Returns the final accumulated value.</returns>
        public static TResult DirectedDepthFirstTraversal<TData, TResult>(this IDirectedGraphNode<TData> root,
            Func<TData, TResult, TResult> function, TResult initial)
        {
            var stack = new Stack<IDirectedGraphNode<TData>>();
            var visited = new HashSet<IDirectedGraphNode<TData>>();
            TResult acc = initial;

            stack.Push(root);

            while (stack.Count > 0)
            {
                var elem = stack.Pop();
                acc = function(elem.Data, acc);
                visited.Add(elem);

                foreach (IDirectedGraphNode<TData> targetNode in elem.TargetNodes)
                {
                    if (!visited.Contains(targetNode))
                    {
                        stack.Push(targetNode);
                    }
                }
            }

            return acc;
        }
    }
}
