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
            PathOperations.FindCommonRoot(new[] {@"a\b\c\x.y"}).Should().Be(@"a\b\c\");
        }

        [Test]
        public void SimpleCommonRoot()
        {
            PathOperations.FindCommonRoot(new[] {@"a\1.txt", @"a\2.txt"}).Should().Be(@"a\");
        }

        [Test]
        public void CommonRootWithDifferentSubDirs()
        {
            PathOperations.FindCommonRoot(new[] { @"a\b\1.txt", @"a\c\2.txt" }).Should().Be(@"a\");
            PathOperations.FindCommonRoot(new[] { @"a\b\1.txt", @"a\c\2.txt", @"a\3.txt" }).Should().Be(@"a\");
        }
    }
}