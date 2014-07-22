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
        [ExpectedException(typeof(InvalidCommandParameterException))]
        public void ThrowsExceptionForMoreParameters()
        {
            new CleanParameters(new[] {"-test1", "-test2"});
        }
    }
}