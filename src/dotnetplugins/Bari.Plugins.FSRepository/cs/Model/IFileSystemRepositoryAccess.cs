using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Generic;

namespace Bari.Plugins.FSRepository.Model
{
    /// <summary>
    /// Interface for accessing the file system repository
    /// </summary>
    [ContractClass(typeof(IFileSystemRepositoryAccessContracts))]
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

        /// <summary>
        /// Lists all the files in the given directory inside the FS repository
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <returns>Returns the files in the given directory, all prefixed with the directory path</returns>
        IEnumerable<string> ListFiles(string path);
    }

    [ContractClassFor(typeof (IFileSystemRepositoryAccess))]
    abstract class IFileSystemRepositoryAccessContracts : IFileSystemRepositoryAccess
    {
        public abstract bool Exists(string path);

        public IFileSystemDirectory GetDirectory(string path)
        {
            Contract.Ensures(Contract.Result<IFileSystemDirectory>() != null);

            return null; // dummy
        }

        public abstract void Copy(string path, IFileSystemDirectory targetDir, string targetFileName);

        public abstract IEnumerable<string> ListFiles(string path);
    }
}