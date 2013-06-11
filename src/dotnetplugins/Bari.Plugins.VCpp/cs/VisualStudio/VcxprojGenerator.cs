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

        /// <summary>
        /// Gets the set of references for the given project
        /// </summary>
        public ISet<TargetRelativePath> References
        {
            get { return references; }
        }

        /// <summary>
        /// Gets the text writer used to generate version information F# file
        /// </summary>
        public TextWriter VersionOutput
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the name of the file the <see cref="IMSBuildProjectGeneratorContext.VersionOutput"/> writer generates
        /// </summary>
        public string VersionFileName
        {
            get { return null; }
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
        public void Generate(Project project, IEnumerable<TargetRelativePath> references, TextWriter output)
        {
            this.references = new HashSet<TargetRelativePath>(references);

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