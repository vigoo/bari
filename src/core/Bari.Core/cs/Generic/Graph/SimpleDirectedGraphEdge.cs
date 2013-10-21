using System;
using System.Collections.Generic;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Simple implementation of a directed graph edge
    /// </summary>
    /// <typeparam name="T">The node value type.</typeparam>
    public class SimpleDirectedGraphEdge<T>: IDirectedGraphEdge<T>, IEquatable<SimpleDirectedGraphEdge<T>>
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

        public bool Equals(SimpleDirectedGraphEdge<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Source, other.Source) && EqualityComparer<T>.Default.Equals(Target, other.Target);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SimpleDirectedGraphEdge<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(Source)*397) ^ EqualityComparer<T>.Default.GetHashCode(Target);
            }
        }

        public static bool operator ==(SimpleDirectedGraphEdge<T> left, SimpleDirectedGraphEdge<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SimpleDirectedGraphEdge<T> left, SimpleDirectedGraphEdge<T> right)
        {
            return !Equals(left, right);
        }
    }
}
