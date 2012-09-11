using System;
using System.Collections.Generic;
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

        public TestFileSystemDirectory(string name, params TestFileSystemDirectory[] childDirs)
        {
            this.name = name;
            foreach (var child in childDirs)
                childDirectories.Add(child);
        }
    }
}