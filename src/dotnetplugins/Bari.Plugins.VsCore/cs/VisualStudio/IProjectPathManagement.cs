using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio
{
    /// <summary>
    /// Interface for project-project path mappers
    /// </summary>
    public interface IProjectPathManagement
    {
        /// <summary>
        /// Gets the visual studio project file for a given project
        /// </summary>
        /// <param name="project">The project model</param>
        /// <returns>Returns the suite relative path to the project file, or <c>null</c> if this is not a visual studio project</returns>
        SuiteRelativePath GetProjectFile(Project project);

        /// <summary>
        /// Maps a project file to a project
        /// </summary>
        /// <param name="project">The project model</param>
        /// <param name="path">Suite relative path to the visual studio project file</param>
        void RegisterProjectFile(Project project, SuiteRelativePath path);
    }
}