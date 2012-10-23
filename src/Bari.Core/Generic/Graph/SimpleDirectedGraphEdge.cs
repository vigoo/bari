namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Simple implementation of a directed graph edge
    /// </summary>
    /// <typeparam name="T">The node value type.</typeparam>
    public class SimpleDirectedGraphEdge<T>: IDirectedGraphEdge<T>
    {
        /// <summary>
        /// Gets the source node's value.
        /// </summary>
        public T Source { get; set; }

        /// <summary>
        /// Gets the target node's value.
        /// </summary>
        public T Target { get; set; }

        /// <summary>
        /// Initializes a new directed graph edge between two
        /// node values.
        /// </summary>
        /// <param name="source">The source node value.</param>
        /// <param name="target">The target node value.</param>
        public SimpleDirectedGraphEdge(T source, T target)
        {
            Source = source;
            Target = target;
        }
    }
}
