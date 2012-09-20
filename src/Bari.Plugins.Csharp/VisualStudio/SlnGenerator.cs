using System;
using System.Collections.Generic;
using System.IO;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio
{
    /// <summary>
    /// Class for generating a Visual C# project file from a set of bari project models
    /// </summary>
    public class SlnGenerator
    {
        private const string csprojGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IEnumerable<Project> projects;
        private readonly TextWriter output;

        /// <summary>
        /// Initializes the solution file generator
        /// </summary>
        /// <param name="projectGuidManagement">Project guid mapping to be used</param>
        /// <param name="projects">The set of projects to be added to the solution</param>
        /// <param name="output">Text writer to write the solution file</param>
        public SlnGenerator(IProjectGuidManagement projectGuidManagement, IEnumerable<Project> projects, TextWriter output)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.projects = projects;
            this.output = output;
        }

        /// <summary>
        /// Generates the solution file
        /// </summary>
        public void Generate()
        {
            output.WriteLine("Microsoft Visual Studio Solution File, Format Version 11.00");
            output.WriteLine("# Visual Studio 2010");

            foreach (var project in projects)
            {
                if (project.HasNonEmptySourceSet("cs"))
                {
                    string projectFileName = project.Name + ".csproj";
                    string projectGuid = projectGuidManagement.GetGuid(project).ToString("B");

                    output.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
                                     csprojGuid, project.Name, projectFileName, projectGuid);
                    output.WriteLine("EndProject");
                }
            }
        }
    }
}