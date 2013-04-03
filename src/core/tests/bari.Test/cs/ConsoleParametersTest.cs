using Bari.Console.UI;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Console.Test
{
    [TestFixture]
    public class ConsoleParametersTest
    {
        [Test]
        public void NoArgs()
        {
            var ps = new ConsoleParameters(new string[0]);
            ps.Command.Should().Be("help"); // default command
            ps.CommandParameters.Should().BeEmpty();
            ps.VerboseOutput.Should().BeFalse();
            ps.Goal.Should().Be("debug"); // default goal
        }

        [Test]
        public void SimpleCommandWithoutArgs()
        {
            var ps = new ConsoleParameters(new[] { "cmd"});
            ps.Command.Should().Be("cmd"); // default command
            ps.CommandParameters.Should().BeEmpty();
            ps.VerboseOutput.Should().BeFalse();
            ps.Goal.Should().Be("debug"); // default goal
        }

        [Test]
        public void CommandWithArgs()
        {
            var ps = new ConsoleParameters(new[] { "cmd", "1", "2", "3" });
            ps.Command.Should().Be("cmd"); // default command
            ps.CommandParameters.Should().BeEquivalentTo(new[] {"1", "2", "3"});
            ps.VerboseOutput.Should().BeFalse();
            ps.Goal.Should().Be("debug"); // default goal
        }

        [Test]
        public void CommandWithArgsAndVerboseSwitch()
        {
            var ps = new ConsoleParameters(new[] { "-v", "cmd", "1", "2", "3" });
            ps.Command.Should().Be("cmd"); // default command
            ps.CommandParameters.Should().BeEquivalentTo(new[] { "1", "2", "3" });
            ps.VerboseOutput.Should().BeTrue();
            ps.Goal.Should().Be("debug"); // default goal
        }

        [Test]
        public void CommandWithArgsAndVerboseSwitchAndGoalSpec1()
        {
            var ps = new ConsoleParameters(new[] { "--target", "goal", "-v", "cmd", "1", "2", "3" });
            ps.Command.Should().Be("cmd"); // default command
            ps.CommandParameters.Should().BeEquivalentTo(new[] { "1", "2", "3" });
            ps.VerboseOutput.Should().BeTrue();
            ps.Goal.Should().Be("goal"); // default goal
        }

        [Test]
        public void CommandWithArgsAndVerboseSwitchAndGoalSpec2()
        {
            var ps = new ConsoleParameters(new[] { "/v", "/target", "goal", "cmd", "1", "2", "3" });
            ps.Command.Should().Be("cmd"); // default command
            ps.CommandParameters.Should().BeEquivalentTo(new[] { "1", "2", "3" });
            ps.VerboseOutput.Should().BeTrue();
            ps.Goal.Should().Be("goal"); // default goal
        }
    }
}