using Bari.Core.Build.Dependencies;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Build.Dependencies
{
    [TestFixture]
    public class NoDependenciesTest
    {
        [Test]
        public void NoDependenciesMakesEqualFingerprints()
        {
            var dep1 = new NoDependencies();
            var dep2 = new NoDependencies();

            var fp1 = dep1.CreateFingerprint();
            var fp2 = dep2.CreateFingerprint();

            fp1.Should().Be(fp2);
            fp2.Should().Be(fp1);
            fp1.Equals(fp2).Should().BeTrue();
        }

        [Test]
        public void HasNoProtocol()
        {
            var dep = new NoDependencies();
            var fp1 = dep.CreateFingerprint();

            var proto = fp1.Protocol;
            proto.Should().BeNull();
        }
    }
}