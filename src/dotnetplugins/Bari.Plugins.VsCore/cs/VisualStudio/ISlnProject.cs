using Bari.Core.Build;
using Bari.Core.Model;
using Bari.Plugins.VsCore.Build;

namespace Bari.Plugins.VsCore.VisualStudio
{
    /// <summary>
    /// Interface for adding projects to SLN files. An implementation represents a project type which can be
    /// added to Visual Studio solutions.
    /// </summary>
    public interface ISlnProject
    {
        /// <summary>
        /// Checks if the given project is supported by this implementation
        /// </summary>
        /// <param name="project">Project to check</param>
        /// <returns>Returns <c>true</c> if the project is supported</returns>
        bool SupportsProject(Project project);

        /// <summary>
        /// Gets a suite relative path for the generated project file of the given project
        /// </summary>
        /// <param name="project">Project to get path for</param>
        /// <returns>Returns a suite relative path. Never returns <c>null</c></returns>
        string GetSuiteRelativeProjectFilePath(Project project);

        /// <summary>
        /// Gets the project type GUID
        /// </summary>
        string ProjectTypeGuid { get; }

        /// <summary>
        /// Creates a builder for a given project
        /// </summary>
        /// <returns></returns>
        ISlnProjectBuilder CreateBuilder(Project project);
    }
}