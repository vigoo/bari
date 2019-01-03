using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IProjectPathManagement pathManagement;
        private readonly IFileSystemDirectory targetDir;

        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        /// <param name="projectGuidManagement">Project GUID management service</param>
        /// <param name="sourceSetName">Source set name</param>
        /// <param name="pathManagement">Project-projectfile mapping</param>
        /// <param name="targetDir">Target directory where the compiled files will be placed</param>
        public ReferencesSection(Suite suite, IProjectGuidManagement projectGuidManagement, string sourceSetName, IProjectPathManagement pathManagement, [TargetRoot] IFileSystemDirectory targetDir)
            : base(suite)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.sourceSetName = sourceSetName;
            this.pathManagement = pathManagement;
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

            foreach (var refPath in context.References.Where(IsValidReference))
            {
                if (IsSolutionReference(refPath))
                {
                    var moduleAndprojectName = ((string) refPath).Substring(4);
                    var parts = moduleAndprojectName.Split('#');
                    var moduleName = parts[0];
                    var projectName = parts[1];

                    var referredProject = Suite.GetModule(moduleName).GetProjectOrTestProject(projectName);

                    if (referredProject.Type == ProjectType.Library ||
                        referredProject.Type == ProjectType.Executable ||
                        referredProject.Type == ProjectType.WindowsExecutable)
                    {
                        writer.WriteComment("Project reference " + projectGuidManagement.GetGuid(referredProject));

                        var projectPath = pathManagement.GetProjectFile(referredProject);
                        if (projectPath != null)
                        {
                            writer.WriteStartElement("ProjectReference");
                            writer.WriteAttributeString("Include",
                                Suite.SuiteRoot.GetRelativePathFrom(
                                    project.RootDirectory.GetChildDirectory(
                                        project.RootDirectory.ChildDirectories.First()), projectPath));
                            writer.WriteElementString("Project", projectGuidManagement.GetGuid(referredProject).ToString("B"));
                            writer.WriteElementString("Name", referredProject.Name);
                            writer.WriteEndElement();
                        }
                        else
                        {
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
                }
                else
                {
                    writer.WriteStartElement("Reference");
                    if (IsGACReference(refPath))
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
                        writer.WriteElementString("Private", "True");
                    }
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        private static bool IsSolutionReference(TargetRelativePath refPath)
        {
            return ((string)refPath).StartsWith("SLN!");
        }

        private static bool IsGACReference(TargetRelativePath refPath)
        {
            return ((string) refPath).StartsWith("GAC!");
        }

        private static readonly ISet<string> allowedExtensions = new HashSet<string>
        {
            ".dll", 
            ".exe"
        };

        private bool IsValidReference(TargetRelativePath reference)
        {
            if (IsGACReference(reference) || IsSolutionReference(reference))
                return true;
            else
            {
                var extension = Path.GetExtension(reference);
                if (extension != null)
                {
                    return allowedExtensions.Contains(extension.ToLowerInvariant());
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
