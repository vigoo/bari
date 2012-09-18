using System.Diagnostics.Contracts;
using System.IO;
using Bari.Core.Generic;

namespace Bari.Core.Model.Discovery
{
    /// <summary>
    /// A <see cref="ISuiteExplorer"/> implementation which discovers modules and projects
    /// by traversing the file system starting from the suite's root directory.
    /// </summary>
    public class ModuleProjectDiscovery: ISuiteExplorer
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
            if (srcDir != null)
            {
                foreach (var moduleName in srcDir.ChildDirectories)
                {
                    Module module = suite.GetModule(moduleName);

                    var moduleDir = srcDir.GetChildDirectory(moduleName);
                    foreach (var projectName in moduleDir.ChildDirectories)
                    {
                        Project project = module.GetProject(projectName);
                        DiscoverProjectSources(project, moduleDir.GetChildDirectory(projectName));
                    }
                }
            }
        }

        /// <summary>
        /// Goes through all the subdirectories of a project and interprets them as source sets
        /// </summary>
        /// <param name="project">The project to be extended with source sets</param>
        /// <param name="projectDir">The project's directory</param>
        private void DiscoverProjectSources(Project project, IFileSystemDirectory projectDir)
        {
            foreach (var sourceSetName in projectDir.ChildDirectories)
            {
                var sourceSet = project.GetSourceSet(sourceSetName);
                AddAllFiles(sourceSet, projectDir.GetChildDirectory(sourceSetName));
            }
        }

        /// <summary>
        /// Recursively adds every file in a given directory to a source set (<see cref="SourceSet"/>)
        /// </summary>
        /// <param name="target">The target source set to be extended</param>
        /// <param name="dir">The root directory for the operation</param>
        private void AddAllFiles(SourceSet target, IFileSystemDirectory dir)
        {
            foreach (var fileName in dir.Files)
            {
                target.Add(new SuiteRelativePath(Path.Combine(suiteRoot.GetRelativePath(dir), fileName)));
            }

            foreach (var childDirectory in dir.ChildDirectories)
            {
                AddAllFiles(target, dir.GetChildDirectory(childDirectory));
            }
        }
    }
}