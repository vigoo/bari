using System.Xml;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio.CsprojSections
{
    /// <summary>
    /// .csproj section listing all the source files
    /// </summary>
    public class SourceItemsSection: CsprojSectionBase
    {
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        public SourceItemsSection(Suite suite) : base(suite)
        {
        }

        /// <summary>
        /// Writes the section using an XML writer
        /// </summary>
        /// <param name="writer">XML writer to use</param>
        /// <param name="project">The project to generate .csproj for</param>
        /// <param name="context">Current .csproj generation context</param>
        public override void Write(XmlWriter writer, Project project, ICsprojGeneratorContext context)
        {
            writer.WriteStartElement("ItemGroup");
            var sourceSet = project.GetSourceSet("cs");
            foreach (var file in sourceSet.Files)
            {
                writer.WriteStartElement("Compile");
                writer.WriteAttributeString("Include", ToProjectRelativePath(project, file));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}