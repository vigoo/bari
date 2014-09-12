using System;
using System.IO;
using System.Linq;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.Exceptions;
using Bari.Plugins.Csharp.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.Csharp.VisualStudio.CsprojSections
{
    /// <summary>
    /// .csproj section for generic project properties
    /// </summary>
    public class PropertiesSection : MSBuildProjectSectionBase
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IFileSystemDirectory targetDir;

        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        /// <param name="projectGuidManagement">Project GUID management service</param>
        /// <param name="targetDir">Target directory where the compiled files will be placed</param>
        public PropertiesSection(Suite suite, IProjectGuidManagement projectGuidManagement, [TargetRoot] IFileSystemDirectory targetDir)
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
            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == 'Bari|Bari' ");
            WriteConfigurationSpecificPart(writer, project);
            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup");
            
            // Writing out configuration specific part to the non conditional block as well
            WriteConfigurationSpecificPart(writer, project);

            writer.WriteElementString("OutputType", GetOutputType(project.Type));
            writer.WriteElementString("AssemblyName", project.Name);
            writer.WriteElementString("ProjectGuid", projectGuidManagement.GetGuid(project).ToString("B"));

            CsharpProjectParameters parameters = project.HasParameters("csharp")
                                                     ? project.GetParameters<CsharpProjectParameters>("csharp")
                                                     : new CsharpProjectParameters(Suite);

            parameters.FillProjectSpecificMissingInfo(project);
            parameters.ToCsprojProperties(writer);       

            WriteAppConfig(writer, project);
            WriteManifest(writer, project);
            WriteApplicationIcon(writer, project, parameters);

            writer.WriteEndElement();
        }

        private void WriteConfigurationSpecificPart(XmlWriter writer, Project project)
        {
            writer.WriteElementString("OutputPath",
                                      ToProjectRelativePath(project,
                                                            Path.Combine(Suite.SuiteRoot.GetRelativePath(targetDir),
                                                                         project.Module.Name), "cs"));
            writer.WriteElementString("IntermediateOutputPath",
                                      ToProjectRelativePath(project,
                                                            Path.Combine(Suite.SuiteRoot.GetRelativePath(targetDir), "tmp",
                                                                         project.Module.Name), "cs"));
        }

        private string GetOutputType(ProjectType type)
        {
            switch (type)
            {
                case ProjectType.Executable:
                    return "Exe";
                case ProjectType.WindowsExecutable:
                    return "WinExe";
                case ProjectType.Library:
                    return "Library";
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }


        private void WriteAppConfig(XmlWriter writer, Project project)
        {
            // Must be called within an open PropertyGroup 

            if (project.HasNonEmptySourceSet("appconfig"))
            {
                var sourceSet = project.GetSourceSet("appconfig");
                var configs = sourceSet.Files.ToList();

                if (configs.Count > 1)
                    throw new TooManyAppConfigsException(project);

                var appConfigPath = configs.FirstOrDefault();
                if (appConfigPath != null)
                {
                    writer.WriteElementString("AppConfig", ToProjectRelativePath(project, appConfigPath, "cs"));
                }
            }
        }

        private void WriteManifest(XmlWriter writer, Project project)
        {
            // Must be called within an open PropertyGroup

            if (project.HasNonEmptySourceSet("manifest"))
            {
                var sourceSet = project.GetSourceSet("manifest");
                var manifests = sourceSet.Files.ToList();

                if (manifests.Count > 1)
                    throw new TooManyManifestsException(project);

                var manifestPath = manifests.FirstOrDefault();
                if (manifestPath != null)
                {
                    writer.WriteElementString("ApplicationManifest", ToProjectRelativePath(project, manifestPath, "cs"));
                }
            }
        }

        private void WriteApplicationIcon(XmlWriter writer, Project project, CsharpProjectParameters parameters)
        {
            if (project.Type == ProjectType.Executable ||
                project.Type == ProjectType.WindowsExecutable)
            {
                string iconPath = Path.Combine(project.RelativeRootDirectory, "resources", parameters.ApplicationIcon);
                if (!String.IsNullOrWhiteSpace(iconPath))
                {
                    writer.WriteElementString("ApplicationIcon", ToProjectRelativePath(project, iconPath, "cs"));
                }
            }
        }
    }
}