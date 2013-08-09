using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio
{
    /// <summary>
    /// Class for generating a Visual Studio solution file from a set of bari project models
    /// </summary>
    public class SlnGenerator
    {        
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IProjectPlatformManagement projectPlatformManagement;
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory slnDir;
        private readonly IList<Project> projects;
        private readonly IEnumerable<ISlnProject> supportedSlnProjects;
        private readonly Func<Project, IEnumerable<Project>> getProjectSolutionReferences;
        private readonly TextWriter output;

        /// <summary>
        /// Initializes the solution file generator
        /// </summary>
        /// <param name="projectGuidManagement">Project guid mapping to be used</param>
        /// <param name="projectPlatformManagement">For getting project's default platform name</param>
        /// <param name="supportedSlnProjects">All the supported SLN project implementations</param>
        /// <param name="projects">The set of projects to be added to the solution</param>
        /// <param name="output">Text writer to write the solution file</param>
        /// <param name="suiteRoot">Suite's root directory </param>
        /// <param name="slnDir">Directory where the sln is being generated </param>
        /// <param name="getProjectSolutionReferences">Function which returns all the referenced projects which are in the same solution</param>
        public SlnGenerator(IProjectGuidManagement projectGuidManagement, IProjectPlatformManagement projectPlatformManagement, IEnumerable<ISlnProject> supportedSlnProjects, IEnumerable<Project> projects, TextWriter output, IFileSystemDirectory suiteRoot, IFileSystemDirectory slnDir, Func<Project, IEnumerable<Project>> getProjectSolutionReferences)
        {
            Contract.Requires(projectGuidManagement != null);
            Contract.Requires(projectPlatformManagement != null);
            Contract.Requires(projects != null);
            Contract.Requires(output != null);
            Contract.Requires(suiteRoot != null);
            Contract.Requires(slnDir != null);
            Contract.Requires(getProjectSolutionReferences != null);
            Contract.Requires(supportedSlnProjects != null);

            this.projectGuidManagement = projectGuidManagement;
            this.projectPlatformManagement = projectPlatformManagement;
            this.projects = projects.ToList();
            this.output = output;
            this.suiteRoot = suiteRoot;
            this.slnDir = slnDir;
            this.getProjectSolutionReferences = getProjectSolutionReferences;
            this.supportedSlnProjects = supportedSlnProjects;
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
                GenerateProjectSection(project);

            output.WriteLine("Global");
            output.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            output.WriteLine("\t\tBari|Bari = Bari|Bari");
            output.WriteLine("\tEndGlobalSection");
            output.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");

            foreach (var project in projects)
            {
                string projectGuid = projectGuidManagement.GetGuid(project).ToString("B").ToUpperInvariant();
                string platform = projectPlatformManagement.GetDefaultPlatform(project);

                output.WriteLine("\t\t{0}.Bari|Bari.ActiveCfg = Bari|{1}", projectGuid, platform);
                output.WriteLine("\t\t{0}.Bari|Bari.Build.0 = Bari|{1}", projectGuid, platform);
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

        private void GenerateProjectSection(Project project)
        {
            var slnProject = supportedSlnProjects.FirstOrDefault(prj => prj.SupportsProject(project));
            if (slnProject != null)
                GenerateProjectSection(project, slnProject);
        }

        private void GenerateProjectSection(Project project, ISlnProject slnProject)
        {
            string relativeProjectFilePath = suiteRoot.GetRelativePathFrom(
                slnDir, slnProject.GetSuiteRelativeProjectFilePath(project));

            string projectGuid = projectGuidManagement.GetGuid(project).ToString("B").ToUpperInvariant();

            output.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
                             slnProject.ProjectTypeGuid, project.Name, relativeProjectFilePath, projectGuid);

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
}