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
    public class TestTest
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
            var cmd = kernel.Get<ICommand>("test");
            cmd.Should().NotBeNull();
            cmd.Name.Should().Be("test");
        }

        [Test]
        public void HasHelpAndDescription()
        {
            var cmd = kernel.Get<ICommand>("test");
            cmd.Description.Should().NotBeNullOrWhiteSpace();
            cmd.Help.Should().NotBeNullOrWhiteSpace();
        }
    }
}