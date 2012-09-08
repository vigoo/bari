using Bari.Core.Model;
using FluentAssertions;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test
{
    [TestFixture]
    public class SuiteLoaderTest
    {
        [Test]
        public void HasSuiteLoaderImplementation()
        {
            var loader = Kernel.Root.Get<ISuiteLoader>();
            loader.Should().NotBeNull();
        }

        [Test]
        public void DetectsAndReadInMemoryYAMLSpec()
        {
            var loader = Kernel.Root.Get<ISuiteLoader>();
            loader.Should().NotBeNull();

            const string yaml = "---\nsuite: Test suite";

            var suite = loader.Load(yaml);
            suite.Should().NotBeNull();
            suite.Name.Should().Be("Test suite");
        }
    }
}