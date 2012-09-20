﻿using System.IO;
using System.Text;
using System.Xml;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio
{
    /// <summary>
    /// Class for generating a Visual C# project file from a bari project model
    /// </summary>
    public class CsprojGenerator
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly Project project;
        private readonly XmlWriter writer;
        private readonly string pathToSuiteRoot;

        /// <summary>
        /// Initializes the project file generator
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to be used</param>
        /// <param name="pathToSuiteRoot">Relative path to be used in generate project file to get to the suite root directory</param>
        /// <param name="project">The project model to work on</param>
        /// <param name="output">Output where the csproj file will be written</param>
        public CsprojGenerator(IProjectGuidManagement projectGuidManagement, string pathToSuiteRoot, Project project, TextWriter output)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.pathToSuiteRoot = pathToSuiteRoot;
            this.project = project;

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
            writer.WriteElementString("ProjectGuid", projectGuidManagement.GetGuid(project).ToString());
            writer.WriteEndElement();
        }

        private void WriteReferences()
        {
            writer.WriteStartElement("ItemGroup");
            // <Reference ...>
            writer.WriteEndElement();
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
            return Path.Combine(pathToSuiteRoot, suiteRelativePath);
        }
    }
}