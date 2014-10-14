using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Plugins.VsCore.VisualStudio.SolutionName;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Bari.Plugins.Csharp.Test.VisualStudio
{
    [TestFixture]
    public class ReadableSlnNameGeneratorTest
    {
        private ReadableSlnNameGenerator generator;
        private Mock<ISlnNameGenerator> fallback;
        private ISuiteContentsAnalyzer suiteContents;
        private Suite suite;
        private Module mod1;
        private Module mod2;
        private Module mod3;
        private Project proj11;
        private TestProject tproj11;
        private Project proj21;
        private Project proj22;
        private Project proj31;
        private Product prod1;
        private Product prod2;
        private Project proj41;
        private Module mod4;
        private Project proj51;
        private Module mod5;
        private Project proj61;
        private Module mod6;
        private Project proj71;
        private Module mod7;
        private Module mod8;
        private Project proj81;
        private TestProject tproj81;

        [SetUp]
        public void SetUp()
        {
            fallback = new Mock<ISlnNameGenerator>();                        
            suite = new Suite(new TestFileSystemDirectory("root"));
            suiteContents = new DefaultSuiteContentsAnalyzer(suite);
            generator = new ReadableSlnNameGenerator(fallback.Object, suiteContents);

            fallback.Setup(f => f.GetName(It.IsAny<IEnumerable<Project>>())).Returns("fallback");

            // mod1: proj11, testproj11
            // mod2: proj21, proj22
            // mod3: proj31        
            // mod4: proj41
            // mod5: proj51
            // mod6: proj61
            // mod7: proj71

            // prod1: mod1, mod2
            // prod2: mod2, mod3

            mod1 = suite.GetModule("mod1");
            mod2 = suite.GetModule("mod2");
            mod3 = suite.GetModule("mod3");
            mod4 = suite.GetModule("mod4");
            mod5 = suite.GetModule("mod5");
            mod6 = suite.GetModule("mod6");
            mod7 = suite.GetModule("mod7");
            mod8 = suite.GetModule("mod8");

            proj11 = mod1.GetProject("proj1");
            tproj11 = mod1.GetTestProject("test1");
            proj21 = mod2.GetProject("proj21");
            proj22 = mod2.GetProject("proj22");
            proj31 = mod3.GetProject("proj3");
            proj41 = mod4.GetProject("proj4");
            proj51 = mod5.GetProject("proj5");
            proj61 = mod6.GetProject("proj6");
            proj71 = mod7.GetProject("proj7");
            proj81 = mod8.GetProject("proj8");
            tproj81 = mod8.GetTestProject("test8");

            prod1 = suite.GetProduct("prod1");
            prod1.AddModule(mod1);
            prod1.AddModule(mod2);

            prod2 = suite.GetProduct("prod2");
            prod2.AddModule(mod2);
            prod2.AddModule(mod3);
        }

        // Cases: 
        //  - No partial module matches
        //      - All has tests
        //          - Matching a product
        //          - Does not match a product
        //            - Less than 'max module count'
        //            - More than 'max module count' => fallback
        //      - None has tests
        //          - Matching a product
        //          - Does not match a product
        //            - Less than 'max module count'
        //            - More than 'max module count' => fallback
        //      - Some has tests => fallback
        //  - Partial module matches => fallback

        [Test]
        public void NoPartial_NoneHasTest_MatchProduct()
        {
            var name = generator.GetName(new[] {proj21, proj22, proj31});
            name.Should().Be("prod2");
        }

        [Test]
        public void NoPartial_NoneHasTest_NotMatchingProduct_LessThanMax()
        {
            var name = generator.GetName(new[] { proj31, proj41 });
            name.Should().Be("mod3_mod4");
        }

        [Test]
        public void NoPartial_NoneHasTest_NotMatchingProduct_MoreThanMax()
        {
            var name = generator.GetName(new[] { proj31, proj41, proj51, proj61, proj71 });
            name.Should().Be("fallback");
        }

        [Test]
        public void NoPartial_SomeHasTests()
        {
            var name = generator.GetName(new[] {proj11, tproj11, proj81});
            name.Should().Be("fallback");
        }

        [Test]
        public void Partial()
        {
            var name = generator.GetName(new[] {proj11, proj21, proj31});
            name.Should().Be("fallback");
        }

        [Test]
        public void NoPartial_AllHasTest_MatchProduct()
        {
            var name = generator.GetName(new[] { proj21, proj22, proj11, tproj11 });
            name.Should().Be("prod1-withtests");
        }

        [Test]
        public void NoPartial_AllHasTest_NotMatchingProduct_LessThanMax()
        {
            var name = generator.GetName(new[] { proj81, tproj81, proj11, tproj11, proj41 });
            name.Should().Be("mod1_mod4_mod8-withtests");
        }

        [Test]
        public void NoPartial_AllHasTest_NotMatchingProduct_MoreThanMax()
        {
            var name = generator.GetName(new[] { proj31, proj41, proj51, proj61, proj71, proj11, tproj11, proj81, tproj81 });
            name.Should().Be("fallback");
        }

        [Test]
        public void NoMatch()
        {
            var s = new Suite(new TestFileSystemDirectory("z"));
            var name =
                generator.GetName(new[]
                {
                    new Project("x", new Module("y", s)),
                    new Project("xx", new Module("y", s))
                });
            name.Should().Be("fallback");
        }

        [Test]
        public void Issue90_SingleProjectProduct()
        {
            var s = new Suite(new TestFileSystemDirectory("z"));
            var m = s.GetModule("m");
            var p = m.GetProject("p");
            var prod = s.GetProduct("prod");
            prod.AddModule(m);

            var sc = new DefaultSuiteContentsAnalyzer(s);
            var g = new ReadableSlnNameGenerator(fallback.Object, sc);
            var name = g.GetName(new[] { p });
            name.Should().Be("prod");
        }
    }
}