using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;
using System.IO;
using Bari.Plugins.VCpp.Model;

namespace Bari.Plugins.VCpp.VisualStudio.VcxprojSections
{
    /// <summary>
    /// .vcxproj section generating and referring to the version information
    /// </summary>
    public class VersionSection : MSBuildProjectSectionBase
    {
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        public VersionSection(Suite suite) : base(suite)
        {
        }

        /// <summary>
        /// Writes the section using an XML writer
        /// </summary>
        /// <param name="writer">XML writer to use</param>
        /// <param name="project">The project to generate .csproj for</param>
        /// <param name="context">Current .vcxproj generation context</param>
        public override void Write(System.Xml.XmlWriter writer, Core.Model.Project project, VsCore.VisualStudio.IMSBuildProjectGeneratorContext context)
        {
            if (context.VersionOutput != null && project.GetCLIMode() != CppCliMode.Disabled)
            {
                // Generating the version file (C# source code)
                var generator = new CppVersionInfoGenerator(project);
                generator.Generate(context.VersionOutput);

                // Adding reference to it to the .csproj file
                writer.WriteStartElement("ItemGroup");
                writer.WriteStartElement("ClCompile");
                writer.WriteAttributeString("Include", Path.Combine("..", context.VersionFileName));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }
}
