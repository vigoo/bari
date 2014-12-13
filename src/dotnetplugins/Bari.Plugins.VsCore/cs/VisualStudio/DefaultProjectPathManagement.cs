using System;
using System.Collections.Generic;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio
{
    public class DefaultProjectPathManagement: IProjectPathManagement
    {
        private readonly IDictionary<Project, SuiteRelativePath> map = new Dictionary<Project, SuiteRelativePath>();
        private readonly object sync = new object();

        /// <summary>
        /// Gets the visual studio project file for a given project
        /// </summary>
        /// <param name="project">The project model</param>
        /// <returns>Returns the suite relative path to the project file, or <c>null</c> if this is not a visual studio project</returns>
        public SuiteRelativePath GetProjectFile(Project project)
        {
            lock (sync)
            {
                SuiteRelativePath result = null;
                map.TryGetValue(project, out result);
                return result;
            }
        }

        /// <summary>
        /// Maps a project file to a project
        /// </summary>
        /// <param name="project">The project model</param>
        /// <param name="path">Suite relative path to the visual studio project file</param>
        public void RegisterProjectFile(Project project, SuiteRelativePath path)
        {
            lock (sync)
            {
                map.Add(project, path);
            }
        }
    }
}