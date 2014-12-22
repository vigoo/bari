using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var cliMode = GetCLIMode(project);

            writer.WriteStartElement("ItemGroup");
            writer.WriteAttributeString("Label", "ProjectConfigurations");
            writer.WriteStartElement("ProjectConfiguration");
            writer.WriteAttributeString("Include", string.Format("Bari|{0}", platform));
            writer.WriteElementString("Configuration", "Bari");
            writer.WriteElementString("Platform", platform);
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Label", "Globals");
            writer.WriteElementString("ProjectGuid", projectGuidManagement.GetGuid(project).ToString("B"));

            if (cliMode == CppCliMode.Disabled)
            {
                writer.WriteElementString("Keyword", "Win32Proj");
            }
            else
            {
                writer.WriteElementString("TargetFrameworkVersion", "v4.0");
                writer.WriteElementString("Keyword", "ManagedCProj");
                writer.WriteElementString("RootNamespace", project.Name);
            }

            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Condition", string.Format("'$(Configuration)|$(Platform)' == 'Bari|{0}' ", platform));
            writer.WriteAttributeString("Label", "Configuration");
            WriteHighLevelConfigurationSpecificPart(writer, project);
            writer.WriteEndElement();

            writer.WriteStartElement("Import");
            writer.WriteAttributeString("Project", @"$(VCTargetsPath)\Microsoft.Cpp.props");
            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Condition", string.Format("'$(Configuration)|$(Platform)' == 'Bari|{0}' ", platform));
            WriteConfigurationSpecificPart(writer, project);
            writer.WriteEndElement();

            writer.WriteStartElement("ItemDefinitionGroup");
            writer.WriteAttributeString("Condition", string.Format(" '$(Configuration)|$(Platform)' == 'Bari|{0}' ", platform));
            WriteMIDLParameters(writer, project);
            WriteCompilerParameters(writer, project);
            WriteLinkerParameters(writer, project);
            WriteManifestParameters(writer, project);
            WriteResourceCompilerParameters(writer, project);
            writer.WriteEndElement();
        }

        private void WriteResourceCompilerParameters(XmlWriter writer, Project project)
        {
            writer.WriteStartElement("ResourceCompile");

            var items = new List<string>();
            var ver = project.EffectiveVersion;

            if (ver != null)
            {
                items.Add(String.Format("BARI_PROJECT_VERSION=\"\\\"{0}\\0\\\"\"", ver));
                string[] parts = ver.Split('.');

                if (parts.Length == 4)
                {
                    var nums = parts.Select(int.Parse).ToArray();
                    items.Add(String.Format("BARI_PROJECT_VERSION_VI={0},{1},{2},{3}", nums[0], nums[1], nums[2], nums[3]));
                }                
            }

            if (project.EffectiveCopyright != null)
            {
                items.Add(String.Format("BARI_PROJECT_COPYRIGHT=\"\\\"{0}\\0\\\"\"", project.EffectiveCopyright));
            }

            if (items.Count > 0)
            {
                writer.WriteElementString("PreprocessorDefinitions", String.Join(";", items));
            }

            writer.WriteEndElement();
        }

        private void WriteManifestParameters(XmlWriter writer, Project project)
        {
            var manifestParameters = GetManifestParameters(project);

            if (manifestParameters.GenerateManifest)
            {
                writer.WriteStartElement("Manifest");
                manifestParameters.ToVcxprojProperties(writer);
                writer.WriteEndElement();
            }
        }

        private VCppProjectManifestParameters GetManifestParameters(Project project)
        {
            VCppProjectManifestParameters manifestParameters = project.HasParameters("manifest")
                ? project.GetParameters<VCppProjectManifestParameters>("manifest")
                : new VCppProjectManifestParameters(Suite);

            manifestParameters.FillProjectSpecificMissingInfo(project);
            return manifestParameters;
        }

        private void WriteHighLevelConfigurationSpecificPart(XmlWriter writer, Project project)
        {
            writer.WriteElementString("ConfigurationType", GetConfigurationType(project));
            writer.WriteElementString("UseDebugLibraries", XmlConvert.ToString(Suite.ActiveGoal.Has(Suite.DebugGoal.Name)));
            writer.WriteElementString("PlatformToolset", "v110");

            var cliMode = GetCLIMode(project);
            if (cliMode != CppCliMode.Disabled)
            {
                writer.WriteElementString("CLRSupport", cliMode.ToString().Replace("Enabled", "true"));
            }

            writer.WriteElementString("WholeProgramOptimization", XmlConvert.ToString(Suite.ActiveGoal.Has(Suite.ReleaseGoal.Name)));
            writer.WriteElementString("CharacterSet", "Unicode");

            string useOfAtl = GetUseOfAtl(project);
            if (!String.IsNullOrEmpty(useOfAtl))
                writer.WriteElementString("UseOfAtl", useOfAtl);
        }

        private string GetUseOfAtl(Project project)
        {
            VCppProjectATLParameters atlParameters = project.HasParameters("atl")
                ? project.GetParameters<VCppProjectATLParameters>("atl")
                : new VCppProjectATLParameters();

            switch (atlParameters.UseOfATL)
            {
                case UseOfATL.None:
                    return String.Empty;
                case UseOfATL.Static:
                    return "Static";
                case UseOfATL.Dynamic:
                    return "Dynamic";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private CppCliMode GetCLIMode(Project project)
        {
            VCppProjectCLIParameters cliParameters = project.HasParameters("cli")
                ? project.GetParameters<VCppProjectCLIParameters>("cli")
                : new VCppProjectCLIParameters();

            return cliParameters.Mode;
        }

        private string GetConfigurationType(Project project)
        {
            switch (project.Type)
            {
                case ProjectType.Executable:
                    return "Application";
                case ProjectType.Library:
                    return "DynamicLibrary";
                case ProjectType.StaticLibrary:
                    return "StaticLibrary";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WriteMIDLParameters(XmlWriter writer, Project project)
        {
            VCppProjectMIDLParameters midlParameters = project.HasParameters("midl")
                                                                   ? project.GetParameters<VCppProjectMIDLParameters>("midl")
                                                                   : new VCppProjectMIDLParameters(Suite);

            midlParameters.FillProjectSpecificMissingInfo(project, targetDir as LocalFileSystemDirectory);

            writer.WriteStartElement("Midl");
            midlParameters.ToVcxprojProperties(writer);
            writer.WriteEndElement();
        }

        private void WriteCompilerParameters(XmlWriter writer, Project project)
        {
            VCppProjectCompilerParameters compilerParameters = project.HasParameters("cpp-compiler")
                                                                   ? project.GetParameters<VCppProjectCompilerParameters>("cpp-compiler")
                                                                   : new VCppProjectCompilerParameters(Suite);

            compilerParameters.FillProjectSpecificMissingInfo(project, GetCLIMode(project), targetDir as LocalFileSystemDirectory);

            writer.WriteStartElement("ClCompile");
            compilerParameters.ToVcxprojProperties(writer);

            if (project.GetSourceSet("cpp").Files.Any(file => Path.GetFileName(file) == "stdafx.cpp"))
                writer.WriteElementString("PrecompiledHeader", "Use");

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
                                      ToProjectRelativePath(project, GetOutputPath(targetDir, project), "cpp") + Path.DirectorySeparatorChar);
            writer.WriteElementString("IntDir",
                                      ToProjectRelativePath(project, Path.Combine(Suite.SuiteRoot.GetRelativePath(targetDir), "tmp", project.Module.Name, project.Name), "cpp") + Path.DirectorySeparatorChar);

            var manifestParameters = GetManifestParameters(project);
            writer.WriteElementString("EmbedManifest", XmlConvert.ToString(manifestParameters.EmbedManifest));
            writer.WriteElementString("GenerateManifest", XmlConvert.ToString(manifestParameters.GenerateManifest));
        }
    }
}