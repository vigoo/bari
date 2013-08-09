using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio
{
    /// <summary>
    /// Interface for getting the default platform name to be used in solution files and MSBuild files
    /// </summary>
    public interface IProjectPlatformManagement
    {
        /// <summary>
        /// Gets the platform name for a given project
        /// </summary>
        /// <param name="project">Project to get its default platform name</param>
        /// <returns>Returns a platform name</returns>
        string GetDefaultPlatform(Project project);
    }
}