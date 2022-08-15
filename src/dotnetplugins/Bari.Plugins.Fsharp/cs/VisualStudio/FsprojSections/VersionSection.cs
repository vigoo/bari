using System.Xml;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;
using System.IO;

namespace Bari.Plugins.Fsharp.VisualStudio.FsprojSections
{
    /// <summary>
    /// .fsproj section generating and referring to the version information
    /// </summary>
    public class VersionSection : MSBuildProjectSectionBase
    {
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        public VersionSection(Suite suite)
            : base(suite)
        {
        }

        /// <summary>
        /// Writes the section using an XML writer
        /// </summary>
        /// <param name="writer">XML writer to use</param>
        /// <param name="project">The project to generate .csproj for</param>
        /// <param name="context">Current .csproj generation context</param>
        public override void Write(XmlWriter writer, Project project, IMSBuildProjectGeneratorContext context)
        {
            if (context.VersionOutput != null)
            {
                // Generating the version file (F# source code)
                var generator = new FsharpVersionInfoGenerator(project);
                generator.Generate(context.VersionOutput);

                // Adding reference to it to the .csproj file
                writer.WriteStartElement("ItemGroup");
                writer.WriteStartElement("Compile");
                writer.WriteAttributeString("Include", Path.Combine("..", context.VersionFileName));
                writer.WriteElementString("Link", Path.Combine("_Generated", "version.fs"));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }
}