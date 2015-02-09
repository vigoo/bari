using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio
{
    /// <summary>
    /// Default implementation for the <see cref="IProjectGuidManagement"/> interface
    /// </summary>
    public class DefaultProjectGuidManagement: IProjectGuidManagement
    {
        private readonly Lazy<IFileSystemDirectory> cacheRoot;
        private readonly Suite suite;
        private readonly object sync = new object();
        private bool isInitialized;
        private readonly IDictionary<Project, Guid> projectMap = new Dictionary<Project, Guid>();
        private readonly IDictionary<Module, Guid> moduleMap = new Dictionary<Module, Guid>();

        /// <summary>
        /// Initializes the GUID management service
        /// </summary>
        /// <param name="cacheRoot">Cache root directory</param>
        /// <param name="suite">Active suite</param>
        public DefaultProjectGuidManagement([CacheRoot] Lazy<IFileSystemDirectory> cacheRoot, Suite suite)
        {
            Contract.Requires(cacheRoot != null);
            Contract.Requires(suite != null);

            this.cacheRoot = cacheRoot;
            this.suite = suite;
        }        

        /// <summary>
        /// Gets the GUID associated with the given project
        /// </summary>
        /// <param name="project">The bari project model</param>
        /// <returns>Always returns the same GUID for the same project within one process execution.</returns>
        public Guid GetGuid(Project project)
        {
            lock(sync)
            {
                if (!isInitialized)
                    InitializeFromCache();

                Guid result;
                if (!projectMap.TryGetValue(project, out result))
                {
                    result = Guid.NewGuid();
                    projectMap.Add(project, result);

                    SaveToCache();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the GUID associated with the given module
        /// </summary>
        /// <param name="module">The bari module model</param>
        /// <returns>Always returns the same GUD for the same module within one process execution</returns>
        public Guid GetGuid(Module module)
        {
            lock (sync)
            {
                if (!isInitialized)
                    InitializeFromCache();

                Guid result;
                if (!moduleMap.TryGetValue(module, out result))
                {
                    result = Guid.NewGuid();
                    moduleMap.Add(module, result);

                    SaveToCache();
                }

                return result;
            }
        }

        private void InitializeFromCache()
        {
            if (cacheRoot.Value.Exists("guids"))
            {
                using (var reader = cacheRoot.Value.ReadTextFile("guids"))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        string[] parts = line.Split('=');
                        var name = parts[0];

                        if (name.StartsWith("module "))
                        {
                            var module = FindModule(name.Substring(7));
                            if (module != null)
                                moduleMap.Add(module, Guid.Parse(parts[1]));
                        }
                        else
                        {
                            var project = FindProject(name);
                            if (project != null)
                                projectMap.Add(project, Guid.Parse(parts[1]));
                        }

                        line = reader.ReadLine();
                    }
                }
            }

            isInitialized = true;
        }

        private Project FindProject(string moduleAndProject)
        {
            return (from module in suite.Modules
                    where moduleAndProject.StartsWith(module.Name + '.', StringComparison.InvariantCultureIgnoreCase)
                    let projectName = moduleAndProject.Substring(module.Name.Length + 1)
                    select module.GetProjectOrTestProject(projectName)).FirstOrDefault();
        }

        private Module FindModule(string module)
        {
            return suite.HasModule(module) ? suite.GetModule(module) : null;
        }

        private void SaveToCache()
        {
            using (var writer = cacheRoot.Value.CreateTextFile("guids"))
            {
                foreach (var pair in projectMap)
                {
                    string key = pair.Key.Module.Name + "." + pair.Key.Name;
                    string value = pair.Value.ToString("B");
                    writer.WriteLine("{0}={1}", key, value);
                }

                foreach (var pair in moduleMap)
                {
                    string key = "module " + pair.Key.Name;
                    string value = pair.Value.ToString("B");
                    writer.WriteLine("{0}={1}", key, value);
                }
            }
        }
    }
}