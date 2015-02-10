using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bari.Core.Test.Build.Dependencies
{
    [TestFixture]
    public class SubtaskDependencyTest
    {
        [Test]
        public void UsesDependentTasksFingerprint()
        {
            var referencedBuilder = new Mock<IBuilder>();
            var referencedDep = new Mock<IDependencies>();
            var fingerprint = new Mock<IDependencyFingerprint>();

            var subtaskDep = new SubtaskDependency(referencedBuilder.Object);

            referencedBuilder.Setup(b => b.Dependencies).Returns(referencedDep.Object);
            referencedDep.SetupGet(d => d.Fingerprint).Returns(fingerprint.Object);

            var fp1 = referencedBuilder.Object.Dependencies.Fingerprint;
            var fp2 = subtaskDep.Fingerprint;

            fp1.Should().Be(fp2);
        }
    }
}