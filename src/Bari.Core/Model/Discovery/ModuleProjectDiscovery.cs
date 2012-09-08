using System.Diagnostics.Contracts;
using Bari.Core.Generic;

namespace Bari.Core.Model.Discovery
{
    /// <summary>
    /// A <see cref="ISuiteDiscovery"/> implementation which discovers modules and projects
    /// by traversing the file system starting from the suite's root directory.
    /// </summary>
    public class ModuleProjectDiscovery: ISuiteDiscovery
    {
        private readonly IFileSystemDirectory suiteRoot;
        private readonly Suite suite;

        /// <summary>
        /// Constructs the suite discovery implementation
        /// </summary>
        /// <param name="suiteRoot">File system directory representing the suite's root</param>
        /// <param name="suite">The suite model to fill</param>
        public ModuleProjectDiscovery(IFileSystemDirectory suiteRoot, Suite suite)
        {
            Contract.Requires(suiteRoot != null);
            Contract.Requires(suite != null);

            this.suiteRoot = suiteRoot;
            this.suite = suite;
        }

        /// <summary>
        /// Extends suite model with discovered information based on bari conventions
        /// </summary>        
        public void ExtendWithDiscoveries()
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
                    }
                }
            }
        }
    }
}