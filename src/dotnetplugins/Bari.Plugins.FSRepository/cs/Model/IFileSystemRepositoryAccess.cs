namespace Bari.Plugins.FSRepository.Model
{
    /// <summary>
    /// Interface for accessing the file system repository
    /// </summary>
    public interface IFileSystemRepositoryAccess
    {
        /// <summary>
        /// Checks if the repository contains a valid file in the given path
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Returns <c>true</c> if the file exists.</returns>
        bool Exists(string path);
    }
}