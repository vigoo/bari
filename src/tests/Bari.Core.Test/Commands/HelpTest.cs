using Bari.Core.Commands;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Core.UI;
using FluentAssertions;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Commands
{
    [TestFixture]
    public class HelpTest
    {
        private IKernel kernel;
        private TestUserOutput output;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            output = new TestUserOutput();
            kernel.Bind<IUserOutput>().ToConstant(output).InSingletonScope();
        }
    
        [Test]
        public void Exists()
        {
            var cmd = kernel.Get<ICommand>("help");
            cmd.Should().NotBeNull();
            cmd.Name.Should().Be("help");
        }

        [Test]
        public void HasHelpAndDescription()
        {
            var cmd = kernel.Get<ICommand>("help");
            cmd.Description.Should().NotBeBlank();
            cmd.Help.Should().NotBeBlank();
        }

        [Test]
        [ExpectedException(typeof(InvalidCommandParameterException))]
        public void HelpCalledWithMoreThanOneParametersThrowException()
        {
            var cmd = kernel.Get<ICommand>("help");
            cmd.Run(kernel.Get<Suite>(), new[] {"test1", "test2"});
        }

        [Test]
        [ExpectedException(typeof(InvalidCommandParameterException))]
        public void UnknownCommandNameInParameterThrowsException()
        {
            var cmd = kernel.Get<ICommand>("help");
            cmd.Run(kernel.Get<Suite>(), new[] { "non-existing-command" });
        }

        [Test]
        public void CalledWithoutArgumentPrintsDescriptions()
        {
            var cmd = kernel.Get<ICommand>("help");
            cmd.Run(kernel.Get<Suite>(), new string[0]);

            output.Messages.Should().NotBeEmpty();
            output.Descriptions.Should().NotBeEmpty();
            output.Descriptions.Should().Contain(t => t.Item1 == "help");
        }

        [Test]
        public void CalledWithOneArgumentPrintsHelpString()
        {
            var cmd = kernel.Get<ICommand>("help");
            cmd.Run(kernel.Get<Suite>(), new[] {"help"});

            string.Join("\n", output.Messages).Should().Be(cmd.Help);
        }

        [Test]
        public void SupportsAnyRegisteredCommand()
        {
            var cmd = kernel.Get<ICommand>("help");

            var dummy = new DummyCommand
                            {
                                Name = "dummy",
                                Description = "dummy description",
                                Help = "dummy help"
                            };
            kernel.Bind<ICommand>().ToConstant(dummy).Named("dummy");

            cmd.Run(kernel.Get<Suite>(), new string[0]);
            output.Messages.Should().NotBeEmpty();
            output.Descriptions.Should().NotBeEmpty();
            output.Descriptions.Should().Contain(t => t.Item1 == "dummy" && t.Item2 == "dummy description");

            output.Reset();
            cmd.Run(kernel.Get<Suite>(), new[] { "dummy" });

            output.Messages.Should().HaveCount(1);
            output.Messages.Should().HaveElementAt(0, "dummy help");
            output.Descriptions.Should().BeEmpty();
        }
    }
}