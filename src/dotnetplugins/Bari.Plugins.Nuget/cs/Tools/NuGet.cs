using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Tools;
using Bari.Core.UI;
using Bari.Plugins.Nuget.Generic;

namespace Bari.Plugins.Nuget.Tools
{
    /// <summary>
    /// Default implementation of the <see cref="INuGet"/> interface, uses the command line NuGet tool in a separate process.
    /// </summary>
    public class NuGet : DownloadableExternalTool, INuGet
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (NuGet));

        private readonly IParameters parameters;

        /// <summary>
        /// Creates the external tool
        /// </summary>
        public NuGet(IParameters parameters)
            : base("NuGet", @"C:\Programs\", "NuGet.exe", new Uri("https://nuget.org/nuget.exe"))
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Installs a package and returns the path to the DLLs to be linked
        /// </summary>
        /// <param name="name">Package name</param>
        /// <param name="version">Package version, if null or empty then the latest one will be used</param>
        /// <param name="root">Root directory for storing the downloaded packages</param>
        /// <param name="relativeTargetDirectory">Path relative to <c>root</c> where the downloaded package should be placed</param>
        /// <param name="dllsOnly">If <c>true</c>, only the DLLs will be returned, otherwise all the files in the package</param>
        /// <param name="maxProfile">Maximum allowed profile</param>
        /// <returns>Returns the <c>root</c> relative paths of the DLL files to be used</returns>
        public Tuple<string, IEnumerable<string>> InstallPackage(string name, string version, IFileSystemDirectory root, string relativeTargetDirectory, bool dllsOnly, NugetLibraryProfile maxProfile)
        {
            if (String.IsNullOrWhiteSpace(version))
                Run(root, "install", name, "-o", "\""+relativeTargetDirectory+"\"", "-Verbosity", Verbosity);
            else
                Run(root, "install", name, "-Version", version, "-o", "\"" + relativeTargetDirectory + "\"", "-Verbosity", Verbosity);

            var result = new List<string>(); // root relative paths
            string commonRoot = String.Empty; // root relative path

            var localRoot = root as LocalFileSystemDirectory;
            if (localRoot != null)
            {
                var pkgRoot = new DirectoryInfo(Path.Combine(localRoot.AbsolutePath, relativeTargetDirectory));
                var modRoot = pkgRoot.GetDirectories(name + "*", SearchOption.TopDirectoryOnly).FirstOrDefault();

                if (modRoot != null)
                {
                    var libRoot = modRoot.GetDirectories("lib", SearchOption.TopDirectoryOnly).FirstOrDefault();
                    var contentRoot = modRoot.GetDirectories("content", SearchOption.TopDirectoryOnly).FirstOrDefault();
                    commonRoot = GetRelativePath(modRoot.FullName, localRoot);

                    if (libRoot != null)
                    {
                        AddDlls(libRoot, result, localRoot, maxProfile);
                        commonRoot = GetRelativePath(libRoot.FullName, localRoot);
                    }
                    if (contentRoot != null && !dllsOnly)
                    {
                        AddContents(contentRoot, result, localRoot);

                        if (libRoot == null)
                            commonRoot = GetRelativePath(contentRoot.FullName, localRoot);
                    }
                }
            }

            log.DebugFormat("Returning common root {0}", commonRoot);
            return Tuple.Create(commonRoot, result.AsEnumerable());
        }

        public void CreatePackage(IFileSystemDirectory targetRoot, string packageName, string nuspec)
        {
            var localRoot = targetRoot as LocalFileSystemDirectory;
            if (localRoot != null)
            {
                var nuSpecName = packageName + ".nuspec";
                using (var writer = localRoot.CreateTextFile(nuSpecName))
                    writer.WriteLine(nuspec);

                Run(targetRoot, "pack", nuSpecName, "-Verbosity", Verbosity);
            }
        }

        private string GetRelativePath(string path, LocalFileSystemDirectory root)
        {
            return path.Substring(root.AbsolutePath.Length).TrimStart(Path.DirectorySeparatorChar);
        }

        private void AddDlls(DirectoryInfo libRoot, List<string> result, LocalFileSystemDirectory localRoot, NugetLibraryProfile maxProfile)
        {
            var lib45 = libRoot.GetDirectories("net45-full").FirstOrDefault() ??
                        libRoot.GetDirectories("net45").FirstOrDefault();
            var lib40 = libRoot.GetDirectories("net40-full").FirstOrDefault() ??
                        libRoot.GetDirectories("net40").FirstOrDefault() ?? 
                        libRoot.GetDirectories("net4").FirstOrDefault();
            var lib40client = libRoot.GetDirectories("net40-client").FirstOrDefault();
            var lib35 = libRoot.GetDirectories("net35").FirstOrDefault();
            var lib20 = libRoot.GetDirectories("net20").FirstOrDefault() ??
                        libRoot.GetDirectories("20").FirstOrDefault();

            if (lib45 != null && maxProfile == NugetLibraryProfile.Net45)
                result.AddRange(GetDllsIn(localRoot, lib45));
            else if (lib40 != null && maxProfile >= NugetLibraryProfile.Net4)
                result.AddRange(GetDllsIn(localRoot, lib40));
            else if (lib40client != null && maxProfile >= NugetLibraryProfile.Net4Client)
                result.AddRange(GetDllsIn(localRoot, lib40client));
            else if (lib35 != null && maxProfile != NugetLibraryProfile.Net35)
                result.AddRange(GetDllsIn(localRoot, lib35));
            else if (lib20 != null && maxProfile != NugetLibraryProfile.Net2)
                result.AddRange(GetDllsIn(localRoot, lib20));
            else
                result.AddRange(GetDllsIn(localRoot, libRoot));
        }

        private void AddContents(DirectoryInfo contentRoot, List<string> result, LocalFileSystemDirectory localRoot)
        {                
            result.AddRange(GetAllIn(localRoot, contentRoot));
        }

        private IEnumerable<string> GetDllsIn(LocalFileSystemDirectory root, DirectoryInfo dir)
        {
            log.DebugFormat("Getting DLLs from {0} relative to {1}...", dir.FullName, root.AbsolutePath);

            return from file in dir.GetFiles("*.dll")
                let relPath = GetRelativePath(file.FullName, root)
                select relPath;
        }

        private IEnumerable<string> GetAllIn(LocalFileSystemDirectory root, DirectoryInfo dir)
        {
            log.DebugFormat("Getting all files from {0} relative to {1}...", dir.FullName, root.AbsolutePath);

            return from file in dir.RecursiveGetFiles()
                let relPath = GetRelativePath(file.FullName, root)
                select relPath;
        }

        private string Verbosity
        {
            get { return parameters.VerboseOutput ? "detailed" : "quiet"; }
        }
    }
}