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
        private readonly IDictionary<Project, Guid> map = new Dictionary<Project, Guid>();

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
                if (!map.TryGetValue(project, out result))
                {
                    result = Guid.NewGuid();
                    map.Add(project, result);

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
                        var project = FindProject(parts[0]);
                        if (project != null)
                            map.Add(project, Guid.Parse(parts[1]));

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

        private void SaveToCache()
        {
            using (var writer = cacheRoot.Value.CreateTextFile("guids"))
            {
                foreach (var pair in map)
                {
                    string key = pair.Key.Module.Name + "." + pair.Key.Name;
                    string value = pair.Value.ToString("B");
                    writer.WriteLine("{0}={1}", key, value);
                }
            }
        }
    }
}