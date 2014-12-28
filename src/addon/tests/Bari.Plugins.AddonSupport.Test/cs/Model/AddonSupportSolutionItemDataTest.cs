using Bari.Core.Commands.Helper;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Plugins.AddonSupport.Model;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.IO;

namespace Bari.Plugins.AddonSupport.Test.Model
{
    [TestFixture]
    public class AddonSupportSolutionItemDataTest
    {
        private Suite suite;
        private Suite suiteNoExe;
        private Product product;
        private Mock<ICommandTargetParser> targetParser;
        private Mock<IHasBuildTarget> buildTargetSource;
        private AddonSupportSolutionItemData data;

        [SetUp]
        public void SetUp()
        {
            suite = new Suite(new TestFileSystemDirectory("root"));
            suiteNoExe = new Suite(new TestFileSystemDirectory("root"));
            suite.GetModule("TestModule").GetProject("TestLib").Type = ProjectType.Library;
            suiteNoExe.GetModule("TestModule").GetProject("TestLib").Type = ProjectType.Library;
            suite.GetModule("TestModule").GetProject("TestExe").Type = ProjectType.Executable;

            product = suite.GetProduct("TestProduct");
            product.AddModule(suite.GetModule("TestModule"));

            targetParser = new Mock<ICommandTargetParser>();
            buildTargetSource = new Mock<IHasBuildTarget>();
            data = new AddonSupportSolutionItemData(targetParser.Object, buildTargetSource.Object, new Goal("test"));
        }

        [Test]
        public void GoalName()
        {
            data.Goal.Should().Be("test");
        }

        [Test]
        public void TargetGotFromCommand()
        {
            buildTargetSource.SetupGet(s => s.BuildTarget).Returns("testtarget");
            data.Target.Should().Be("testtarget");
        }

        [Test]
        public void TargetNotAvailableInCommand()
        {
            data = new AddonSupportSolutionItemData(targetParser.Object, null, new Goal("test"));
            data.Target.Should().BeEmpty();
        }

        [Test]
        public void ExePathForNonProductBuild()
        {
            targetParser.Setup(p => p.ParseTarget(It.IsAny<string>())).Returns(new FullSuiteTarget(suite));
            data.StartupPath.Should().Be(Path.Combine("TestModule", "TestExe.exe"));
        }

        [Test]
        public void NoExePathIfNoExecutableProjects()
        {
            targetParser.Setup(p => p.ParseTarget(It.IsAny<string>())).Returns(new FullSuiteTarget(suiteNoExe));
            data.StartupPath.Should().BeEmpty();
        }

        [Test]
        public void ProductTargetNoExplicitStartupExe()
        {
            targetParser.Setup(p => p.ParseTarget(It.IsAny<string>())).Returns(new ProductTarget(product));
            data.StartupPath.Should().Be(Path.Combine("TestModule", "TestExe.exe"));
        }

        [Test]
        public void ProductWithExplicitStartupExe()
        {
            var mod2 = suite.GetModule("TestModule2");
            var alternativeExe = mod2.GetProject("AlternativeExe");
            alternativeExe.Type = ProjectType.WindowsExecutable;
            product.AddModule(mod2);
            product.AddParameters("startup", new StartupModuleParameters(alternativeExe));

            targetParser.Setup(p => p.ParseTarget(It.IsAny<string>())).Returns(new ProductTarget(product));
            data.StartupPath.Should().Be(Path.Combine("TestModule2", "AlternativeExe.exe"));
        }
    }
}