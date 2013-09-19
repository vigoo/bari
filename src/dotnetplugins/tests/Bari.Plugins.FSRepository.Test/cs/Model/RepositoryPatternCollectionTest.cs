using Bari.Plugins.FSRepository.Model;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bari.Plugins.FSRepository.Test.Model
{
    [TestFixture]
    public class RepositoryPatternCollectionTest
    {
        [Test]
        public void FirstMatchChoosen()
        {
            var access = new Mock<IFileSystemRepositoryAccess>();
            var context = CreateContext();

            access.Setup(a => a.Exists(It.IsAny<string>())).Returns(true);

            var patterns = new RepositoryPatternCollection(access.Object);
            patterns.AddPattern(new RepositoryPattern("pattern1"));
            patterns.AddPattern(new RepositoryPattern("pattern2"));
            patterns.AddPattern(new RepositoryPattern("pattern3"));

            var resolution = patterns.Resolve(context.Object);

            resolution.Should().Be("pattern1");
        }

        [Test]
        public void MatchUsesFileSystemChecks()
        {
            var access = new Mock<IFileSystemRepositoryAccess>();
            var context = CreateContext();

            access.Setup(a => a.Exists("pattern1")).Returns(false);
            access.Setup(a => a.Exists("pattern2")).Returns(true);
            access.Setup(a => a.Exists("pattern3")).Returns(false);

            var patterns = new RepositoryPatternCollection(access.Object);
            patterns.AddPattern(new RepositoryPattern("pattern1"));
            patterns.AddPattern(new RepositoryPattern("pattern2"));
            patterns.AddPattern(new RepositoryPattern("pattern3"));

            var resolution = patterns.Resolve(context.Object);

            resolution.Should().Be("pattern2");
        }

        [Test]
        public void NoMatchBecauseOfFileSystem()
        {
            var access = new Mock<IFileSystemRepositoryAccess>();
            var context = CreateContext();

            var patterns = new RepositoryPatternCollection(access.Object);
            patterns.AddPattern(new RepositoryPattern("pattern1"));
            patterns.AddPattern(new RepositoryPattern("pattern2"));
            patterns.AddPattern(new RepositoryPattern("pattern3"));

            var resolution = patterns.Resolve(context.Object);

            resolution.Should().BeNull();
        }

        [Test]
        public void NoMatchBecauseOfPatternResolution()
        {
            var access = new Mock<IFileSystemRepositoryAccess>();
            var context = CreateContext();

            access.Setup(a => a.Exists(It.IsAny<string>())).Returns(true);

            var patterns = new RepositoryPatternCollection(access.Object);
            patterns.AddPattern(new RepositoryPattern("pattern1.%VERSION"));
            patterns.AddPattern(new RepositoryPattern("pattern2.%VERSION"));
            patterns.AddPattern(new RepositoryPattern("pattern3.%VERSION"));

            var resolution = patterns.Resolve(context.Object);

            resolution.Should().BeNull();
        }

        private Mock<IPatternResolutionContext> CreateContext()
        {
            var context = new Mock<IPatternResolutionContext>();

            context.SetupGet(c => c.DependencyName).Returns("dependency");
            context.SetupGet(c => c.FileName).Returns("something");
            context.SetupGet(c => c.Extension).Returns("dll");
            context.SetupGet(c => c.Version).Returns((string)null);

            return context;
        }
    }
}