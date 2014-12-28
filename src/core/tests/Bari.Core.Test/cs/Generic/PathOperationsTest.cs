using System.IO;
using Bari.Core.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Generic
{
    [TestFixture]
    public class PathOperationsTest
    {
        [Test]
        public void EmptySet()
        {
            PathOperations.FindCommonRoot(new string[0]).Should().BeEmpty();
        }

        [Test]
        public void SingleFile()
        {
            PathOperations.FindCommonRoot(new[] { Path.Combine("a", "b", "c", "x.y") })
                .Should().Be(Path.Combine("a", "b", "c") + Path.DirectorySeparatorChar);
        }

        [Test]
        public void SimpleCommonRoot()
        {
            PathOperations.FindCommonRoot(new[] 
                {
                    Path.Combine("a", "1.txt"), 
                    Path.Combine("a", "2.txt")
                })
                .Should().Be("a" + Path.DirectorySeparatorChar);
        }

        [Test]
        public void CommonRootWithDifferentSubDirs()
        {
            PathOperations.FindCommonRoot(new[] { Path.Combine("a", "b", "1.txt"), Path.Combine("a", "c", "2.txt") }).Should().Be("a" + Path.DirectorySeparatorChar);
            PathOperations.FindCommonRoot(new[] { Path.Combine("a", "b", "1.txt"), Path.Combine("a", "c", "2.txt"), Path.Combine("a", "3.txt") }).Should().Be("a" + Path.DirectorySeparatorChar);
        }
    }
}