using Bari.Core.Model;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Model
{
    [TestFixture]
    public class SuiteTest
    {
        [Test]
        public void SuiteNameCanBeModified()
        {
            var suite = new Suite {Name = "test"};
            suite.Name.Should().Be("test");
            suite.Name = "test2";
            suite.Name.Should().Be("test2");
        }

        [Test]
        public void SuiteHasNoModulesInitially()
        {
            var suite = new Suite();
            suite.Modules.Should().BeEmpty();
        }

        [Test]
        public void GetModuleCreatesInstanceIfMissing()
        {
            var suite = new Suite();
            var mod1 = suite.GetModule("mod");

            mod1.Should().NotBeNull();
            mod1.Name.Should().Be("mod");
        }

        [Test]
        public void GetProjectReturnsTheSameInstanceIfCalledTwice()
        {
            var suite = new Suite();
            var mod1 = suite.GetModule("mod");
            var mod2 = suite.GetModule("mod");

            mod1.Should().BeSameAs(mod2);
        } 
    }
}