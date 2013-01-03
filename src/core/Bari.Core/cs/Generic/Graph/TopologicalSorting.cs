using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Computes topological ordering on a graph given by a set of directed graph nodes (<see cref="IDirectedGraphNode{T}"/>)
    /// </summary>
    public static class TopologicalSorting
    {
        /// <summary>
        /// Performs the topological ordering
        /// </summary>
        /// <typeparam name="TData">Node data type</typeparam>
        /// <param name="nodes">Set of directed graph nodes</param>
        /// <returns>Returns the data stored in the graph in order</returns>
        public static IEnumerable<TData> TopologicalSort<TData>(this IEnumerable<IDirectedGraphNode<TData>> nodes)
        {
            return new TopologicalSortingImplementation<TData>(nodes);
        }

        private class TopologicalSortingImplementation<TData>: IEnumerable<TData>
        {
            private readonly IList<IDirectedGraphNode<TData>> result;
            private readonly HashSet<IDirectedGraphNode<TData>> visited;

            public TopologicalSortingImplementation(IEnumerable<IDirectedGraphNode<TData>> nodes)
            {
                result = new List<IDirectedGraphNode<TData>>();
                visited = new HashSet<IDirectedGraphNode<TData>>();

                foreach (var node in nodes)
                {
                    Visit(node);
                }
            }

            private void Visit(IDirectedGraphNode<TData> node)
            {
                if (!visited.Contains(node))
                {
                    visited.Add(node);
                    
                    foreach (var target in node.TargetNodes)
                        Visit(target);

                    result.Add(node);
                }
            }

            public IEnumerator<TData> GetEnumerator()
            {
                return result.Select(node => node.Data).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
