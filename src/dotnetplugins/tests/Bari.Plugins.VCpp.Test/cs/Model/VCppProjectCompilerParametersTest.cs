using System.IO;
using System.Xml;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Plugins.VCpp.Model;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bari.Plugins.VCpp.Test.Model
{
    [TestFixture]
    public class VCppProjectCompilerParametersTest
    {
        private Suite debugSuite;
        private Suite releaseSuite;
        private Suite customDebugSuite;
        private Suite customReleaseSuite;

        [SetUp]
        public void SetUp()
        {
            var root = new TestFileSystemDirectory("root");
            debugSuite = new Suite(root, new[] {Suite.DebugGoal, Suite.ReleaseGoal}, Suite.DebugGoal);
            releaseSuite = new Suite(root, new[] { Suite.DebugGoal, Suite.ReleaseGoal }, Suite.ReleaseGoal);

            var customDebug = new Goal("test-debug", new[] {Suite.DebugGoal});
            var customRelease = new Goal("test-release", new[] { Suite.ReleaseGoal });

            customDebugSuite = new Suite(root, new[] { customDebug, customRelease }, customDebug);
            customReleaseSuite = new Suite(root, new[] { customDebug, customRelease }, customRelease);
        }

        private void OptimizationEnabledFor(VCppProjectCompilerParameters p)
        {
            p.Favor.Should().Be(OptimizationFavor.Speed);
            p.Optimization.Should().Be(OptimizationLevel.Full);
            p.WholeProgramOptimization.Should().BeTrue();
            p.SmallerTypeCheck.Should().BeFalse();
        }

        private void OptimizationDisabledFor(VCppProjectCompilerParameters p)
        {
            p.Favor.Should().Be(OptimizationFavor.Speed);
            p.Optimization.Should().Be(OptimizationLevel.Disabled);
            p.WholeProgramOptimization.Should().BeFalse();
            p.SmallerTypeCheck.Should().BeTrue();
        }

        [Test]
        public void OptimizationEnabledForRelease()
        {
            var p1 = new VCppProjectCompilerParameters(releaseSuite);
            OptimizationEnabledFor(p1);
        }

        [Test]
        public void OptimizationEnabledForCustomRelease()
        {
            var p1 = new VCppProjectCompilerParameters(customReleaseSuite);
            OptimizationEnabledFor(p1);
        }

        [Test]
        public void OptimizationDisabledForDebug()
        {
            var p1 = new VCppProjectCompilerParameters(debugSuite);
            OptimizationDisabledFor(p1);
        }

        [Test]
        public void OptimizationDisabledForCustomDebug()
        {
            var p1 = new VCppProjectCompilerParameters(customDebugSuite);
            OptimizationDisabledFor(p1);
        }

        private void HasDebugDefines(VCppProjectCompilerParameters p)
        {
            p.Defines.Should().Contain("_DEBUG");
            p.Defines.Should().NotContain("NDEBUG");
        }

        private void HasReleaseDefines(VCppProjectCompilerParameters p)
        {
            p.Defines.Should().Contain("NDEBUG");
            p.Defines.Should().NotContain("_DEBUG");
        }

        [Test]
        public void DefaultDefinesForRelease()
        {
            var p = new VCppProjectCompilerParameters(releaseSuite);
            HasReleaseDefines(p);
        }

        [Test]
        public void DefaultDefinesForDebug()
        {
            var p = new VCppProjectCompilerParameters(debugSuite);
            HasDebugDefines(p);
        }

        [Test]
        public void DefaultDefinesForCustomRelease()
        {
            var p = new VCppProjectCompilerParameters(customReleaseSuite);
            HasReleaseDefines(p);
        }

        [Test]
        public void DefaultDefinesForCustomDebug()
        {
            var p = new VCppProjectCompilerParameters(customDebugSuite);
            HasDebugDefines(p);
        }

        [Test]
        public void DefaultDebugDefinesWrittenToVcxproj()
        {
            var p = new VCppProjectCompilerParameters(debugSuite);
            var xml = GetProperties(p);

            var defineNodes = xml.SelectNodes("PreprocessorDefinitions");
            defineNodes.Should().NotBeNull();
            defineNodes.Count.Should().Be(1);
            defineNodes[0].InnerText.Should().Be("_DEBUG");
        }

        [Test]
        public void DefaultReleaseDefinesWrittenToVcxproj()
        {
            var p = new VCppProjectCompilerParameters(releaseSuite);
            var xml = GetProperties(p);

            var defineNodes = xml.SelectNodes("PreprocessorDefinitions");
            defineNodes.Should().NotBeNull();
            defineNodes.Count.Should().Be(1);
            defineNodes[0].InnerText.Should().Be("NDEBUG");
        }

        [Test]
        public void DefaultDebugOptimizationWrittenToVcxproj()
        {
            var p = new VCppProjectCompilerParameters(debugSuite);
            var xml = GetProperties(p);

            var defineNodes = xml.SelectNodes("Optimization");
            defineNodes.Should().NotBeNull();
            defineNodes.Count.Should().Be(1);
            defineNodes[0].InnerText.Should().Be("Disabled");
        }

        [Test]
        public void DefaultReleaseOptimizationWrittenToVcxproj()
        {
            var p = new VCppProjectCompilerParameters(releaseSuite);
            var xml = GetProperties(p);

            var defineNodes = xml.SelectNodes("Optimization");
            defineNodes.Should().NotBeNull();
            defineNodes.Count.Should().Be(1);
            defineNodes[0].InnerText.Should().Be("Full");
        }

        private XmlElement GetProperties(VCppProjectCompilerParameters p)
        {
            using (var writer = new StringWriter())
            using (var xmlWriter = new XmlTextWriter(writer))
            {
                xmlWriter.WriteStartElement("Root");
                p.ToVcxprojProperties(xmlWriter);
                xmlWriter.WriteEndElement();

                var doc = new XmlDocument();
                doc.LoadXml(writer.ToString());
                return doc.DocumentElement;
            }
        }
    }
}