using Bari.Core.Model;
using Bari.Core.Test.Helper;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Model
{
    [TestFixture]
    public class TestProjectTest
    {
        [Test]
        public void RootDirectoryIsSubdirectoryOfModulesTestDirectory()
        {
            var projdir = new TestFileSystemDirectory("test");
            var fs = new TestFileSystemDirectory(
                "root", new TestFileSystemDirectory(
                            "src", new TestFileSystemDirectory(
                                       "testmod", new TestFileSystemDirectory(
                                                      "tests", projdir))));
            var module = new Module("testmod", new Suite(fs));
            var project = new TestProject("test", module);

            project.RootDirectory.Should().Be(projdir);
        }
    }
}