namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Undirected graph edge
    /// </summary>
    /// <typeparam name="T">Node value type</typeparam>
    public interface IUndirectedGraphEdge<out T>
    {
        /// <summary>
        /// Gets one of the node values .
        /// </summary>
        T Data1 { get; }

        /// <summary>
        /// Gets one of the node values.
        /// </summary>
        T Data2 { get; }
    }
}
