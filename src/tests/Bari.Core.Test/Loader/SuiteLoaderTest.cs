using System.IO;
using Bari.Core.Model;
using FluentAssertions;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test
{
    [TestFixture]
    public class SuiteLoaderTest
    {
        private const string yaml = "---\nsuite: Test suite";

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

            var suite = loader.Load(yaml);
            suite.Should().NotBeNull();
            suite.Name.Should().Be("Test suite");
        }

        [Test]
        public void DetectsAndReadLocalYAMLFileSpec()
        {
            var tempFile = Path.GetTempFileName();
            using (var writer = File.CreateText(tempFile))
            {
                writer.WriteLine(yaml);
            }
            try
            {
                var loader = Kernel.Root.Get<ISuiteLoader>();
                loader.Should().NotBeNull();

                var suite = loader.Load(tempFile);
                suite.Should().NotBeNull();
                suite.Name.Should().Be("Test suite");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}