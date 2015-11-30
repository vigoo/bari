using Bari.Core.Commands;
using Bari.Core.Commands.Clean;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Core.UI;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Commands
{
    [TestFixture]
    public class CleanTest
    {
        private IKernel kernel;
        private Suite suite;
        private TestFileSystemDirectory target;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("root")).WhenTargetHas
                <SuiteRootAttribute>();

            target = new TestFileSystemDirectory("target");
            kernel.Bind<IFileSystemDirectory>().ToConstant(target).WhenTargetHas
                <TargetRootAttribute>();

            kernel.Bind<IUserOutput>().To<TestUserOutput>();

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
            var cmd = kernel.Get<ICommand>("clean");
            cmd.Should().NotBeNull();
            cmd.Name.Should().Be("clean");
        }

        [Test]
        public void HasHelpAndDescription()
        {
            var cmd = kernel.Get<ICommand>("clean");
            cmd.Description.Should().NotBeNullOrWhiteSpace();
            cmd.Help.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void CalledWithMoreParametersThrowException()
        {
            var cmd = kernel.Get<ICommand>("clean");
            Assert.That(cmd.Run(kernel.Get<Suite>(), new[] { "test2", "-test3", "something" }), Throws.TypeOf<InvalidCommandParameterException>());
        }

        [Test]
        public void DeletesTargetDirectory()
        {
            target.IsDeleted.Should().BeFalse();

            var cmd = kernel.Get<ICommand>("clean");
            cmd.Run(suite, new string[0]);

            target.IsDeleted.Should().BeTrue();
        }

        [Test]
        public void CallsRegisteredCleaners()
        {
            var c1 = new Mock<ICleanExtension>();
            var c2 = new Mock<ICleanExtension>();

            kernel.Bind<ICleanExtension>().ToConstant(c1.Object);
            kernel.Bind<ICleanExtension>().ToConstant(c2.Object);

            var cmd = kernel.Get<ICommand>("clean");
            cmd.Run(suite, new string[0]);

            c1.Verify(c => c.Clean(It.IsNotNull<ICleanParameters>()), Times.Once());
            c2.Verify(c => c.Clean(It.IsNotNull<ICleanParameters>()), Times.Once());
        }
    }
}