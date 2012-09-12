using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Generic;

namespace Bari.Core.Test.Helper
{
    public class TestFileSystemDirectory: IFileSystemDirectory
    {
        private readonly string name;
        private readonly IList<TestFileSystemDirectory> childDirectories = new List<TestFileSystemDirectory>();
        private readonly IList<string> files = new List<string>();

        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Enumerates all the files within the directory by their names
        /// </summary>
        public IEnumerable<string> Files
        {
            get { return files; }
        }

        public IEnumerable<string> ChildDirectories
        {
            get { return childDirectories.Select(dir => dir.Name); }
        }

        /// <summary>
        /// Gets interface for a given child directory
        /// </summary>
        /// <param name="childName">Name of the child directory</param>
        /// <returns>Returns either a directory abstraction or <c>null</c> if it does not exists.</returns>
        public IFileSystemDirectory GetChildDirectory(string childName)
        {
            return childDirectories.FirstOrDefault(d => d.name == childName);
        }

        /// <summary>
        /// Gets the relative path from this directory to another directory (in any depth)
        /// 
        /// <para>If the given argument is not a child of this directory, an <see cref="ArgumentException"/>will
        /// be thrown.</para>
        /// </summary>
        /// <param name="childDirectory">The child directory to get path to</param>
        /// <returns>Returns the path</returns>
        public string GetRelativePath(IFileSystemDirectory childDirectory)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates a child directory if it does not exist yet
        /// </summary>
        /// <param name="name">Name of the child directory</param>
        /// <returns>Returns the directory abstraction of the new (or already existing) directory</returns>
        public IFileSystemDirectory CreateDirectory(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new text file with a text writer in this directory
        /// </summary>
        /// <param name="name">Name of the new file</param>
        /// <returns>Returns the text writer to be used to write the contents of the file.</returns>
        public TextWriter CreateTextFile(string name)
        {
            throw new NotImplementedException();
        }

        public TestFileSystemDirectory(string name, params TestFileSystemDirectory[] childDirs)
        {
            this.name = name;
            foreach (var child in childDirs)
                childDirectories.Add(child);
        }
    }
}