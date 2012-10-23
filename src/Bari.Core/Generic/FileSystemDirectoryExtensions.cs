using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Bari.Core.Generic
{
    /// <summary>
    /// Extension methods for the <see cref="IFileSystemDirectory"/> interface
    /// </summary>
    public static class FileSystemDirectoryExtensions
    {
        /// <summary>
        /// Gets a child directory and optionally creates it if missing
        /// </summary>
        /// <param name="root">The root directory</param>
        /// <param name="childName">The name of the direct child directory</param>
        /// <param name="createIfMissing">If <c>true</c>, the child directory will be created if does not exist</param>
        /// <exception cref="ArgumentException">Thrown if the child directory does not exist and <c>createIfMissing</c> parameter is false</exception>
        /// <returns>Returns the file system abstraction of the child directory</returns>
        public static IFileSystemDirectory GetChildDirectory(this IFileSystemDirectory root, string childName, bool createIfMissing)
        {
            Contract.Requires(root != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(childName));
            Contract.Ensures(Contract.Result<IFileSystemDirectory>() != null);

            if (root.ChildDirectories.Any(name => name.Equals(childName, StringComparison.InvariantCultureIgnoreCase)))
            {
                return root.GetChildDirectory(childName);
            }
            else
            {
                if (createIfMissing)
                    return root.CreateDirectory(childName);
                else
                    throw new ArgumentException("The argument is not a child directory of this directory", "childName");
            }
        }

        /// <summary>
        /// Creates a binary file in the given directory, or in a subdirectory of it.
        /// If the subdirectory does not exist, it will be created.
        /// </summary>
        /// <param name="root">The root directory</param>
        /// <param name="relativePath">Relative path of the file to be created</param>
        /// <returns>Returns the stream for writing the binary file.</returns>
        public static Stream CreateBinaryFileWithDirectories(this IFileSystemDirectory root, string relativePath)
        {
            Contract.Requires(root != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(relativePath));

            string dirName = Path.GetDirectoryName(relativePath);
            if (!String.IsNullOrWhiteSpace(dirName))
            {
                var subdir = root.GetChildDirectory(dirName, createIfMissing: true);
                return subdir.CreateBinaryFile(Path.GetFileName(relativePath));
            }
            else
            {
                return root.CreateBinaryFile(relativePath);
            }
        }
    }
}