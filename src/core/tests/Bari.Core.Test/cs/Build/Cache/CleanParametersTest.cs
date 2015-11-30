using Bari.Core.Commands.Clean;
using Bari.Core.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Build.Cache
{
    [TestFixture]
    public class CleanParametersTest
    {
        [Test]
        public void ByDefaultDoesNotKeepReferences()
        {
            var parameters = new CleanParameters(new string[0]);
            parameters.KeepReferences.Should().BeFalse();
        }

        [Test]
        public void ByDefaultNoSoftClean()
        {
            var parameters = new CleanParameters(new string[0]);
            parameters.SoftClean.Should().BeFalse();
        }

        [Test]
        public void AcceptsKeepReferencesParameter()
        {
            var parameters = new CleanParameters(new[] {"--keep-references"});
            parameters.KeepReferences.Should().BeTrue();
        }

        [Test]
        public void AcceptsAlternativeKeepReferencesParameter()
        {
            var parameters = new CleanParameters(new[] { "--keep-refs" });
            parameters.KeepReferences.Should().BeTrue();
        }

        [Test]
        public void AcceptsSoftCleanParameter()
        {
            var parameters = new CleanParameters(new[] {"--soft-clean"});
            parameters.SoftClean.Should().BeTrue();
        }

        [Test]
        public void AcceptsBothParametersInAnyOrder()
        {
            var parameters1 = new CleanParameters(new[] { "--soft-clean", "--keep-refs" });
            parameters1.SoftClean.Should().BeTrue();
            parameters1.KeepReferences.Should().BeTrue();

            var parameters2 = new CleanParameters(new[] { "--keep-references", "--soft-clean" });
            parameters2.SoftClean.Should().BeTrue();
            parameters2.KeepReferences.Should().BeTrue();
        }

        [Test]
        public void ThrowsExceptionForMoreParameters()
        {
            Assert.That(new CleanParameters(new[] {"-test1", "-test2", "-test3"}), Throws.TypeOf<InvalidCommandParameterException>());
        }
    }
}