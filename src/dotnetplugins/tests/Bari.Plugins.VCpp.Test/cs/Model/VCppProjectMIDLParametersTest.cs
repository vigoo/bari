using System.IO;
using System.Xml;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using Bari.Plugins.VCpp.Model;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Plugins.VCpp.Test.Model
{
    [TestFixture]
    public class VCppProjectMIDLParametersTest
    {
        private Suite x86Suite;
        private Suite x64Suite;

        [SetUp]
        public void SetUp()
        {
            var root = new TestFileSystemDirectory("root");
            var goalx86 = new Goal("debug-x86", new[] { Suite.DebugGoal, new Goal("x86") });
            var goalx64 = new Goal("debug-x64", new[] { Suite.DebugGoal, new Goal("x64") });

            x86Suite = new Suite(root, new[] {goalx86, goalx64}, goalx86);
            x64Suite = new Suite(root, new[] {goalx86, goalx64}, goalx64);
        }

        [Test]
        public void DefaultTargetEnvironemntForX86()
        {
            var p = new VCppProjectMIDLParameters(x86Suite);
            p.TargetEnvironment.Should().Be(MidlTargetEnvironment.Win32);
        }

        [Test]
        public void DefaultTargetEnvironemntForX64()
        {
            var p = new VCppProjectMIDLParameters(x64Suite);
            p.TargetEnvironment.Should().Be(MidlTargetEnvironment.X64);
        }

        [Test]
        public void DefaultTargetEnvironemntForX86WrittenToVcxproj()
        {
            var p = new VCppProjectMIDLParameters(x86Suite);
            var xml = GetProperties(p);

            var defineNodes = xml.SelectNodes("TargetEnvironment");
            defineNodes.Should().NotBeNull();
            defineNodes.Count.Should().Be(1);
            defineNodes[0].InnerText.Should().Be("Win32");
        }

        [Test]
        public void DefaultTargetEnvironemntForX64WrittenToVcxproj()
        {
            var p = new VCppProjectMIDLParameters(x64Suite);
            var xml = GetProperties(p);

            var defineNodes = xml.SelectNodes("TargetEnvironment");
            defineNodes.Should().NotBeNull();
            defineNodes.Count.Should().Be(1);
            defineNodes[0].InnerText.Should().Be("X64");
        }

        private XmlElement GetProperties(VCppProjectMIDLParameters p)
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