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
        private readonly IFileSystemDirectory targetDir;

        public PropertiesSection(Suite suite, IProjectGuidManagement projectGuidManagement, [TargetRoot] IFileSystemDirectory targetDir)
            : base(suite)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.targetDir = targetDir;
        }

        public override void Write(XmlWriter writer, Project project, IMSBuildProjectGeneratorContext context)
        {
            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Label", "Global");
            writer.WriteElementString("ProjectGuid", projectGuidManagement.GetGuid(project).ToString("B"));
            writer.WriteElementString("Keyword", "Win32Proj");
            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == 'Bari|Bari' ");
            WriteConfigurationSpecificPart(writer, project);
            writer.WriteEndElement();

            writer.WriteStartElement("ItemDefinitionGroup");
            writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == 'Bari|Bari' ");
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
                                      ToProjectRelativePath(project,
                                                            Path.Combine(Suite.SuiteRoot.GetRelativePath(targetDir),
                                                                         project.Module.Name), "cpp"));
            writer.WriteElementString("IntDir",
                                      ToProjectRelativePath(project,
                                                            Path.Combine(Suite.SuiteRoot.GetRelativePath(targetDir), "tmp",
                                                                         project.Module.Name), "cpp"));
        }
    }
}