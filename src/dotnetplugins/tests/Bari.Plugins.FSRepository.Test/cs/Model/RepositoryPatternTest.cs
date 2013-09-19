using Bari.Plugins.FSRepository.Model;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bari.Plugins.FSRepository.Test.Model
{
    [TestFixture]
    public class RepositoryPatternTest
    {
        [Test]
        public void EnvironmentVariableSupport()
        {
            var pattern = new RepositoryPattern("$ENV1, $ENV2 and $ENV3");
            var context = new Mock<IPatternResolutionContext>();

            context.Setup(c => c.GetEnvironmentVariable("ENV1")).Returns("e1");
            context.Setup(c => c.GetEnvironmentVariable("ENV2")).Returns("e2");
            context.Setup(c => c.GetEnvironmentVariable("ENV3")).Returns("e3");

            var resolution = pattern.Resolve(context.Object);

            resolution.Should().NotBeNull();
            resolution.Should().Be("e1, e2 and e3");
        }
        
        [Test]
        public void NonExistingEnvironmentVariablesStopsResolution()
        {
            var pattern = new RepositoryPattern("$ENV1, $ENV2 and $ENV3");
            var context = new Mock<IPatternResolutionContext>();

            context.Setup(c => c.GetEnvironmentVariable("ENV1")).Returns("e1");

            var resolution = pattern.Resolve(context.Object);

            resolution.Should().BeNull();
        }

        [Test]
        public void ReferenceVariablesReplaced()
        {
            var pattern = new RepositoryPattern("/root/%NAME/%FILENAME.%VERSION.%EXT");
            var context = new Mock<IPatternResolutionContext>();

            context.SetupGet(c => c.DependencyName).Returns("dependency");
            context.SetupGet(c => c.FileName).Returns("something");
            context.SetupGet(c => c.Extension).Returns("dll");
            context.SetupGet(c => c.Version).Returns("version");

            var resolution = pattern.Resolve(context.Object);

            resolution.Should().NotBeNull();
            resolution.Should().Be("/root/dependency/something.version.dll");
        }

        [Test]
        public void ResolutionFailsIfNoVersionInfo()
        {
            var pattern = new RepositoryPattern("/root/%NAME/%FILENAME.%VERSION.%EXT");
            var context = new Mock<IPatternResolutionContext>();

            context.SetupGet(c => c.DependencyName).Returns("dependency");
            context.SetupGet(c => c.FileName).Returns("something");
            context.SetupGet(c => c.Extension).Returns("dll");
            context.SetupGet(c => c.Version).Returns((string) null);

            var resolution = pattern.Resolve(context.Object);

            resolution.Should().BeNull();
        }

        [Test]
        public void WhenVersionNotNeededItWorks()
        {
            var pattern = new RepositoryPattern("/root/%NAME/%FILENAME.%EXT");
            var context = new Mock<IPatternResolutionContext>();

            context.SetupGet(c => c.DependencyName).Returns("dependency");
            context.SetupGet(c => c.FileName).Returns("something");
            context.SetupGet(c => c.Extension).Returns("dll");
            context.SetupGet(c => c.Version).Returns((string) null);

            var resolution = pattern.Resolve(context.Object);

            resolution.Should().NotBeNull();
            resolution.Should().Be("/root/dependency/something.dll");
        }
    }
}