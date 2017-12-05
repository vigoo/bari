using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.VCpp.VisualStudio
{
    /// <summary>
    /// Class generating .vcxproj project files
    /// </summary>
    public class VcxprojGenerator: IMSBuildProjectGeneratorContext
    {
        private readonly IEnumerable<IMSBuildProjectSection> sections;
        private ISet<TargetRelativePath> references;
        private TextWriter versionOutput;
        private string versionFileName;

        /// <summary>
        /// Gets the set of references for the given project
        /// </summary>
        public ISet<TargetRelativePath> References
        {
            get { return references; }
        }

        /// <summary>
        /// Gets the text writer used to generate version information C++ file
        /// </summary>
        public TextWriter VersionOutput
        {
            get { return versionOutput; }
        }

        /// <summary>
        /// Gets the name of the file the <see cref="IMSBuildProjectGeneratorContext.VersionOutput"/> writer generates
        /// </summary>
        public string VersionFileName
        {
            get { return versionFileName; }
        }

        /// <summary>
        /// Initializes the project file generator
        /// </summary>
        /// <param name="sections">Vcxproj section writers to be used</param>
        public VcxprojGenerator(IEnumerable<IMSBuildProjectSection> sections)
        {            
            this.sections = sections;
        }

        /// <summary>
        /// Writes the output
        /// </summary>
        /// <param name="project">The project to generate vcxproj file for</param>
        /// <param name="references">Paths to the external references to be included in the project</param>
        /// <param name="output">Output where the vcxproj file will be written</param>
        /// <param name="versionOutput">Output where the version info should be generated</param>
        /// <param name="versionFileName">File name relative to the vcxproj file for the version info</param>      
        public void Generate(Project project, IEnumerable<TargetRelativePath> references, TextWriter output, TextWriter versionOutput, string versionFileName)
        {
            this.references = new HashSet<TargetRelativePath>(references);
            this.versionOutput = versionOutput;
            this.versionFileName = versionFileName;

            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
                NamespaceHandling = NamespaceHandling.Default,
                Encoding = Encoding.UTF8
            };
            var writer = XmlWriter.Create(output, settings);

            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
            writer.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            writer.WriteAttributeString("ToolsVersion", "4.0");
            writer.WriteAttributeString("DefaultTargets", "Build");

            writer.WriteStartElement("Import");
            writer.WriteAttributeString("Project", @"$(VCTargetsPath)\Microsoft.Cpp.Default.props");
            writer.WriteEndElement();

            foreach (var section in sections)
                section.Write(writer, project, this);

            writer.WriteStartElement("Import");
            writer.WriteAttributeString("Project", @"$(VCTargetsPath)\Microsoft.Cpp.targets");
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.Flush();
        }
    }
}