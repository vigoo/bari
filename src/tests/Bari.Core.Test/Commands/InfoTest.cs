using System.Linq;
using Bari.Core.Commands;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Core.UI;
using FluentAssertions;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Commands
{
    [TestFixture]
    public class InfoTest
    {
        private IKernel kernel;
        private TestUserOutput output;
        private Suite suite;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            output = new TestUserOutput();
            kernel.Bind<IUserOutput>().ToConstant(output).InSingletonScope();

            suite = kernel.Get<Suite>();
            suite.Name = "test suite";
            var mod1 = suite.GetModule("first module");
            var mod2 = suite.GetModule("other module");
            var mod3 = suite.GetModule("last module");

            var proj11 = mod1.GetProject("project 1.1");
            var proj12 = mod1.GetProject("project 1.2");
            var proj31 = mod3.GetProject("project 3.1");

            var cs = proj31.GetSourceSet("cs");
            var vb = proj31.GetSourceSet("vb");
            var fs = proj31.GetSourceSet("fs");

            cs.Add(new SuiteRelativePath("a.cs"));
            cs.Add(new SuiteRelativePath("b.cs"));
            cs.Add(new SuiteRelativePath("c.cs"));
            
            vb.Add(new SuiteRelativePath("x.vb"));
        }

        [Test]
        public void Exists()
        {
            var cmd = kernel.Get<ICommand>("info");
            cmd.Should().NotBeNull();
            cmd.Name.Should().Be("info");
        }

        [Test]
        public void HasHelpAndDescription()
        {
            var cmd = kernel.Get<ICommand>("info");
            cmd.Description.Should().NotBeBlank();
            cmd.Help.Should().NotBeBlank();
        }

        [Test]
        [ExpectedException(typeof(InvalidCommandParameterException))]
        public void CallingWithAnyParametersThrowsException()
        {
            var cmd = kernel.Get<ICommand>("info");
            cmd.Run(kernel.Get<Suite>(), new[] { "anything" });
        }

        [Test]
        public void PrintsSuiteName()
        {
            var cmd = kernel.Get<ICommand>("info");
            cmd.Run(suite, new string[0]);

            output.Messages.Select(x => x.Trim()).Should().Contain("*Suite name:* test suite");
        }

        [Test]
        public void PrintsAllModuleNames()
        {
            var cmd = kernel.Get<ICommand>("info");
            cmd.Run(suite, new string[0]);

            output.Messages.Select(x => x.Trim()).Should().Contain("*Name:* first module");
            output.Messages.Select(x => x.Trim()).Should().Contain("*Name:* other module");
            output.Messages.Select(x => x.Trim()).Should().Contain("*Name:* last module");
        }

        [Test]
        public void PrintsAllProjectNames()
        {
            var cmd = kernel.Get<ICommand>("info");
            cmd.Run(suite, new string[0]);

            output.Messages.Select(x => x.Trim()).Should().Contain("*Name:* project 1.1");
            output.Messages.Select(x => x.Trim()).Should().Contain("*Name:* project 1.2");
            output.Messages.Select(x => x.Trim()).Should().Contain("*Name:* project 3.1");
        }

        [Test]
        public void PrintsFileCountForNonEmptySourceSets()
        {
            var cmd = kernel.Get<ICommand>("info");
            cmd.Run(suite, new string[0]);

            output.Messages.Select(x => x.Trim()).Should().Contain("`cs`\t3 files");
            output.Messages.Select(x => x.Trim()).Should().Contain("`vb`\t1 files");
        }

        [Test]
        public void DoesNotPrintSourceCountForEmptySourceSets()
        {
            var cmd = kernel.Get<ICommand>("info");
            cmd.Run(suite, new string[0]);

            output.Messages.Select(x => x.Trim()).Should().NotContain("`fs`\t0 files");
        }
    }
}