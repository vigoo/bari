using System;
using System.Collections.Generic;
using System.Linq;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Simple implementation of a directed graph node
    /// </summary>
    /// <typeparam name="T">The node value type.</typeparam>
    public class SimpleDirectedGraphNode<T> : IDirectedGraphNode<T>, IEquatable<SimpleDirectedGraphNode<T>>
    {
        private readonly IList<IDirectedGraphNode<T>> targets = new List<IDirectedGraphNode<T>>();
        private readonly IList<IDirectedGraphNode<T>> sources = new List<IDirectedGraphNode<T>>();

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
            get
            {
                return targets.Cast<IUndirectedGraphNode<T>>().Concat(
                       sources).Distinct();
            }
        }

        /// <summary>
        /// Gets the set of nodes that are targets of directed edges
        /// which have this node as a source.
        /// </summary>
        /// <value></value>
        public IEnumerable<IDirectedGraphNode<T>> TargetNodes
        {
            get { return targets; }
        }

        /// <summary>
        /// Gets the set of nodes that are sources of directed edges
        /// which have this node as a target.
        /// </summary>
        /// <value></value>
        public IEnumerable<IDirectedGraphNode<T>> SourceNodes
        {
            get { return sources; }
        }

        /// <summary>
        /// Initializes a leaf node.
        /// </summary>
        /// <param name="data">The data value.</param>
        public SimpleDirectedGraphNode(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Initializes a node
        /// </summary>
        /// <param name="data">The data value.</param>
        /// <param name="s">The adjacent nodes that are sources.</param>
        /// <param name="t">The adjacent nodes that are targets.</param>
        public SimpleDirectedGraphNode(T data, IEnumerable<IDirectedGraphNode<T>> s, IEnumerable<IDirectedGraphNode<T>> t)
        {
            Data = data;

            foreach (IDirectedGraphNode<T> node in t)
            {
                targets.Add(node);
            }

            foreach (IDirectedGraphNode<T> node in s)
            {
                sources.Add(node);
            }
        }

        /// <summary>
        /// Adds a source adjacent node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddSource(IDirectedGraphNode<T> node)
        {
            sources.Add(node);
        }

        /// <summary>
        /// Adds the target adjacent node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddTarget(IDirectedGraphNode<T> node)
        {
            targets.Add(node);
        }

        public bool Equals(SimpleDirectedGraphNode<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Data, other.Data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SimpleDirectedGraphNode<T>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Data);
        }

        public static bool operator ==(SimpleDirectedGraphNode<T> left, SimpleDirectedGraphNode<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SimpleDirectedGraphNode<T> left, SimpleDirectedGraphNode<T> right)
        {
            return !Equals(left, right);
        }
    }
}
