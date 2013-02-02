using System.Linq;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Plugins.Csharp.VisualStudio.SolutionName;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Plugins.Csharp.Test.VisualStudio
{
    [TestFixture]
    class SlnContentsAnalyzerTest
    {
        private Suite suite;

        [SetUp]
        public void SetUp()
        {
            suite = new Suite(new TestFileSystemDirectory("root"));
        }

        [Test]
        public void ModulesCoveredByEmptyProjectSet()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var matches = a.GetCoveredModules(new Project[0]);

            matches.Should().BeEmpty();
        }

        [Test]
        public void ModulesCoveredByProjectsBelongingToOtherSutie()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var mod1 = new Module("mod1", new Suite(new TestFileSystemDirectory("other")));
            var matches = a.GetCoveredModules(new[]
                {
                    new Project("x", mod1), 
                    new Project("y", mod1), 
                });

            matches.Should().BeEmpty();
        }

        [Test]
        public void ModulesPartiallyCovered()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var mod1 = suite.GetModule("mod1");
            var mod2 = suite.GetModule("mod2");
            var projx = mod1.GetProject("x");
            var projy = mod1.GetProject("y");

            var matches = a.GetCoveredModules(new[]
                {
                    projx
                });

            matches.Should().HaveCount(1);
            matches.First().Partial.Should().BeTrue();
            matches.First().Module.Should().Be(mod1);
        }

        [Test]
        public void ModulesFullyCovered()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var mod1 = suite.GetModule("mod1");
            var mod2 = suite.GetModule("mod2");
            var projx = mod1.GetProject("x");
            var projy = mod1.GetProject("y");
            var projz = mod2.GetProject("z");

            var matches = a.GetCoveredModules(new[]
                {
                    projx,
                    projz,
                    projy
                }).ToList();

            matches.Should().HaveCount(2);
            matches.Should().Contain(
                m => m.Module == mod1 && !m.Partial && !m.IncludingTests);
            matches.Should().Contain(
                m => m.Module == mod2 && !m.Partial && !m.IncludingTests);
        }

        [Test]
        public void TestProjectsPartiallyCovered()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var mod1 = suite.GetModule("mod1");
            var projx = mod1.GetProject("x");
            var test1 = mod1.GetTestProject("1");
            var test2 = mod1.GetTestProject("2");

            var matches = a.GetCoveredModules(new[]
                {
                    projx,
                    test1
                });

            matches.Should().ContainSingle(
                m => m.Module == mod1 && m.Partial && m.IncludingTests);
        }

        [Test]
        public void ModuleWithTestProjectsOnly()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var mod1 = suite.GetModule("mod1");
            var test1 = mod1.GetTestProject("1");

            var matches = a.GetCoveredModules(new[]
                {
                    test1
                });

            matches.Should().ContainSingle(
                m => m.Module == mod1 && !m.Partial && m.IncludingTests);
        }

        [Test]
        public void TestProjectsFullyCovered()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var mod1 = suite.GetModule("mod1");
            var projx = mod1.GetProject("x");
            var test1 = mod1.GetTestProject("1");
            var test2 = mod1.GetTestProject("2");

            var matches = a.GetCoveredModules(new[]
                {
                    projx,
                    test1,
                    test2
                });

            matches.Should().ContainSingle(
                m => m.Module == mod1 && !m.Partial && m.IncludingTests);
        }

        [Test]
        public void GetProductNameWithEmptyList()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            a.GetProductName(new Module[0]).Should().BeNull();
        }

        [Test]
        public void GetProductNameExactMatch()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var mod1 = suite.GetModule("mod1");
            var mod2 = suite.GetModule("mod2");
            var mod3 = suite.GetModule("mod3");
            var prod1 = suite.GetProduct("prod1");
            prod1.AddModule(mod2);
            prod1.AddModule(mod3);

            a.GetProductName(new[] {mod3, mod2}).Should().Be("prod1");
        }

        [Test]
        public void GetProductNameSubsetDoesNotMatch()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var mod1 = suite.GetModule("mod1");
            var mod2 = suite.GetModule("mod2");
            var mod3 = suite.GetModule("mod3");
            var prod1 = suite.GetProduct("prod1");
            prod1.AddModule(mod2);
            prod1.AddModule(mod3);

            a.GetProductName(new[] {mod3}).Should().BeNull();
        }

        [Test]
        public void GetProductNameSupersetDoesNotMatch()
        {
            var a = new DefaultSuiteContentsAnalyzer(suite);
            var mod1 = suite.GetModule("mod1");
            var mod2 = suite.GetModule("mod2");
            var mod3 = suite.GetModule("mod3");
            var prod1 = suite.GetProduct("prod1");
            prod1.AddModule(mod2);
            prod1.AddModule(mod3);

            a.GetProductName(new[] { mod3, mod1, mod2 }).Should().BeNull();
        }
    }
}
