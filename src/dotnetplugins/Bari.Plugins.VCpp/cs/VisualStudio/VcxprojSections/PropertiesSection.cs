using System.IO;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.VCpp.VisualStudio.VcxprojSections
{
    public class PropertiesSection : MSBuildProjectSectionBase
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IProjectPlatformManagement platformManagement;
        private readonly IFileSystemDirectory targetDir;

        public PropertiesSection(Suite suite, IProjectGuidManagement projectGuidManagement, [TargetRoot] IFileSystemDirectory targetDir, IProjectPlatformManagement platformManagement)
            : base(suite)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.targetDir = targetDir;
            this.platformManagement = platformManagement;
        }

        public override void Write(XmlWriter writer, Project project, IMSBuildProjectGeneratorContext context)
        {
            var platform = platformManagement.GetDefaultPlatform(project);

            writer.WriteStartElement("ItemGroup");
            writer.WriteAttributeString("Label", "ProjectConfigurations");
            writer.WriteStartElement("ProjectConfiguration");
            writer.WriteAttributeString("Include", string.Format("Bari|{0}", platform));
            writer.WriteElementString("Configuration", "Bari");
            writer.WriteElementString("Platform", platform);
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Label", "Global");
            writer.WriteElementString("ProjectGuid", projectGuidManagement.GetGuid(project).ToString("B"));
            writer.WriteElementString("Keyword", "Win32Proj");
            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Condition", string.Format("'$(Configuration)|$(Platform)' == 'Bari|{0}' ", platform));
            WriteConfigurationSpecificPart(writer, project);
            writer.WriteEndElement();

            writer.WriteStartElement("ItemDefinitionGroup");
            writer.WriteAttributeString("Condition", string.Format(" '$(Configuration)|$(Platform)' == 'Bari|{0}' ", platform));
            WriteCompilerParameters(writer, project);
            WriteLinkerParameters(writer, project);
            writer.WriteEndElement();
        }

        private void WriteCompilerParameters(XmlWriter writer, Project project)
        {
            VCppProjectCompilerParameters compilerParameters = project.HasParameters("cpp-compiler")
                                                                   ? project.GetParameters<VCppProjectCompilerParameters>(
                                                                       "cpp-compiler")
                                                                   : new VCppProjectCompilerParameters(Suite);

            compilerParameters.FillProjectSpecificMissingInfo(project);

            writer.WriteStartElement("ClCompile");
            compilerParameters.ToVcxprojProperties(writer);
            writer.WriteEndElement();
        }

        private void WriteLinkerParameters(XmlWriter writer, Project project)
        {
            VCppProjectLinkerParameters compilerParameters = project.HasParameters("cpp-linker")
                                                                   ? project.GetParameters<VCppProjectLinkerParameters>("cpp-linker")
                                                                   : new VCppProjectLinkerParameters(Suite);

            compilerParameters.FillProjectSpecificMissingInfo(project);

            writer.WriteStartElement("Link");
            compilerParameters.ToVcxprojProperties(writer);
            writer.WriteEndElement();
        }

        private void WriteConfigurationSpecificPart(XmlWriter writer, Project project)
        {
            writer.WriteElementString("OutDir",
                                      ToProjectRelativePath(project, Path.Combine(Suite.SuiteRoot.GetRelativePath(targetDir), project.Module.Name), "cpp") + '\\');
            writer.WriteElementString("IntDir",
                                      ToProjectRelativePath(project, Path.Combine(Suite.SuiteRoot.GetRelativePath(targetDir), "tmp", project.Module.Name), "cpp") + '\\');
        }
    }
}