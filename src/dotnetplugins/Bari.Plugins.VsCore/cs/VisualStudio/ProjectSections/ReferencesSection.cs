using System.IO;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio.ProjectSections
{
    /// <summary>
    /// .csproj section describing the project's assembly references
    /// </summary>
    public class ReferencesSection: MSBuildProjectSectionBase
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly string sourceSetName;
        private readonly IFileSystemDirectory targetDir;

        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        /// <param name="projectGuidManagement">Project GUID management service</param>
        /// <param name="sourceSetName">Source set name</param>
        /// <param name="targetDir">Target directory where the compiled files will be placed</param>
        public ReferencesSection(Suite suite, IProjectGuidManagement projectGuidManagement, string sourceSetName, [TargetRoot] IFileSystemDirectory targetDir)
            : base(suite)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.sourceSetName = sourceSetName;
            this.targetDir = targetDir;
        }

        /// <summary>
        /// Writes the section using an XML writer
        /// </summary>
        /// <param name="writer">XML writer to use</param>
        /// <param name="project">The project to generate .csproj for</param>
        /// <param name="context">Current .csproj generation context</param>
        public override void Write(XmlWriter writer, Project project, IMSBuildProjectGeneratorContext context)
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

                    if (referredProject.Type == ProjectType.Library)
                    {
                        writer.WriteComment("Project reference " + projectGuidManagement.GetGuid(project));
                        writer.WriteStartElement("Reference");
                        writer.WriteAttributeString("Include", projectName);
                        writer.WriteElementString("HintPath",
                            ToProjectRelativePath(project,
                                Path.Combine(Suite.SuiteRoot.GetRelativePath(targetDir), referredProject.Module.Name,
                                    referredProject.Name + ".dll"), sourceSetName));
                        writer.WriteElementString("SpecificVersion", "False");
                        writer.WriteEndElement();
                    }
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
                        string relativePath = ToProjectRelativePath(project, Path.Combine("target", refPath), sourceSetName);

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