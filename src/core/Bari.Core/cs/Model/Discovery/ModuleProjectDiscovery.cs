using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Monads;
using Bari.Core.Generic;

namespace Bari.Core.Model.Discovery
{
    /// <summary>
    /// A <see cref="ISuiteExplorer"/> implementation which discovers modules and projects
    /// by traversing the file system starting from the suite's root directory.
    /// </summary>
    public class ModuleProjectDiscovery : ISuiteExplorer
    {
        private readonly IFileSystemDirectory suiteRoot;        

        /// <summary>
        /// Constructs the suite discovery implementation
        /// </summary>
        /// <param name="suiteRoot">File system directory representing the suite's root</param>
        public ModuleProjectDiscovery([SuiteRoot] IFileSystemDirectory suiteRoot)
        {
            Contract.Requires(suiteRoot != null);

            this.suiteRoot = suiteRoot;
        }

        /// <summary>
        /// Extends suite model with discovered information based on bari conventions
        /// </summary>        
        /// <param name="suite">The suite model to be extended with discoveries</param>
        public void ExtendWithDiscoveries(Suite suite)
        {
            var srcDir = suiteRoot.GetChildDirectory("src");
            srcDir.With(s => s.ChildDirectories.Do(
                moduleName =>
                {
                    Module module = suite.GetModule(moduleName);

                    var moduleDir = srcDir.GetChildDirectory(moduleName);
                    foreach (var projectName in moduleDir.ChildDirectories)
                    {
                        if (projectName.Equals("tests", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // This is the special subdirectory for test projects
                            var testsDir = moduleDir.GetChildDirectory(projectName);
                            foreach (var testProjectName in testsDir.ChildDirectories)
                            {
                                var testProject = module.GetTestProject(testProjectName);
                                DiscoverProjectSources(testProject, testsDir.GetChildDirectory(testProjectName), suite.SourceSetIgnoreLists);
                            }
                        }
                        else
                        {
                            // This is a project directory

                            Project project = module.GetProject(projectName);
                            DiscoverProjectSources(project, moduleDir.GetChildDirectory(projectName), suite.SourceSetIgnoreLists);
                        }
                    }
                }));
        }

        /// <summary>
        /// Goes through all the subdirectories of a project and interprets them as source sets
        /// </summary>
        /// <param name="project">The project to be extended with source sets</param>
        /// <param name="projectDir">The project's directory</param>
        /// <param name="ignoreLists">The suite's source set ignore lists</param>
        private void DiscoverProjectSources(Project project, IFileSystemDirectory projectDir, SourceSetIgnoreLists ignoreLists)
        {
            foreach (var sourceSetName in projectDir.ChildDirectories)
            {
                var sourceSet = project.GetSourceSet(sourceSetName);
                AddAllFiles(sourceSet, projectDir.GetChildDirectory(sourceSetName), ignoreLists.Get(new SourceSetType(sourceSetName)));
            }
        }

        /// <summary>
        /// Recursively adds every file in a given directory to a source set (<see cref="SourceSet"/>)
        /// </summary>
        /// <param name="target">The target source set to be extended</param>
        /// <param name="dir">The root directory for the operation</param>
        /// <param name="ignoreList">Ignore list for the target source set</param>
        private void AddAllFiles(SourceSet target, IFileSystemDirectory dir, SourceSetIgnoreList ignoreList)
        {
            foreach (var fileName in dir.Files)
            {
                var suiteRelativePath = new SuiteRelativePath(Path.Combine(suiteRoot.GetRelativePath(dir), fileName));
                if (!ignoreList.IsIgnored(suiteRelativePath))
                {
                    target.Add(suiteRelativePath);
                }
            }

            foreach (var childDirectory in dir.ChildDirectories)
            {
                AddAllFiles(target, dir.GetChildDirectory(childDirectory), ignoreList);
            }
        }
    }
}