using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio
{
    /// <summary>
    /// Default implementation for the <see cref="IProjectGuidManagement"/> interface
    /// </summary>
    public class DefaultProjectGuidManagement: IProjectGuidManagement
    {
        private readonly IFileSystemDirectory cacheRoot;
        private readonly Suite suite;
        private readonly object sync = new object();
        private readonly IDictionary<Project, Guid> map = new Dictionary<Project, Guid>();

        /// <summary>
        /// Initializes the GUID management service
        /// </summary>
        /// <param name="cacheRoot">Cache root directory</param>
        /// <param name="suite">Active suite</param>
        public DefaultProjectGuidManagement([CacheRoot] IFileSystemDirectory cacheRoot, Suite suite)
        {
            Contract.Requires(cacheRoot != null);
            Contract.Requires(suite != null);

            this.cacheRoot = cacheRoot;
            this.suite = suite;

            InitializeFromCache();
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
            if (cacheRoot.Exists("guids"))
            {
                using (var reader = cacheRoot.ReadTextFile("guids"))
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
        }

        private Project FindProject(string moduleAndProject)
        {
            foreach (var module in suite.Modules)
            {
                if (moduleAndProject.StartsWith(module.Name + '.', StringComparison.InvariantCultureIgnoreCase))
                {
                    string projectName = moduleAndProject.Substring(module.Name.Length + 1);
                    if (module.HasProject(projectName))
                        return module.GetProject(projectName);
                    else if (module.HasTestProject(projectName))
                        return module.GetTestProject(projectName);
                }
            }

            return null;
        }

        private void SaveToCache()
        {
            using (var writer = cacheRoot.CreateTextFile("guids"))
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