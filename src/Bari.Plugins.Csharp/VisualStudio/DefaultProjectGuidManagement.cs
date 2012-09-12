using System;
using System.Collections.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio
{
    /// <summary>
    /// Default implementation for the <see cref="IProjectGuidManagement"/> interface
    /// </summary>
    public class DefaultProjectGuidManagement: IProjectGuidManagement
    {
        private readonly object sync = new object();
        private readonly IDictionary<Project, Guid> map = new Dictionary<Project, Guid>();

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
                }

                return result;
            }
        }
    }
}