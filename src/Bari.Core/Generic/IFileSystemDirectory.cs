using System.Collections.Generic;

namespace Bari.Core.Generic
{
    /// <summary>
    /// Abstraction of a file system directory
    /// </summary>
    public interface IFileSystemDirectory
    {
        /// <summary>
        /// Enumerates all the child directories of the directory by their names
        /// 
        /// <para>Use <see cref="GetChildDirectory"/> to get any of these children.</para>
        /// </summary>
        IEnumerable<string> ChildDirectories { get; }

        /// <summary>
        /// Gets interface for a given child directory
        /// </summary>
        /// <param name="name">Name of the child directory</param>
        /// <returns>Returns either a directory abstraction or <c>null</c> if it does not exists.</returns>
        IFileSystemDirectory GetChildDirectory(string name);
    }
}