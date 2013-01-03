namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Directed graph edge
    /// </summary>
    /// <typeparam name="T">Node value type</typeparam>
    public interface IDirectedGraphEdge<out T>
    {
        /// <summary>
        /// Gets the source node's value.
        /// </summary>
        T Source { get; }

        /// <summary>
        /// Gets the target node's value.
        /// </summary>
        T Target { get; }
    }
}
