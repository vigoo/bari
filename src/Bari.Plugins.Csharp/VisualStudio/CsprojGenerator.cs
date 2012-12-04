using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.Exceptions;

namespace Bari.Plugins.Csharp.VisualStudio
{
    /// <summary>
    /// Class for generating a Visual C# project file from a bari project model
    /// </summary>
    public class CsprojGenerator
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly Project project;
        private readonly Suite suite;
        private readonly XmlWriter writer;
        private readonly ISet<TargetRelativePath> references;
        private readonly IFileSystemDirectory targetDir;

        /// <summary>
        /// Initializes the project file generator
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to be used</param>
        /// <param name="project">The project model to work on</param>
        /// <param name="references">Paths to the external references to be included in the project</param>
        /// <param name="output">Output where the csproj file will be written</param>
        /// <param name="suite">The suite the project belongs to </param>
        /// <param name="targetDir">Build target directory </param>
        public CsprojGenerator(IProjectGuidManagement projectGuidManagement, Project project, IEnumerable<TargetRelativePath> references, TextWriter output, Suite suite, IFileSystemDirectory targetDir)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.project = project;
            this.suite = suite;
            this.targetDir = targetDir;
            this.references = new HashSet<TargetRelativePath>(references);

            var settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true,
                    NamespaceHandling = NamespaceHandling.Default,
                    Encoding = Encoding.UTF8
                };
            writer = XmlWriter.Create(output, settings);
        }

        /// <summary>
        /// Writes the output
        /// </summary>
        public void Generate()
        {
            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
            writer.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            writer.WriteAttributeString("ToolsVersion", "4.0");
            writer.WriteAttributeString("DefaultTargets", "Build");

            WriteProperties();
            WriteReferences();
            WriteSourceItems();

            writer.WriteStartElement("Import");
            writer.WriteAttributeString("Project", @"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.Flush();
        }

        private void WriteProperties()
        {
            writer.WriteStartElement("PropertyGroup");
            writer.WriteElementString("AssemblyName", project.Name);
            writer.WriteElementString("ProjectGuid", projectGuidManagement.GetGuid(project).ToString("B"));
            writer.WriteElementString("OutputPath", ToProjectRelativePath(Path.Combine(suite.SuiteRoot.GetRelativePath(targetDir), project.Module.Name)));
            writer.WriteElementString("IntermediateOutputPath", ToProjectRelativePath(Path.Combine(suite.SuiteRoot.GetRelativePath(targetDir), "tmp", project.Module.Name)));
            writer.WriteElementString("OutputType", GetOutputType(project.Type));

            WriteAppConfig();

            writer.WriteEndElement();
        }

        private string GetOutputType(ProjectType type)
        {
            switch (type)
            {
                case ProjectType.Executable:
                    return "Exe";
                case ProjectType.Library:
                    return "Library";
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private void WriteReferences()
        {
            writer.WriteStartElement("ItemGroup");

            foreach (var refPath in references)
            {
                if (((string)refPath).StartsWith("SLN!"))
                {
                    var moduleAndprojectName = ((string) refPath).Substring(4);
                    var parts = moduleAndprojectName.Split('#');
                    var moduleName = parts[0];
                    var projectName = parts[1];

                    var referredProject = suite.GetModule(moduleName).GetProject(projectName);

                    writer.WriteStartElement("ProjectReference");
                    writer.WriteAttributeString("Include", 
                        ToProjectRelativePath(Path.Combine(suite.SuiteRoot.GetRelativePath(referredProject.RootDirectory), projectName + ".csproj")));
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
                        string relativePath = ToProjectRelativePath(Path.Combine("target", refPath));

                        writer.WriteAttributeString("Include", Path.GetFileNameWithoutExtension(relativePath));
                        writer.WriteElementString("HintPath", relativePath);
                        writer.WriteElementString("SpecificVersion", "False");
                    }
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        private void WriteAppConfig()
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
                    writer.WriteElementString("AppConfig", ToProjectRelativePath(appConfigPath));
                }
            }
        }

        private void WriteSourceItems()
        {
            writer.WriteStartElement("ItemGroup");
            var sourceSet = project.GetSourceSet("cs");
            foreach (var file in sourceSet.Files)
            {
                writer.WriteStartElement("Compile");
                writer.WriteAttributeString("Include", ToProjectRelativePath(file));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private string ToProjectRelativePath(string suiteRelativePath)
        {
            return suite.SuiteRoot.GetRelativePathFrom(project.RootDirectory, suiteRelativePath);
        }
    }
}