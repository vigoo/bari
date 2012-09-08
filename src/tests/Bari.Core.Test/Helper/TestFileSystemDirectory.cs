using System.Collections.Generic;
using System.Linq;
using Bari.Core.Generic;

namespace Bari.Core.Test.Helper
{
    public class TestFileSystemDirectory: IFileSystemDirectory
    {
        private readonly string name;
        private readonly IList<TestFileSystemDirectory> childDirectories = new List<TestFileSystemDirectory>();

        public string Name
        {
            get { return name; }
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

        public TestFileSystemDirectory(string name, params TestFileSystemDirectory[] childDirs)
        {
            this.name = name;
            foreach (var child in childDirs)
                childDirectories.Add(child);
        }
    }
}