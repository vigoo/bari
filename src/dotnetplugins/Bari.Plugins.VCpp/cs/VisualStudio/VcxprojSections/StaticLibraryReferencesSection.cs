using System.IO;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.VCpp.VisualStudio.VcxprojSections
{
    public class StaticLibraryReferencesSection: MSBuildProjectSectionBase
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IFileSystemDirectory targetDir;

        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        /// <param name="projectGuidManagement">Project GUID management service</param>
        /// <param name="targetDir">Target directory where the compiled files will be placed</param>
        public StaticLibraryReferencesSection(Suite suite, IProjectGuidManagement projectGuidManagement, [TargetRoot] IFileSystemDirectory targetDir)
            : base(suite)
        {
            this.projectGuidManagement = projectGuidManagement;
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
                if (((string) refPath).StartsWith("SLN!"))
                {
                    var moduleAndprojectName = ((string) refPath).Substring(4);
                    var parts = moduleAndprojectName.Split('#');
                    var moduleName = parts[0];
                    var projectName = parts[1];

                    var referredProject = Suite.GetModule(moduleName).GetProjectOrTestProject(projectName);

                    if (referredProject.Type == ProjectType.StaticLibrary)
                    {
                        writer.WriteStartElement("ProjectReference");
                        writer.WriteAttributeString("Include", 
                            ToProjectRelativePath(project, Path.Combine(Suite.SuiteRoot.GetRelativePath(referredProject.RootDirectory), "cpp", projectName+".vcxproj"), "cpp"));
                        writer.WriteElementString("Project", projectGuidManagement.GetGuid(project).ToString("B"));
                        writer.WriteEndElement();
                    }
                }
            }

            writer.WriteEndElement();
        }

    }
}