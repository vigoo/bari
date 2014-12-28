using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bari.Core.Generic;

namespace Bari.Core.Test.Helper
{
    public class TestFileSystemDirectory : IFileSystemDirectory
    {
        private readonly string name;
        private readonly IList<TestFileSystemDirectory> childDirectories = new List<TestFileSystemDirectory>();
        private IList<string> files = new List<string>();
        private readonly IDictionary<string, long> fileSizes = new Dictionary<string, long>();
        private readonly IDictionary<string, DateTime> fileDates = new Dictionary<string, DateTime>();
        private readonly IDictionary<string, string> textContents = new Dictionary<string, string>();
        private TestFileSystemDirectory parent;
        private bool isDeleted;

        public bool IsDeleted
        {
            get { return isDeleted; }
        }

        public string Name
        {
            get { return name; }
        }

        public void SetFileSize(string name, long newSize)
        {
            fileSizes[name] = newSize;
        }

        public void SetDate(string name, DateTime newDate)
        {
            fileDates[name] = newDate;
        }

        public void SetFileContents(string name, string contents)
        {
            textContents[name] = contents;
        }

        /// <summary>
        /// Enumerates all the files within the directory by their names
        /// </summary>
        public IEnumerable<string> Files
        {
            get { return files; }
            set { files = value.ToList(); }
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
            var testChild = childDirectory as TestFileSystemDirectory;
            if (testChild != null)
            {
                var dirs = new List<TestFileSystemDirectory>();
                TestFileSystemDirectory current = testChild;
                while (current != this)
                {
                    dirs.Add(current);
                    current = current.parent;
                }

                dirs.Reverse();

                var result = new StringBuilder();
                for (int i = 0; i < dirs.Count; i++)
                {
                    if (i > 0)
                        result.Append(Path.DirectorySeparatorChar);

                    result.Append(dirs[i].Name);
                }

                return result.ToString();
            }
            else
            {
                throw new NotSupportedException();
            }
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

        /// <summary>
        /// Creates a new binary file in this directory
        /// </summary>
        /// <param name="name">Name of the new file</param>
        /// <returns>Returns the stream to be used to write the contents of the file.</returns>
        public Stream CreateBinaryFile(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads an existing binary file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the stream belonging to the given file</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public Stream ReadBinaryFile(string relativePath)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(relativePath));
        }

        /// <summary>
        /// Reads an existing text file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the text reader belonging to the given file</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public TextReader ReadTextFile(string relativePath)
        {
            if (textContents.ContainsKey(relativePath))
                return new StringReader(textContents[relativePath]);
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// Gets the last modification's date for a given file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the last modified date.</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public DateTime GetLastModifiedDate(string relativePath)
        {
            DateTime val;
            if (fileDates.TryGetValue(relativePath, out val))
                return val;
            else
                return new DateTime(2000, 1, 1);
        }

        /// <summary>
        /// Gets the size of the given file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the file size in bytes</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public long GetFileSize(string relativePath)
        {
            long val;
            if (fileSizes.TryGetValue(relativePath, out val))
                return val;
            else
                return 11;
        }

        /// <summary>
        /// Deletes a child directory
        /// </summary>
        /// <param name="name">Name of the directory</param>
        public void DeleteDirectory(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a file from this directory
        /// </summary>
        /// <param name="name">Name of the file</param>
        public void DeleteFile(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the directory
        /// </summary>
        public void Delete()
        {
            isDeleted = true;
        }

        /// <summary>
        /// Partially deletes the directory, based on a filter function
        /// </summary>
        /// <param name="filter">Filter function, a relative path, and if it returns <c>true</c>, the file/directory is going to be deleted</param>
        public void Delete(Func<string, bool> filter)
        {
            isDeleted = true;
        }

        /// <summary>
        /// Checks whether a file exists at the given relative path
        /// </summary>
        /// <param name="relativePath">Path to the file to check, relative to this directory</param>
        /// <returns>Returns <c>true</c> if the file exists.</returns>
        public bool Exists(string relativePath)
        {
            return true; // ReadBinaryFile always returns a stream containing the file name
        }

        public void Remake()
        {            
        }

        public void CopyFile(string name, IFileSystemDirectory target, string targetName)
        {
            throw new NotImplementedException();
        }

        public TestFileSystemDirectory(string name, params TestFileSystemDirectory[] childDirs)
        {
            this.name = name;
            foreach (var child in childDirs)
            {
                child.parent = this;
                childDirectories.Add(child);
            }
        }
    }
}