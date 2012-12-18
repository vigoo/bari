using System.IO;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Plugins.Csharp.VisualStudio;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bari.Plugins.Csharp.Test.VisualStudio
{
    [TestFixture]
    public class CsharpVersionInfoGeneratorTest
    {
        private Module module;
        private Project project;

        [SetUp]
        public void SetUp()
        {
            module = new Module("testmod", new Suite(new TestFileSystemDirectory("root")));
            project = new Project("test", module);
        }

        [Test]
        public void NullVersionIsNotWritten()
        {
            project.EffectiveVersion.Should().BeNull();

            var lines = RunGenerator(project);
            lines.Should().NotContain(l => l.StartsWith("[assembly: AssemblyVersion"));
            lines.Should().NotContain(l => l.StartsWith("[assembly: AssemblyFileVersion"));
        }

        [Test]
        public void EmptyVersionIsNotWritten()
        {
            project.Version = "  ";
            project.EffectiveVersion.Should().BeBlank();

            var lines = RunGenerator(project);
            lines.Should().NotContain(l => l.StartsWith("[assembly: AssemblyVersion"));
            lines.Should().NotContain(l => l.StartsWith("[assembly: AssemblyFileVersion"));
        }

        [Test]
        public void NonEmptyVersionIsWritten()
        {
            project.Version = "1.0.0.0";
            project.EffectiveVersion.Should().Be("1.0.0.0");

            var lines = RunGenerator(project);
            lines.Should().Contain("[assembly: AssemblyVersion(\"1.0.0.0\")]");
            lines.Should().Contain("[assembly: AssemblyFileVersion(\"1.0.0.0\")]");
        }

        [Test]
        public void VersionFromModuleIsWrittenIfProjectHasNoVersion()
        {
            project.Version = null;
            module.Version = "2.0.0.0";
            project.EffectiveVersion.Should().Be("2.0.0.0");

            var lines = RunGenerator(project);
            lines.Should().Contain("[assembly: AssemblyVersion(\"2.0.0.0\")]");
            lines.Should().Contain("[assembly: AssemblyFileVersion(\"2.0.0.0\")]");
        }

        private static string[] RunGenerator(Project project)
        {
            var generator = new CsharpVersionInfoGenerator(project);
            using (var tw = new StringWriter())
            {
                generator.Generate(tw);
                return tw.ToString().Split('\n', '\r');
            }
        }
    }
}