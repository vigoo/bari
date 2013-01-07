using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Tools;

namespace Bari.Plugins.Nuget.Tools
{
    /// <summary>
    /// Default implementation of the <see cref="INuGet"/> interface, uses the command line NuGet tool in a separate process.
    /// </summary>
    public class NuGet : DownloadableExternalTool, INuGet
    {
        /// <summary>
        /// Creates the external tool
        /// </summary>
        public NuGet()
            : base("NuGet", @"C:\Programs\", "NuGet.exe", new Uri("https://nuget.org/nuget.exe"))
        {
        }

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
                        var lib40client = libRoot.GetDirectories("net40-client").FirstOrDefault();
                        var lib35 = libRoot.GetDirectories("net35").FirstOrDefault();
                        var lib20 = libRoot.GetDirectories("net20").FirstOrDefault();

                        if (lib40full != null)
                            return GetDllsIn(localRoot.AbsolutePath, lib40full);
                        else if (lib40 != null)
                            return GetDllsIn(localRoot.AbsolutePath, lib40);
                        else if (lib40client != null)
                            return GetDllsIn(localRoot.AbsolutePath, lib40client);
                        else if (lib35 != null)
                            return GetDllsIn(localRoot.AbsolutePath, lib35);
                        else if (lib20 != null)
                            return GetDllsIn(localRoot.AbsolutePath, lib20);
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
    }
}