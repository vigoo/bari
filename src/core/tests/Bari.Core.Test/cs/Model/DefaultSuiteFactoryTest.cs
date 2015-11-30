using System.Collections.Generic;
using Bari.Core.Commands;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Core.UI;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bari.Core.Test.Model
{
    [TestFixture]
    public class DefaultSuiteFactoryTest
    {
        private IFileSystemDirectory suiteRoot;
        private Mock<IParameters> parameters;
        private Mock<ICommandEnumerator> commandEnumerator;
            
        [SetUp]
        public void SetUp()
        {
            suiteRoot = new TestFileSystemDirectory("root");
            parameters = new Mock<IParameters>();
            commandEnumerator = new Mock<ICommandEnumerator>();
        }

        [Test]
        public void AddsDefaultGoalsIfNoCustomGoalsPresent()
        {
            var factory = new DefaultSuiteFactory(parameters.Object, suiteRoot, commandEnumerator.Object);
            var suite = factory.CreateSuite(new HashSet<Goal>(), Suite.DebugGoal);

            suite.Should().NotBeNull();
            suite.Goals.Should().HaveCount(2);
            suite.Goals.Should().Contain(new[] {Suite.DebugGoal, Suite.ReleaseGoal});
        }

        [Test]
        public void UsesCustomGoalsIfSpecified()
        {
            var factory = new DefaultSuiteFactory(parameters.Object, suiteRoot, commandEnumerator.Object);

            var goal1 = new Goal("goal1");
            var goal2 = new Goal("goal2");
            var goal3 = new Goal("goal3");
            var suite = factory.CreateSuite(new HashSet<Goal>(new[] { goal1, goal2, goal3 }), goal1);

            suite.Should().NotBeNull();
            suite.Goals.Should().HaveCount(3);
            suite.Goals.Should().Contain(new[] { goal1, goal2, goal3 });
        }

        [Test]
        public void UsesDefaultGoalIfTargetNotSpecified()
        {
            var factory = new DefaultSuiteFactory(parameters.Object, suiteRoot, commandEnumerator.Object);

            var goal1 = new Goal("goal1");
            var goal2 = new Goal("goal2");
            var goal3 = new Goal("goal3");
            var suite = factory.CreateSuite(new HashSet<Goal>(new[] { goal1, goal2, goal3 }), goal2);

            suite.Should().NotBeNull();
            suite.ActiveGoal.Should().Be(goal2);
        }

        [Test]
        public void RequiresExplicitActiveGoalIfCommandNeedsIt()
        {
            commandEnumerator.Setup(c => c.NeedsExplicitTargetGoal(It.IsAny<string>())).Returns(true);

            var factory = new DefaultSuiteFactory(parameters.Object, suiteRoot, commandEnumerator.Object);

            var goal1 = new Goal("goal1");
            Assert.That(factory.CreateSuite(new HashSet<Goal>(new[] { goal1 }), Suite.DebugGoal), Throws.TypeOf<InvalidGoalException>());
        }

        [Test]
        public void UsesFirstAvailableAsActiveGoalIfCommandDoesNotNeedIt()
        {
            commandEnumerator.Setup(c => c.NeedsExplicitTargetGoal(It.IsAny<string>())).Returns(false);

            var factory = new DefaultSuiteFactory(parameters.Object, suiteRoot, commandEnumerator.Object);

            var goal1 = new Goal("goal1");
            var suite = factory.CreateSuite(new HashSet<Goal>(new[] { goal1 }), Suite.DebugGoal);

            suite.ActiveGoal.Should().Be(goal1);
        }

        [Test]
        public void UsesExplicitTargetGoalAsActiveGoalIfCommandNeedsIt()
        {
            commandEnumerator.Setup(c => c.NeedsExplicitTargetGoal(It.IsAny<string>())).Returns(true);
            UsesExplicitTargetGoalAsActive();
        }

        [Test]
        public void UsesExplicitTargetGoalAsActiveGoalIfCommandDoesNotNeedIt()
        {
            commandEnumerator.Setup(c => c.NeedsExplicitTargetGoal(It.IsAny<string>())).Returns(false);
            UsesExplicitTargetGoalAsActive();
        }

        private void UsesExplicitTargetGoalAsActive()
        {
            parameters.SetupGet(p => p.Goal).Returns("goal2");

            var factory = new DefaultSuiteFactory(parameters.Object, suiteRoot, commandEnumerator.Object);

            var goal1 = new Goal("goal1");
            var goal2 = new Goal("goal2");
            var suite = factory.CreateSuite(new HashSet<Goal>(new[] { goal1, goal2 }), Suite.DebugGoal);

            suite.ActiveGoal.Should().Be(goal2);
        }
    }
}