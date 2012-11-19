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
    }
}