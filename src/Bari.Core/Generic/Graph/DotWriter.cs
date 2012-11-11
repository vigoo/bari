using System;
using System.Collections.Generic;
using System.IO;

namespace Bari.Core.Generic.Graph
{
    /// <summary>
    /// Utility class for exporting directed graphs to GraphViz dot format
    /// </summary>
    public sealed class DotWriter: IDisposable
    {
        private readonly TextWriter writer;
        private string rankdir;
        private bool removeSelfLoops = true;

        /// <summary>
        /// Ranking direction, can be LR, RL, TB and BT or <c>null</c> to use the default operation
        /// </summary>
        public string Rankdir
        {
            get { return rankdir; }
            set { rankdir = value; }
        }

        /// <summary>
        /// If <c>true</c>, self loops will be not exported
        /// </summary>
        public bool RemoveSelfLoops
        {
            get { return removeSelfLoops; }
            set { removeSelfLoops = value; }
        }

        /// <summary>
        /// Initializes the dot writer on a local file
        /// </summary>
        /// <param name="path">Path to the file to be written</param>
        public DotWriter(string path)
        {
            writer = new StreamWriter(path);
        }

        /// <summary>
        /// Initializes the dot writer on a stream
        /// </summary>
        /// <param name="targetStream">Stream to be used for export</param>
        public DotWriter(Stream targetStream)
        {
            writer = new StreamWriter(targetStream);
        }

        /// <summary>
        /// Exports a graph to the dot file
        /// 
        /// <para>Every node will be represented by the string returned by its <c>ToString</c> method.</para>
        /// </summary>
        /// <typeparam name="TNode">Node type</typeparam>
        /// <param name="edges">Edges representing the directed graph</param>
        public void WriteGraph<TNode>(IEnumerable<IDirectedGraphEdge<TNode>> edges)
        {
            writer.WriteLine("digraph G {");

            if (rankdir != null)
                writer.WriteLine("\trankdir={0};", rankdir);

            var nodeMap = new Dictionary<TNode, string>();
            int counter = 1;

            foreach (var edge in edges)
            {
                if (!removeSelfLoops || !Equals(edge.Source, edge.Target))
                {
                    string sourceName = GetNodeName(nodeMap, edge.Source, ref counter);
                    string targetName = GetNodeName(nodeMap, edge.Target, ref counter);

                    writer.WriteLine("\t{0} -> {1};", sourceName, targetName);
                }
            }

            WriteNodeNames(nodeMap);

            writer.WriteLine("}");
        }

        private void WriteNodeNames<TNode>(Dictionary<TNode, string> nodeMap)
        {
            foreach (var pair in nodeMap)
            {
                writer.WriteLine("\t{0} [label=\"{1}\"]", pair.Value, pair.Key);
            }
        }

        private string GetNodeName<TNode>(IDictionary<TNode, string> nodeMap, TNode node, ref int counter)
        {
            string name;
            if (!nodeMap.TryGetValue(node, out name))                
            {
                name = String.Format("node{0}", counter++);
                nodeMap.Add(node, name);
            }
            return name;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            writer.Flush();
            writer.Dispose();
        }
    }
}