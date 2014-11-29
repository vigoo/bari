using System.Xml;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;
using System.IO;

namespace Bari.Plugins.Csharp.VisualStudio.CsprojSections
{
    /// <summary>
    /// Section for generating <c>WCFMetadata</c> and <c>WCFMetadataStorage</c> items
    /// </summary>
    public class WCFMetadataSection: MSBuildProjectSectionBase
    {
        public WCFMetadataSection(Suite suite) : base(suite)
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
            var csRoot = project.RootDirectory.GetChildDirectory("cs");
            if (csRoot != null)
            {
                var serviceReferencesRoot = csRoot.GetChildDirectory("Service References");
                if (serviceReferencesRoot != null)
                {
                    writer.WriteStartElement("ItemGroup");
                    
                    writer.WriteStartElement("WCFMetadata");
                    writer.WriteAttributeString("Include", "Service References" + Path.DirectorySeparatorChar);
                    writer.WriteEndElement();

                    foreach (var child in serviceReferencesRoot.ChildDirectories)
                    {
                        writer.WriteStartElement("WCFMetadataStorage");
                        writer.WriteAttributeString("Include", "Service References\\" + child + Path.DirectorySeparatorChar);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
            }
        }
    }
}