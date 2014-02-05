
using System;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Simple implementation of an undirected graph edge.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleUndirectedGraphEdge<T>: IUndirectedGraphEdge<T>
    {
        /// <summary>
        /// Gets one of the node values.
        /// </summary>
        /// <value></value>
        public T Data1 { get; set;}
        /// <summary>
        /// Gets one of the node values.
        /// </summary>
        /// <value></value>
        public T Data2 { get; set;}

        /// <summary>
        /// Initializes a new undirected graph edge
        /// </summary>
        /// <param name="data1">One of the node values.</param>
        /// <param name="data2">One of the node values.</param>
        public SimpleUndirectedGraphEdge(T data1, T data2)
        {
            Data1 = data1;
            Data2 = data2;
        }

        public override string ToString()
        {
            return String.Format("{0} --- {1}", Data1, Data2);
        }
    }
}
