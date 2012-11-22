using System;
using Bari.Core.Model;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Model
{
    [TestFixture]
    public class ReferenceTest
    {
        [Test]
        public void UriCanBeReadBack()
        {
            var reference = new Reference(new Uri("test://random.uri"));
            reference.Uri.ToString().Should().Be("test://random.uri/");
        }

        [Test]
        public void EqualityTest()
        {
            var ref1 = new Reference(new Uri("test://random.uri"));
            var ref2 = new Reference(new Uri("test://random.uri"));

            ref1.Equals(ref2).Should().BeTrue();
            (ref1 == ref2).Should().BeTrue();
            (ref1 != ref2).Should().BeFalse();
            ref1.GetHashCode().Should().Be(ref2.GetHashCode());
        }

        [Test]
        public void InequalityTest()
        {
            var ref1 = new Reference(new Uri("test://random.uri"));
            var ref2 = new Reference(new Uri("test://other.uri"));

            ref1.Equals(ref2).Should().BeFalse();
            (ref1 == ref2).Should().BeFalse();
            (ref1 != ref2).Should().BeTrue();
            ref1.GetHashCode().Should().NotBe(ref2.GetHashCode());
        }
    }
}