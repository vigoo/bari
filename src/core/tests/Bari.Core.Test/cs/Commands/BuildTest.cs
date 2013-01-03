using Bari.Core.Commands;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using FluentAssertions;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Commands
{
    [TestFixture]
    public class BuildTest
    {
        private IKernel kernel;
        private Suite suite;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("root")).WhenTargetHas
                <SuiteRootAttribute>();
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("target")).WhenTargetHas
                <TargetRootAttribute>();

            suite = kernel.Get<Suite>();
            suite.Name = "test suite";
        }

        [TearDown]
        public void TearDown()
        {
            kernel.Dispose();
        }

        [Test]
        public void Exists()
        {
            var cmd = kernel.Get<ICommand>("build");
            cmd.Should().NotBeNull();
            cmd.Name.Should().Be("build");
        }

        [Test]
        public void HasHelpAndDescription()
        {
            var cmd = kernel.Get<ICommand>("build");
            cmd.Description.Should().NotBeBlank();
            cmd.Help.Should().NotBeBlank();
        }
    }
}