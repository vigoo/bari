using Bari.Core.Generic;
using Bari.Core.Model;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Model
{
    [TestFixture]
    public class ProjectTest
    {
        [Test]
        public void ProjectInitiallyHasNoSourceSets()
        {
            var project = new Project("testproject", new Module("testmod"));
            project.SourceSets.Should().NotBeNull();
            project.SourceSets.Should().BeEmpty();
        }

        [Test]
        public void ProjectNameCanBeQueried()
        {
            var project = new Project("testproject", new Module("testmod"));
            project.Name.Should().Be("testproject");
        }

        [Test]
        public void GetSourceSetCreatesSetIfMissing()
        {
            var project = new Project("test", new Module("testmod"));
            var set1 = project.GetSourceSet("cs");

            set1.Should().NotBeNull();
            set1.Type.Should().Be("cs");
        }

        [Test]
        public void GetSourceSetReturnsTheSameInstanceIfCalledTwice()
        {
            var project = new Project("test", new Module("testmod"));
            var set1 = project.GetSourceSet("cs");
            var set2 = project.GetSourceSet("cs");

            set1.Should().BeSameAs(set2);
        }

        [Test]
        public void CreatedSourceSetAddedToSourceSetsProperty()
        {
            var project = new Project("test", new Module("testmod"));
            var set1 = project.GetSourceSet("cs");
            var set2 = project.GetSourceSet("cs");

            project.SourceSets.Should().HaveCount(1);
            project.SourceSets.Should().HaveElementAt(0, set1);
        }

        [Test]
        public void HasNonEmptySourceSetMethodWorks()
        {
            var project = new Project("test", new Module("testmod"));
            var set1 = project.GetSourceSet("cs");

            project.HasNonEmptySourceSet("cs").Should().BeFalse();
            project.HasNonEmptySourceSet("vb").Should().BeFalse();

            set1.Add(new SuiteRelativePath("testfile"));

            project.HasNonEmptySourceSet("cs").Should().BeTrue();
            project.HasNonEmptySourceSet("vb").Should().BeFalse();
        }

        [Test]
        public void HasNonEmptySourceSetDoesNotCreateSet()
        {
            var project = new Project("test", new Module("testmod"));
            project.HasNonEmptySourceSet("cs");

            project.SourceSets.Should().BeEmpty();
        }
    }
}