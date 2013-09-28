using Bari.Core.Generic;

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

        /// <summary>
        /// Gets the directory for a given path (the path's file name part is not used)
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Returns an abstract directory interface</returns>
        IFileSystemDirectory GetDirectory(string path);

        /// <summary>
        /// Copies a file from the FS repository to a given target directory
        /// </summary>
        /// <param name="path">Path to the file in the FS repository</param>
        /// <param name="targetDir">Target directory</param>
        /// <param name="targetFileName">Target file name</param>
        void Copy(string path, IFileSystemDirectory targetDir, string targetFileName);
    }
}