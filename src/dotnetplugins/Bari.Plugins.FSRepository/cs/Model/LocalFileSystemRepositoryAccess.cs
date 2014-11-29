using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Generic;

namespace Bari.Plugins.FSRepository.Model
{
    public class LocalFileSystemRepositoryAccess: IFileSystemRepositoryAccess
    {
        private readonly IFileSystemDirectory suiteRoot;

        public LocalFileSystemRepositoryAccess([SuiteRoot] IFileSystemDirectory suiteRoot)
        {
            this.suiteRoot = suiteRoot;
        }

        /// <summary>
        /// Checks if the repository contains a valid file in the given path
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Returns <c>true</c> if the file exists.</returns>
        public bool Exists(string path)
        {
            if (Path.GetFileName(path) == "*.*")
                return Exists(Path.GetDirectoryName(path));
            else
            {
                if (Path.IsPathRooted(path))
                    return File.Exists(path) || Directory.Exists(path);
                else
                    return suiteRoot.Exists(path);
            }
        }

        /// <summary>
        /// Gets the directory for a given path (the path's file name part is not used)
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Returns an abstract directory interface</returns>
        public IFileSystemDirectory GetDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);

            if (Path.IsPathRooted(path))
                return new LocalFileSystemDirectory(dir);
            else
                return suiteRoot.GetChildDirectory(dir);
        }

        /// <summary>
        /// Copies a file from the FS repository to a given target directory
        /// </summary>
        /// <param name="path">Path to the file in the FS repository</param>
        /// <param name="targetDir">Target directory</param>
        /// <param name="targetFileName">Target file name</param>
        public void Copy(string path, IFileSystemDirectory targetDir, string targetFileName)
        {
            using (var source = GetDirectory(path).ReadBinaryFile(Path.GetFileName(path)))
            using (var target = targetDir.CreateBinaryFile(targetFileName))
            {
                StreamOperations.Copy(source, target);
            }
        }

        /// <summary>
        /// Lists all the files in the given directory inside the FS repository
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <returns>Returns the files in the given directory, all prefixed with the directory path</returns>
        public IEnumerable<string> ListFiles(string path)
        {
            return GetDirectory(path + Path.DirectorySeparatorChar).Files.Select(fname => Path.Combine(path, fname));
        }
    }
}