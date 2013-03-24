using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
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
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory slnDir;
        private readonly IList<Project> projects;
        private readonly Func<Project, IEnumerable<Project>> getProjectSolutionReferences;
        private readonly TextWriter output;

        /// <summary>
        /// Initializes the solution file generator
        /// </summary>
        /// <param name="projectGuidManagement">Project guid mapping to be used</param>
        /// <param name="projects">The set of projects to be added to the solution</param>
        /// <param name="output">Text writer to write the solution file</param>
        /// <param name="suiteRoot">Suite's root directory </param>
        /// <param name="slnDir">Directory where the sln is being generated </param>
        /// <param name="getProjectSolutionReferences">Function which returns all the referenced projects which are in the same solution</param>
        public SlnGenerator(IProjectGuidManagement projectGuidManagement, IEnumerable<Project> projects, TextWriter output, IFileSystemDirectory suiteRoot, IFileSystemDirectory slnDir, Func<Project, IEnumerable<Project>> getProjectSolutionReferences)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.projects = projects.ToList();
            this.output = output;
            this.suiteRoot = suiteRoot;
            this.slnDir = slnDir;
            this.getProjectSolutionReferences = getProjectSolutionReferences;
        }

        /// <summary>
        /// Generates the solution file
        /// </summary>
        public void Generate()
        {
            const string testProjectNode = "{6181E5C5-1B34-46C3-9917-0E6779125067}";

            output.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            output.WriteLine("# Visual Studio 2012");

            var testProjects = new HashSet<TestProject>(projects.OfType<TestProject>());
            if (testProjects.Count > 0)
            {
                output.WriteLine("Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"Tests\", \"Tests\", \"{0}\"", testProjectNode);
                output.WriteLine("EndProject");
            }

            foreach (var project in projects)
            {
                if (project.HasNonEmptySourceSet("cs"))
                {
                    string relativeCsprojPath = suiteRoot.GetRelativePathFrom(
                        slnDir, GetSuiteRelativeCsprojPath(project));

                    string projectGuid = projectGuidManagement.GetGuid(project).ToString("B").ToUpperInvariant();

                    output.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
                                     csprojGuid, project.Name, relativeCsprojPath, projectGuid);

                    var projectDeps = new HashSet<Project>(getProjectSolutionReferences(project));
                    if (projectDeps.Count > 0)
                    {
                        output.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
                        foreach (var dependentProject in projectDeps)
                        {
                            var depGuid = projectGuidManagement.GetGuid(dependentProject).ToString("B").ToUpperInvariant();
                            output.WriteLine("\t\t{0} = {0}", depGuid);
                        }
                        output.WriteLine("\tEndProjectSection");
                    }

                    output.WriteLine("EndProject");
                }
            }

            output.WriteLine("Global");
            output.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            output.WriteLine("\t\tBari|Bari = Bari|Bari");
            output.WriteLine("\tEndGlobalSection");
            output.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            
            foreach (var project in projects)
            {
                string projectGuid = projectGuidManagement.GetGuid(project).ToString("B").ToUpperInvariant();

                output.WriteLine("\t\t{0}.Bari|Bari.ActiveCfg = Bari|Bari", projectGuid);
                output.WriteLine("\t\t{0}.Bari|Bari.Build.0 = Bari|Bari", projectGuid);
            }
            
            output.WriteLine("\tEndGlobalSection");
            output.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
            output.WriteLine("\t\tHideSolutionNode = FALSE");
            output.WriteLine("\tEndGlobalSection");

            output.WriteLine("\tGlobalSection(NestedProjects) = preSolution");
            foreach (var testProject in testProjects)
            {
                string projectGuid = projectGuidManagement.GetGuid(testProject).ToString("B").ToUpperInvariant();
                output.WriteLine("\t\t{0} = {1}", projectGuid, testProjectNode);
            }
            output.WriteLine("\tEndGlobalSection");

            output.WriteLine("EndGlobal");
        }

        private string GetSuiteRelativeCsprojPath(Project project)
        {
            string projectFileName = project.Name + ".csproj";
            return Path.Combine(suiteRoot.GetRelativePath(project.RootDirectory.GetChildDirectory("cs")), projectFileName);
        }
    }
}