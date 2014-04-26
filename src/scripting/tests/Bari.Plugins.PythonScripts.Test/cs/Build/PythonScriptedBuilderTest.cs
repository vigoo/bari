using Bari.Core.Build.Dependencies;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Plugins.PythonScripts.Build;
using Bari.Plugins.PythonScripts.Scripting;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bari.Plugins.PythonScripts.Test.Build
{

    [TestFixture]
    public class PythonScriptedBuilderTest
    {
        [Test]
        public void SameScriptedBuilderForDifferentProjectsAreNotEquals()
        {
            var suite = new Suite(new TestFileSystemDirectory("root"));
            var mod = new Module("mod1", suite);
            var project1 = new Project("prj1", mod);
            var project2 = new Project("prj2", mod);
            var script = new Mock<IBuildScript>();
            var fingerprintFactory = new Mock<ISourceSetFingerprintFactory>();
            var scriptRunner = new Mock<IProjectBuildScriptRunner>();

            var builder1 = new PythonScriptedBuilder(project1, script.Object, fingerprintFactory.Object, scriptRunner.Object);
            var builder2 = new PythonScriptedBuilder(project2, script.Object, fingerprintFactory.Object, scriptRunner.Object);

            builder1.Should().NotBe(builder2);
        }
    }
} 