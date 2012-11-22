using System.Collections.Generic;
using System.Linq;
using Bari.Core.Generic.Graph;
using NUnit.Framework;

namespace Bari.Core.Test.Generic
{
    [TestFixture]
    public class NodeBuilderTest
    {
        [Test]
        public void Undirected1()
        {
            var edges = CreateUndirectedEdges1();
            TestUndirected1(edges);
        }

        private static IEnumerable<IUndirectedGraphEdge<int>> CreateUndirectedEdges1()
        {
            return new[]
                       {
                           new SimpleUndirectedGraphEdge<int>(1, 2),
                           new SimpleUndirectedGraphEdge<int>(1, 4),
                           new SimpleUndirectedGraphEdge<int>(3, 1),
                           new SimpleUndirectedGraphEdge<int>(3, 4),
                           new SimpleUndirectedGraphEdge<int>(2, 3)
                       };
        }

        private static void TestUndirected1(IEnumerable<IUndirectedGraphEdge<int>> edges)
        {
            var n1 = edges.BuildNodes(1);
            Assert.AreEqual(1, n1.Data);
            Assert.AreEqual(3, n1.AdjacentNodes.Count());
            Assert.IsTrue(n1.AdjacentNodes.Count(n => n.Data == 2) == 1);
            Assert.IsTrue(n1.AdjacentNodes.Count(n => n.Data == 3) == 1);
            Assert.IsTrue(n1.AdjacentNodes.Count(n => n.Data == 4) == 1);

            var n2 = edges.BuildNodes(2);
            Assert.AreEqual(2, n2.Data);
            Assert.AreEqual(2, n2.AdjacentNodes.Count());
            Assert.IsTrue(n2.AdjacentNodes.Count(n => n.Data == 1) == 1);
            Assert.IsTrue(n2.AdjacentNodes.Count(n => n.Data == 3) == 1);

            var n3 = edges.BuildNodes(3);
            Assert.AreEqual(3, n3.Data);
            Assert.AreEqual(3, n3.AdjacentNodes.Count());
            Assert.IsTrue(n3.AdjacentNodes.Count(n => n.Data == 1) == 1);
            Assert.IsTrue(n3.AdjacentNodes.Count(n => n.Data == 2) == 1);
            Assert.IsTrue(n3.AdjacentNodes.Count(n => n.Data == 4) == 1);
        }

        [Test]
        public void Directed1()
        {
            var edges = CreateDirectedEdges1();
            TestDirected1(edges);
        }

        private static IEnumerable<IDirectedGraphEdge<int>> CreateDirectedEdges1()
        {
            return new[]
                       {
                           new SimpleDirectedGraphEdge<int>(1, 2),
                           new SimpleDirectedGraphEdge<int>(1, 4),
                           new SimpleDirectedGraphEdge<int>(3, 1),
                           new SimpleDirectedGraphEdge<int>(3, 4),
                           new SimpleDirectedGraphEdge<int>(2, 3)
                       };
        }

        private static void TestDirected1(IEnumerable<IDirectedGraphEdge<int>> edges)
        {
            var n1 = edges.BuildNodes(1);
            Assert.AreEqual(1, n1.Data);
            Assert.AreEqual(3, n1.AdjacentNodes.Count());
            Assert.IsTrue(n1.AdjacentNodes.Count(n => n.Data == 2) == 1);
            Assert.IsTrue(n1.AdjacentNodes.Count(n => n.Data == 3) == 1);
            Assert.IsTrue(n1.AdjacentNodes.Count(n => n.Data == 4) == 1);

            var n2 = edges.BuildNodes(2);
            Assert.AreEqual(2, n2.Data);
            Assert.AreEqual(2, n2.AdjacentNodes.Count());
            Assert.IsTrue(n2.AdjacentNodes.Count(n => n.Data == 1) == 1);
            Assert.IsTrue(n2.AdjacentNodes.Count(n => n.Data == 3) == 1);

            var n3 = edges.BuildNodes(3);
            Assert.AreEqual(3, n3.Data);
            Assert.AreEqual(3, n3.AdjacentNodes.Count());
            Assert.IsTrue(n3.AdjacentNodes.Count(n => n.Data == 1) == 1);
            Assert.IsTrue(n3.AdjacentNodes.Count(n => n.Data == 2) == 1);
            Assert.IsTrue(n3.AdjacentNodes.Count(n => n.Data == 4) == 1);
        }
        
    }
}
