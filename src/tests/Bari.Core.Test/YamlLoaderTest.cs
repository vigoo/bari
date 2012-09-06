using Bari.Core.Model.Loader;
using FluentAssertions;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test
{
    [TestFixture]
    public class YamlLoaderTest
    {
        [Test]
        public void SuiteNameRead()
        {
            const string yaml = @"---                   
suite: Test suite";

            var loader = Kernel.Root.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Name.Should().Be("Test suite");
        } 
    }
}