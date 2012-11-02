using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Bari.Core.Generic
{
    /// <summary>
    /// Abstraction of a file system directory
    /// </summary>
    [ContractClass(typeof(IFileSystemDirectoryContracts))]
    public interface IFileSystemDirectory
    {
        /// <summary>
        /// Enumerates all the files within the directory by their names
        /// </summary>
        IEnumerable<string> Files { get; }
            
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

        /// <summary>
        /// Gets the relative path from this directory to another directory (in any depth)
        /// 
        /// <para>If the given argument is not a child of this directory, an <see cref="ArgumentException"/>will
        /// be thrown.</para>
        /// </summary>
        /// <param name="childDirectory">The child directory to get path to</param>
        /// <returns>Returns the path</returns>
        string GetRelativePath(IFileSystemDirectory childDirectory);        

        /// <summary>
        /// Creates a child directory if it does not exist yet
        /// </summary>
        /// <param name="name">Name of the child directory</param>
        /// <returns>Returns the directory abstraction of the new (or already existing) directory</returns>
        IFileSystemDirectory CreateDirectory(string name);

        /// <summary>
        /// Creates a new text file with a text writer in this directory
        /// </summary>
        /// <param name="name">Name of the new file</param>
        /// <returns>Returns the text writer to be used to write the contents of the file.</returns>
        TextWriter CreateTextFile(string name);

        /// <summary>
        /// Creates a new binary file in this directory
        /// </summary>
        /// <param name="name">Name of the new file</param>
        /// <returns>Returns the stream to be used to write the contents of the file.</returns>
        Stream CreateBinaryFile(string name);

        /// <summary>
        /// Reads an existing binary file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the stream belonging to the given file</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        Stream ReadBinaryFile(string relativePath);

        /// <summary>
        /// Reads an existing text file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the text reader belonging to the given file</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        TextReader ReadTextFile(string relativePath);

        /// <summary>
        /// Gets the last modification's date for a given file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the last modified date.</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        DateTime GetLastModifiedDate(string relativePath);

        /// <summary>
        /// Gets the size of the given file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the file size in bytes</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        long GetFileSize(string relativePath);

        /// <summary>
        /// Deletes a child directory
        /// </summary>
        /// <param name="name">Name of the directory</param>
        void DeleteDirectory(string name);

        /// <summary>
        /// Checks whether a file exists at the given relative path
        /// </summary>
        /// <param name="relativePath">Path to the file to check, relative to this directory</param>
        /// <returns>Returns <c>true</c> if the file exists.</returns>
        bool Exists(string relativePath);
    }

    /// <summary>
    /// Contracts for the <see cref="IFileSystemDirectory"/> interface
    /// </summary>
    [ContractClassFor(typeof(IFileSystemDirectory))]
    public abstract class IFileSystemDirectoryContracts: IFileSystemDirectory
    {
        /// <summary>
        /// Enumerates all the files within the directory by their names
        /// </summary>
        public IEnumerable<string> Files
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<string>>(), name => name  != null));

                return null; // dummy value
            }
        }

        /// <summary>
        /// Enumerates all the child directories of the directory by their names
        /// 
        /// <para>Use <see cref="IFileSystemDirectory.GetChildDirectory"/> to get any of these children.</para>
        /// </summary>
        public IEnumerable<string> ChildDirectories
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<string>>(), name => name  != null));

                return null; // dummy value
            }
        }

        /// <summary>
        /// Gets interface for a given child directory
        /// </summary>
        /// <param name="name">Name of the child directory</param>
        /// <returns>Returns either a directory abstraction or <c>null</c> if it does not exists.</returns>
        public IFileSystemDirectory GetChildDirectory(string name)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(name));
            Contract.Ensures(ChildDirectories.Contains(name) && Contract.Result<IFileSystemDirectory>() != null ||
                             !ChildDirectories.Contains(name) && Contract.Result<IFileSystemDirectory>() == null);
            
            return null; // dummy value
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
            Contract.Requires(childDirectory != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return null; // dummy value
        }

        /// <summary>
        /// Creates a child directory if it does not exist yet
        /// </summary>
        /// <param name="name">Name of the child directory</param>
        /// <returns>Returns the directory abstraction of the new (or already existing) directory</returns>
        public IFileSystemDirectory CreateDirectory(string name)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(name));
            Contract.Ensures(Contract.Result<IFileSystemDirectory>() != null);

            return null; // dummy value
        }

        /// <summary>
        /// Creates a new text file with a text writer in this directory
        /// </summary>
        /// <param name="name">Name of the new file</param>
        /// <returns>Returns the text writer to be used to write the contents of the file.</returns>
        public TextWriter CreateTextFile(string name)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(name));
            Contract.Ensures(Contract.Result<TextWriter>() != null);
            Contract.Ensures(Files.Contains(name));

            return null; // dummy value
        }

        /// <summary>
        /// Creates a new binary file in this directory
        /// </summary>
        /// <param name="name">Name of the new file</param>
        /// <returns>Returns the stream to be used to write the contents of the file.</returns>
        public Stream CreateBinaryFile(string name)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(name));
            Contract.Ensures(Contract.Result<Stream>() != null);
            Contract.Ensures(Files.Contains(name));

            return null; // dummy value
        }

        /// <summary>
        /// Reads an existing binary file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the stream belonging to the given file</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public Stream ReadBinaryFile(string relativePath)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(relativePath));
            Contract.Ensures(Contract.Result<Stream>() != null);
            return null; // dummy value
        }

        /// <summary>
        /// Reads an existing text file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the text reader belonging to the given file</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public TextReader ReadTextFile(string relativePath)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(relativePath));
            Contract.Ensures(Contract.Result<TextReader>() != null);
            return null; // dummy value
        }

        /// <summary>
        /// Gets the last modification's date for a given file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the last modified date.</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public DateTime GetLastModifiedDate(string relativePath)
        {
            Contract.Requires(relativePath != null);
            return DateTime.UtcNow; // dummy value
        }

        /// <summary>
        /// Gets the size of the given file which lies in this directory subtree
        /// </summary>
        /// <param name="relativePath">The relative path to the file from this directory</param>
        /// <returns>Returns the file size in bytes</returns>
        /// <exception cref="ArgumentException">If the file does not exist.</exception>
        public long GetFileSize(string relativePath)
        {
            Contract.Requires(relativePath != null);
            Contract.Ensures(Contract.Result<long>() >= 0);
            return 0; // dummy value
        }

        /// <summary>
        /// Deletes a child directory
        /// </summary>
        /// <param name="name">Name of the directory</param>
        public void DeleteDirectory(string name)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(name));
            Contract.Requires(ChildDirectories.Contains(name));
            Contract.Ensures(!ChildDirectories.Contains(name));
        }

        /// <summary>
        /// Checks whether a file exists at the given relative path
        /// </summary>
        /// <param name="relativePath">Path to the file to check, relative to this directory</param>
        /// <returns>Returns <c>true</c> if the file exists.</returns>
        public bool Exists(string relativePath)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(relativePath));
            return false; // dummy value
        }
    }
}