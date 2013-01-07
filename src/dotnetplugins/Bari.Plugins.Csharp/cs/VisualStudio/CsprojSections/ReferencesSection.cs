using System.IO;
using System.Xml;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio.CsprojSections
{
    /// <summary>
    /// .csproj section describing the project's assembly references
    /// </summary>
    public class ReferencesSection: CsprojSectionBase
    {
        private readonly IProjectGuidManagement projectGuidManagement;

        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        /// <param name="projectGuidManagement">Project GUID management service</param>
        public ReferencesSection(Suite suite, IProjectGuidManagement projectGuidManagement) : base(suite)
        {
            this.projectGuidManagement = projectGuidManagement;
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

            foreach (var refPath in context.References)
            {
                if (((string)refPath).StartsWith("SLN!"))
                {
                    var moduleAndprojectName = ((string) refPath).Substring(4);
                    var parts = moduleAndprojectName.Split('#');
                    var moduleName = parts[0];
                    var projectName = parts[1];

                    var referredProject = Suite.GetModule(moduleName).GetProjectOrTestProject(projectName);

                    writer.WriteStartElement("ProjectReference");
                    writer.WriteAttributeString("Include", 
                        ToProjectRelativePath(project, Path.Combine(Suite.SuiteRoot.GetRelativePath(referredProject.RootDirectory), projectName + ".csproj")));
                    writer.WriteElementString("Project", projectGuidManagement.GetGuid(referredProject).ToString("B"));
                    writer.WriteElementString("Name", projectName);
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement("Reference");
                    if (((string) refPath).StartsWith("GAC!"))
                    {
                        var assemblyName = ((string) refPath).Substring(4);
                        writer.WriteAttributeString("Include", assemblyName);
                    }
                    else
                    {
                        string relativePath = ToProjectRelativePath(project, Path.Combine("target", refPath));

                        writer.WriteAttributeString("Include", Path.GetFileNameWithoutExtension(relativePath));
                        writer.WriteElementString("HintPath", relativePath);
                        writer.WriteElementString("SpecificVersion", "False");
                    }
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

    }
}