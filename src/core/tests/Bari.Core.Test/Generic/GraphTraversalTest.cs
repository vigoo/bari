using System.Collections.Generic;
using Bari.Core.Generic.Graph;
using NUnit.Framework;

namespace Bari.Core.Test.Generic
{
    [TestFixture]
    public class GraphTraversalTest
    {        
        private SimpleUndirectedGraphNode<int> root;

        [SetUp]
        public void SetUp()
        {
            var n11 = new SimpleUndirectedGraphNode<int>(11);
            var n3 = new SimpleUndirectedGraphNode<int>(
                3,
                new SimpleUndirectedGraphNode<int>(
                    9,
                    n11,
                    new SimpleUndirectedGraphNode<int>(10)));
            var n12 = new SimpleUndirectedGraphNode<int>(12);

            root = new SimpleUndirectedGraphNode<int>(
                1,
                new SimpleUndirectedGraphNode<int>(
                    2,
                    new SimpleUndirectedGraphNode<int>(
                        5,
                        new SimpleUndirectedGraphNode<int>(6),
                        new SimpleUndirectedGraphNode<int>(7),
                        new SimpleUndirectedGraphNode<int>(8)),
                    n12),
                n3,
                new SimpleUndirectedGraphNode<int>(4));

            n3.Add(n12);
            n12.Add(n11);
        }

        [Test]
        public void UndirectedDepthFirstTraversal()
        {
            List<int> result = root.UndirectedDepthFirstTraversal(EnumNodes, new List<int>());

            Assert.IsNotNull(result);
            Assert.AreEqual(12, result.Count);

            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(4, result[1]);
            Assert.AreEqual(3, result[2]);
            Assert.AreEqual(12, result[3]);
            Assert.AreEqual(11, result[4]);
            Assert.AreEqual(9, result[5]);
            Assert.AreEqual(10, result[6]);
            Assert.AreEqual(2, result[7]);
            Assert.AreEqual(5, result[8]);
            Assert.AreEqual(8, result[9]);
            Assert.AreEqual(7, result[10]);
            Assert.AreEqual(6, result[11]);
        }

        [Test]
        public void UndirectedBreadthFirstTraversal()
        {
            List<int> result = root.UndirectedBreadthFirstTraversal(EnumNodes, new List<int>());

            Assert.IsNotNull(result);
            Assert.AreEqual(12, result.Count);

            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
            Assert.AreEqual(4, result[3]);
            Assert.AreEqual(5, result[4]);
            Assert.AreEqual(12, result[5]);
            Assert.AreEqual(9, result[6]);
            Assert.AreEqual(6, result[7]);
            Assert.AreEqual(7, result[8]);
            Assert.AreEqual(8, result[9]);
            Assert.AreEqual(11, result[10]);
            Assert.AreEqual(10, result[11]);
        }

        private static List<int> EnumNodes(int arg1, List<int> arg2)
        {
            arg2.Add(arg1);
            return arg2;
        }
    }
}
