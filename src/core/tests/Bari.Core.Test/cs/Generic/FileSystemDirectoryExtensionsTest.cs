using Bari.Core.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.IO;

namespace Bari.Core.Test.Generic
{
    [TestFixture]
    public class FileSystemDirectoryExtensionsTest
    {
        [Test]
        public void GetRelativePathFromTest()
        {
            // r 
            // -> a
            //   -> c.txt
            // -> a.test
            //   -> e.txt
            // -> b
            //   -> d.txt

            var r = new Mock<IFileSystemDirectory>();
            var a = new Mock<IFileSystemDirectory>();
            var atest = new Mock<IFileSystemDirectory>();
            var b = new Mock<IFileSystemDirectory>();

            r.Setup(dir => dir.GetRelativePath(a.Object)).Returns(@"a");
            r.Setup(dir => dir.GetRelativePath(atest.Object)).Returns(@"a.test");
            r.Setup(dir => dir.GetRelativePath(b.Object)).Returns(@"b");

            r.Object.GetRelativePathFrom(a.Object, Path.Combine("a", "c.txt")).Should().Be(@"c.txt");
            r.Object.GetRelativePathFrom(a.Object, Path.Combine("a.test", "e.txt")).Should().Be(Path.Combine("..", "a.test", "e.txt"));
            r.Object.GetRelativePathFrom(a.Object, Path.Combine("b", "d.txt")).Should().Be(Path.Combine("..", "b", "d.txt"));
        }
    }
}