using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bari.Core.Generic;

namespace Bari.Plugins.Nuget.Tools
{
    /// <summary>
    /// Default implementation of the <see cref="INuGet"/> interface, uses the command line NuGet tool in a separate process.
    /// </summary>
    public class NuGet : INuGet
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (NuGet));

        /// <summary>
        /// Installs a package and returns the path to the DLLs to be linked
        /// </summary>
        /// <param name="name">Package name</param>
        /// <param name="root">Root directory for storing the downloaded packages</param>
        /// <param name="relativeTargetDirectory">Path relative to <c>root</c> where the downloaded package should be placed</param>
        /// <returns>Returns the <c>root</c> relative paths of the DLL files to be used</returns>
        public IEnumerable<TargetRelativePath> InstallPackage(string name, IFileSystemDirectory root, string relativeTargetDirectory)
        {
            Run(root, "install", name, "-o", "\""+relativeTargetDirectory+"\"");

            var localRoot = root as LocalFileSystemDirectory;
            if (localRoot != null)
            {
                var pkgRoot = new DirectoryInfo(Path.Combine(localRoot.AbsolutePath, relativeTargetDirectory));
                var modRoot = pkgRoot.GetDirectories(name + "*", SearchOption.TopDirectoryOnly).FirstOrDefault();

                if (modRoot != null)
                {
                    var libRoot = modRoot.GetDirectories("lib", SearchOption.TopDirectoryOnly).FirstOrDefault();

                    if (libRoot != null)
                    {
                        var lib40full = libRoot.GetDirectories("net40-full").FirstOrDefault();
                        var lib40 = libRoot.GetDirectories("net40").FirstOrDefault();

                        if (lib40full != null)
                            return GetDllsIn(localRoot.AbsolutePath, lib40full);
                        else if (lib40 != null)
                            return GetDllsIn(localRoot.AbsolutePath, lib40);
                        else
                            return GetDllsIn(localRoot.AbsolutePath, libRoot);
                    }
                }
            }
                            
            return new TargetRelativePath[0]; // TODO           
        }

        private IEnumerable<TargetRelativePath> GetDllsIn(string rootPath, DirectoryInfo dir)
        {
            return from file in dir.GetFiles("*.dll")
                   let relPath = file.FullName.Substring(rootPath.Length)
                   select new TargetRelativePath(relPath);
        }

        private void Run(IFileSystemDirectory root, params string[] args)
        {
            var localRoot = root as LocalFileSystemDirectory;
            if (localRoot != null)
            {
                const string path = @"c:\programs\NuGet.exe"; // TODO: this must not be hard-coded and could be downloaded dynamically from nuget site (http://nuget.codeplex.com/downloads/get/412077)

                var psi = new ProcessStartInfo
                {
                    FileName = path,
                    WorkingDirectory = localRoot.AbsolutePath,
                    Arguments = String.Join(" ", args),
                    UseShellExecute = false
                };

                log.DebugFormat("Executing {0} with arguments {1}", path, psi.Arguments);

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                }
            }
            else
            {
                throw new NotSupportedException("Only local file system is supported for NuGet!");
            }
        }
    }
}