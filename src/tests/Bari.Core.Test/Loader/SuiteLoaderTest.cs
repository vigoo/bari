using System.IO;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using FluentAssertions;
using NUnit.Framework;
using Ninject;
using Ninject.Extensions.ChildKernel;

namespace Bari.Core.Test
{
    [TestFixture]
    public class SuiteLoaderTest
    {
        private const string yaml = "---\nsuite: Test suite";
        private IKernel kernel;

        [SetUp]
        public void Setup()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("root")).WhenTargetHas
                <SuiteRootAttribute>();
        }

        [Test]
        public void HasSuiteLoaderImplementation()
        {
            var loader = kernel.Get<ISuiteLoader>();
            loader.Should().NotBeNull();
        }

        [Test]
        public void DetectsAndReadInMemoryYAMLSpec()
        {
            var loader = kernel.Get<ISuiteLoader>();
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
                var loader = kernel.Get<ISuiteLoader>();
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