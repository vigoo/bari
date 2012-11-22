using Bari.Core.Model;
using Bari.Core.Test.Helper;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Model
{
    [TestFixture]
    public class ModuleTest
    {
        [Test]
        public void ModuleNameCanBeQueried()
        {
            var module = new Module("testmodule", new TestFileSystemDirectory("module"));
            module.Name.Should().Be("testmodule");
        }

        [Test]
        public void ModuleHasNoProjectsInitially()
        {
            var module = new Module("test", new TestFileSystemDirectory("module"));
            module.Projects.Should().BeEmpty();
        }

        [Test]
        public void GetProjectCreatesInstanceIfMissing()
        {
            var module = new Module("test", new TestFileSystemDirectory("module"));
            var proj1 = module.GetProject("proj1");

            proj1.Should().NotBeNull();
            proj1.Name.Should().Be("proj1");
        }

        [Test]
        public void GetProjectReturnsTheSameInstanceIfCalledTwice()
        {
            var module = new Module("test", new TestFileSystemDirectory("module"));
            var proj1 = module.GetProject("proj");
            var proj2 = module.GetProject("proj");

            proj1.Should().BeSameAs(proj2);
        }

        [Test]
        public void ModuleHasNoTestProjectsInitially()
        {
            var module = new Module("test", new TestFileSystemDirectory("module"));
            module.TestProjects.Should().BeEmpty();
        }

        [Test]
        public void GetTestProjectCreatesInstanceIfMissing()
        {
            var module = new Module("test", new TestFileSystemDirectory("module"));
            var proj1 = module.GetTestProject("proj1");

            proj1.Should().NotBeNull();
            proj1.Name.Should().Be("proj1");
        }

        [Test]
        public void GetTestProjectReturnsTheSameInstanceIfCalledTwice()
        {
            var module = new Module("test", new TestFileSystemDirectory("module"));
            var proj1 = module.GetTestProject("proj");
            var proj2 = module.GetTestProject("proj");

            proj1.Should().BeSameAs(proj2);
        }

        [Test]
        public void HasProjectWorksCorrectly()
        {
            var module = new Module("test", new TestFileSystemDirectory("module"));

            module.HasProject("proj1").Should().BeFalse();
            module.GetProject("proj1");
            module.HasProject("proj1").Should().BeTrue();
        }

        [Test]
        public void HasTestProjectWorksCorrectly()
        {
            var module = new Module("test", new TestFileSystemDirectory("module"));

            module.HasTestProject("proj1").Should().BeFalse();
            module.GetTestProject("proj1");
            module.HasTestProject("proj1").Should().BeTrue();
        }
        
        [Test]
        public void ModuleRootIsChildOfSuiteRoot()
        {
            var fs = new TestFileSystemDirectory(
                "root", new TestFileSystemDirectory(
                            "src", new TestFileSystemDirectory("test")));
            var module = new Module("test", fs);

            module.RootDirectory.Should().Be(
                fs.GetChildDirectory("src").GetChildDirectory("test"));
        }
    }
}