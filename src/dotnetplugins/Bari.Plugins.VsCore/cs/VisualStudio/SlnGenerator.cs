using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio.SolutionItems;

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
        private readonly IEnumerable<ISolutionItemProvider> solutionItemProviders;
        private readonly TextWriter output;
        private readonly string slnName;

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
        /// <param name="solutionItemProviders">List of registered solution item providers</param>
        /// <param name="slnName">Solution's unique name</param>
        public SlnGenerator(IProjectGuidManagement projectGuidManagement, IProjectPlatformManagement projectPlatformManagement, IEnumerable<ISlnProject> supportedSlnProjects, IEnumerable<Project> projects, TextWriter output, IFileSystemDirectory suiteRoot, IFileSystemDirectory slnDir, Func<Project, IEnumerable<Project>> getProjectSolutionReferences, IEnumerable<ISolutionItemProvider> solutionItemProviders, string slnName)
        {
            Contract.Requires(projectGuidManagement != null);
            Contract.Requires(projectPlatformManagement != null);
            Contract.Requires(projects != null);
            Contract.Requires(output != null);
            Contract.Requires(suiteRoot != null);
            Contract.Requires(slnDir != null);
            Contract.Requires(getProjectSolutionReferences != null);
            Contract.Requires(supportedSlnProjects != null);
            Contract.Requires(solutionItemProviders != null);

            this.projectGuidManagement = projectGuidManagement;
            this.projectPlatformManagement = projectPlatformManagement;
            this.projects = projects.ToList();
            this.output = output;
            this.suiteRoot = suiteRoot;
            this.slnDir = slnDir;
            this.getProjectSolutionReferences = getProjectSolutionReferences;
            this.solutionItemProviders = solutionItemProviders;
            this.slnName = slnName;
            this.supportedSlnProjects = supportedSlnProjects;
        }

        /// <summary>
        /// Generates the solution file
        /// </summary>
        public void Generate()
        {
            const string testProjectNode = "{6181E5C5-1B34-46C3-9917-0E6779125067}";
            const string solutionItemsNode = "{9163c076-18a2-46f8-a018-d225f8020b4f}";

            output.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            output.WriteLine("# Visual Studio 2012");

            var testProjects = new HashSet<TestProject>(projects.OfType<TestProject>());
            var startupProject = projects.FirstOrDefault(project => project.Type == ProjectType.Executable);

            if (startupProject != null)
                GenerateProjectSection(startupProject);

            if (testProjects.Count > 0)
            {
                output.WriteLine("Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"_Tests\", \"_Tests\", \"{0}\"", testProjectNode);
                output.WriteLine("EndProject");
            }

            var modules = new HashSet<Module>(projects.Select(prj => prj.Module));
            foreach (var module in modules)
                GenerateModuleNode(module);

            foreach (var project in projects)
                if (project != startupProject)
                    GenerateProjectSection(project);

            GenerateSolutionItems(solutionItemsNode);

            output.WriteLine("Global");
            output.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            output.WriteLine("\t\tBari|Bari = Bari|Bari");
            output.WriteLine("\tEndGlobalSection");
            output.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");

            foreach (var project in projects)
                GenerateProjectConfiguration(project);

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

            GenerateNestedProjects(testProjects);

            output.WriteLine("\tEndGlobalSection");

            output.WriteLine("EndGlobal");
        }

        private void GenerateNestedProjects(HashSet<TestProject> testProjects)
        {
            foreach (var project in projects)
            {
                if (!testProjects.Contains(project))
                {
                    var slnProject = supportedSlnProjects.FirstOrDefault(prj => prj.SupportsProject(project));
                    if (slnProject != null)
                    {
                        string projectGuid = projectGuidManagement.GetGuid(project).ToString("B").ToUpperInvariant();
                        string moduleGuid = projectGuidManagement.GetGuid(project.Module).ToString("B").ToUpperInvariant();
                        output.WriteLine("\t\t{0} = {1}", projectGuid, moduleGuid);
                    }
                }
            }
        }

        private void GenerateSolutionItems(string solutionItemsNode)
        {
            output.WriteLine(
                "Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"_Solution Items\", \"_Solution Items\", \"{0}\"",
                solutionItemsNode);
            output.WriteLine("\tProjectSection(SolutionItems) = preProject");

            foreach (var solutionItemProvider in solutionItemProviders)
            {
                foreach (var item in solutionItemProvider.GetItems(slnName))
                {
                    output.WriteLine("\t\t{0} = {0}", (string)item);
                }
            }

            output.WriteLine("\tEndProjectSection");
            output.WriteLine("EndProject");
        }

        private void GenerateModuleNode(Module module)
        {
            var guid = projectGuidManagement.GetGuid(module).ToString("B").ToUpperInvariant();
            output.WriteLine("Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"{1}\", \"{1}\", \"{0}\"", guid, module.Name);
            output.WriteLine("EndProject");
        }

        private void GenerateProjectConfiguration(Project project)
        {
            string projectGuid = projectGuidManagement.GetGuid(project).ToString("B").ToUpperInvariant();
            string platform = projectPlatformManagement.GetDefaultPlatform(project);

            output.WriteLine("\t\t{0}.Bari|Bari.ActiveCfg = Bari|{1}", projectGuid, platform);
            output.WriteLine("\t\t{0}.Bari|Bari.Build.0 = Bari|{1}", projectGuid, platform);
        }

        private void GenerateProjectSection(Project project)
        {
            var slnProject = supportedSlnProjects.FirstOrDefault(prj => prj.SupportsProject(project));
            if (slnProject != null)
                GenerateProjectSection(project, slnProject);
        }

        private void GenerateProjectSection(Project project, ISlnProject slnProject)
        {
            var projectPath = slnProject.GetSuiteRelativeProjectFilePath(project);
            string relativeProjectFilePath = suiteRoot.GetRelativePathFrom(slnDir, projectPath);

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