using Bari.Core.Commands;
using FluentAssertions;
using Ninject;
using NUnit.Framework;

namespace Bari.Core.Test.Commands
{
    [TestFixture]
    public class CommandEnumeratorTest
    {
        private IKernel kernel;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCommandFactory(kernel);

            var command1 = new DummyCommand {Name = "cmd1", NeedsExplicitTargetGoal = true};
            var command2 = new DummyCommand {Name = "cmd2", NeedsExplicitTargetGoal = false};

            kernel.Bind<ICommand>().ToConstant(command1).Named("cmd1");
            kernel.Bind<ICommand>().ToConstant(command2).Named("cmd2");
        }

        [TearDown]
        public void TearDown()
        {
            kernel.Dispose();
        }

        [Test]
        public void NeedsExplicitTargetGoalAsksTheCommand()
        {
            var enumerator = kernel.Get<ICommandEnumerator>();
            enumerator.NeedsExplicitTargetGoal("cmd1").Should().BeTrue();
            enumerator.NeedsExplicitTargetGoal("cmd2").Should().BeFalse();
        }

        [Test]
        public void ExplicitTargetGoalNeededByDefault()
        {
            var enumerator = kernel.Get<ICommandEnumerator>();
            enumerator.NeedsExplicitTargetGoal("unknown").Should().BeTrue();
        }
    }
}