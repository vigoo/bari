using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bari.Core.Generic
{
    /// <summary>
    /// Default implementation of <see cref="IFileSystemDirectory"/>, using the .NET 
    /// file operations.
    /// </summary>
    public class LocalFileSystemDirectory: IFileSystemDirectory, IEquatable<LocalFileSystemDirectory>
    {
        private readonly string path;

        /// <summary>
        /// Gets the local file system path represented by this directory
        /// </summary>
        public string AbsolutePath
        {
            get { return path; }
        }

        /// <summary>
        /// Constructs the directory abstraction
        /// </summary>
        /// <param name="path">Absolute path to the directory</param>
        public LocalFileSystemDirectory(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Enumerates all the files within the directory by their names
        /// </summary>
        public IEnumerable<string> Files
        {
            get 
            {
                return Directory.EnumerateFiles(path).Select(Path.GetFileName); 
            }
        }

        /// <summary>
        /// Enumerates all the child directories of the directory by their names
        /// 
        /// <para>Use <see cref="IFileSystemDirectory.GetChildDirectory"/> to get any of these children.</para>
        /// </summary>
        public IEnumerable<string> ChildDirectories
        {
            get { return Directory.EnumerateDirectories(path).Select(Path.GetFileName); }
        }

        /// <summary>
        /// Gets interface for a given child directory
        /// </summary>
        /// <param name="name">Name of the child directory</param>
        /// <returns>Returns either a directory abstraction or <c>null</c> if it does not exists.</returns>
        public IFileSystemDirectory GetChildDirectory(string name)
        {
            string childPath = Path.Combine(path, name);
            if (Directory.Exists(childPath))
                return new LocalFileSystemDirectory(childPath);
            else
                return null;
        }

        /// <summary>
        /// Gets the relative path from this directory to another directory (in any depth)
        ///         /// <para>If the given argument is not a child of this directory, an <see cref="ArgumentException"/>will
        /// be thrown.</para>
        /// </summary>
        /// <param name="childDirectory">The child directory to get path to</param>
        /// <returns>Returns the path</returns>
        public string GetRelativePath(IFileSystemDirectory childDirectory)
        {
            var localChildDirectory = childDirectory as LocalFileSystemDirectory;
            if (localChildDirectory == null)            
                throw new ArgumentException("Only LocalFileSystemDirectory objects are supported", "childDirectory");

            var childPath = localChildDirectory.path;
            if (!childPath.StartsWith(path))
                throw new ArgumentException("The argument is not a child directory of this directory", "childDirectory");

            return childPath.Substring(path.Length).TrimStart(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Creates a child directory if it does not exist yet
        /// </summary>
        /// <param name="name">Name of the child directory</param>
        /// <returns>Returns the directory abstraction of the new (or already existing) directory</returns>
        public IFileSystemDirectory CreateDirectory(string name)
        {
            string childPath = Path.Combine(path, name);
            Directory.CreateDirectory(childPath);

            return new LocalFileSystemDirectory(childPath);
        }

        /// <summary>
        /// Creates a new text file with a text writer in this directory
        /// </summary>
        /// <param name="name">Name of the new file</param>
        /// <returns>Returns the text writer to be used to write the contents of the file.</returns>
        public TextWriter CreateTextFile(string name)
        {
            string childPath = Path.Combine(path, name);

            return File.CreateText(childPath);
        }

        /// <summary>
        /// Creates a new binary file in this directory
        /// </summary>
        /// <param name="name">Name of the new file</param>
        /// <returns>Returns the stream to be used to write the contents of the file.</returns>
        public Stream CreateBinaryFile(string name)
        {
            string childPath = Path.Combine(path, name);
            if (Path.GetDirectoryName(childPath) != path)
                throw new ArgumentException("Must be a file name not a relative path", "name");

            return File.Create(childPath);
        }

        /// <summary>
        /// Reads an existing binary file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the stream belonging to the given file</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public Stream ReadBinaryFile(string relativePath)
        {
            string absolutePath = Path.Combine(path, relativePath);
            if (!File.Exists(absolutePath))
                throw new ArgumentException("File does not exists" + absolutePath, "relativePath");

            return new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Reads an existing text file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the text reader belonging to the given file</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public TextReader ReadTextFile(string relativePath)
        {
            string absolutePath = Path.Combine(path, relativePath);
            if (!File.Exists(absolutePath))
                throw new ArgumentException("File does not exists: " + absolutePath, "relativePath");

            return new StreamReader(absolutePath, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the last modification's date for a given file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the last modified date.</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public DateTime GetLastModifiedDate(string relativePath)
        {
            string absolutePath = Path.Combine(path, relativePath);
            if (!File.Exists(absolutePath))
                throw new ArgumentException("File does not exists", "relativePath");

            return File.GetLastWriteTimeUtc(absolutePath);
        }

        /// <summary>
        /// Gets the size of the given file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the file size in bytes</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public long GetFileSize(string relativePath)
        {
            string absolutePath = Path.Combine(path, relativePath);
            if (!File.Exists(absolutePath))
                throw new ArgumentException("File does not exists", "relativePath");

            var info = new FileInfo(absolutePath);
            return info.Length;
        }

        /// <summary>
        /// Deletes a child directory
        /// </summary>
        /// <param name="name">Name of the directory</param>
        public void DeleteDirectory(string name)
        {
            Directory.Delete(Path.Combine(path, name), recursive: true);
        }

        /// <summary>
        /// Deletes a file from this directory
        /// </summary>
        /// <param name="name">Name of the file</param>
        public void DeleteFile(string name)
        {
            File.Delete(Path.Combine(path, name));
        }

        /// <summary>
        /// Deletes the directory
        /// </summary>
        public void Delete()
        {
            Directory.Delete(path, recursive: true);
        }

        /// <summary>
        /// Partially deletes the directory, based on a filter function
        /// </summary>
        /// <param name="filter">Filter function, a relative path, and if it returns <c>true</c>, the file/directory is going to be deleted</param>
        public void Delete(Func<string, bool> filter)
        {
            Delete(filter, String.Empty);
        }

        private void Delete(Func<string, bool> filter, string prefix)
        {
            foreach (var child in ChildDirectories)
            {                
                var wrapper = new LocalFileSystemDirectory(Path.Combine(path, child));
                wrapper.Delete(filter, Path.Combine(prefix, child));
            }

            foreach (var file in Files)
            {
                if (filter(Path.Combine(prefix, file)))
                    DeleteFile(file);
            }

            if (!ChildDirectories.Any() &&
                !Files.Any())
            {
                if (filter(prefix))
                    Delete();
            }
        }

        /// <summary>
        /// Checks whether a file exists at the given relative path
        /// </summary>
        /// <param name="relativePath">Path to the file to check, relative to this directory</param>
        /// <returns>Returns <c>true</c> if the file exists.</returns>
        public bool Exists(string relativePath)
        {
            return File.Exists(Path.Combine(path, relativePath)) ||
                   Directory.Exists(Path.Combine(path, relativePath));
        }

        /// <summary>
        /// Remake the directory if it has been deleted
        /// </summary>
        public void Remake()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Copy a file to a target directory
        /// </summary>
        /// <param name="name">Name of the file</param>
        /// <param name="target">Target file system directory</param>
        /// <param name="targetName">Name (relative path) in the target directory</param>
        public void CopyFile(string name, IFileSystemDirectory target, string targetName)
        {
            var localTarget = target as LocalFileSystemDirectory;
            if (localTarget != null)
            {
                var targetSubdir = Path.GetDirectoryName(targetName);

                if (!String.IsNullOrWhiteSpace(targetSubdir))
                {
                    var absoluteTargetDir = Path.Combine(localTarget.AbsolutePath, targetSubdir);
                    if (!Directory.Exists(absoluteTargetDir))
                        Directory.CreateDirectory(absoluteTargetDir);
                }

                File.Copy(Path.Combine(path, name), Path.Combine(localTarget.AbsolutePath, targetName), overwrite: true);
            }
            else
            {
                using (var sourceStream = ReadBinaryFile(name))
                using (var targetStream = target.CreateBinaryFileWithDirectories(targetName))
                    StreamOperations.Copy(sourceStream, targetStream);
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(LocalFileSystemDirectory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(path, other.path);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LocalFileSystemDirectory) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return path.GetHashCode();
        }

        public static bool operator ==(LocalFileSystemDirectory left, LocalFileSystemDirectory right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LocalFileSystemDirectory left, LocalFileSystemDirectory right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return path;
        }
    }
}