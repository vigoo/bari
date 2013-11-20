using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Gets the relative path from a given directory to given file in a larger common directory subtree.
        /// 
        /// r.Object.GetRelativePathFrom(a.Object, @"a\c.txt").Should().Be(@"c.txt");
        /// r.Object.GetRelativePathFrom(a.Object, @"a.test\e.txt").Should().Be(@"..\a.test\e.txt");
        /// r.Object.GetRelativePathFrom(a.Object, @"b\d.txt").Should().Be(@"..\b\d.txt");
        /// </summary>
        /// <example>
        /// Consider the following hierarchy:
        /// r 
        /// -> a
        ///   -> c.txt
        /// -> a.test
        ///   -> e.txt
        /// -> b
        ///   -> d.txt
        /// 
        /// In this case:
        /// <c>r.GetRelativePathFrom(a, "a\c.txt")</c> is <c>c.txt</c>
        /// <c>r.GetRelativePathFrom(a, "a.test\e.txt")</c> is <c>..\a.test\e.txt</c>
        /// <c>r.GetRelativePathFrom(a, "b\d.txt")</c> is <c>..\b\d.txt</c>
        /// </example>
        /// <param name="root">The directory subtree which contains both <c>innerRoot</c> and <c>outerRelativePath</c></param>
        /// <param name="innerRoot">The directory to get the path relative to</param>
        /// <param name="outerRelativePath">The target path, relative to the <c>root</c> directory</param>
        /// <returns>Returns the relative path from <c>innerRoot</c> to <c>outerRelativePath</c> (which is specified as a relative path
        /// to <c>root</c>)</returns>
        public static string GetRelativePathFrom(this IFileSystemDirectory root, IFileSystemDirectory innerRoot, string outerRelativePath)
        {
            if (root == innerRoot)
                return outerRelativePath;
            else
            {
                string innerFromOuter = root.GetRelativePath(innerRoot);
                if (outerRelativePath.StartsWith(innerFromOuter + '\\', StringComparison.InvariantCultureIgnoreCase))
                    return outerRelativePath.Substring(innerFromOuter.Length).TrimStart('\\');
                else
                {
                    string prefix = String.Join("\\", innerFromOuter.Split('\\').Select(_ => ".."));
                    return Path.Combine(prefix, outerRelativePath);
                }
            }
        }
    }
}