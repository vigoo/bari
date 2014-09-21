using System;
using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Plugins.Nuget.Tools
{
    /// <summary>
    /// Provides an interface to the NuGet package manager
    /// </summary>
    public interface INuGet
    {
        /// <summary>
        /// Installs a package and returns the path to the files to be linked
        /// </summary>
        /// <param name="name">Package name</param>
        /// <param name="version">Package version, if null or empty then the latest one will be used</param>
        /// <param name="root">Root directory for storing the downloaded packages</param>
        /// <param name="relativeTargetDirectory">Path relative to <c>root</c> where the downloaded package should be placed</param>
        /// <param name="dllsOnly">If <c>true</c>, only the DLLs will be returned, otherwise all the files in the package</param>
        /// <param name="maxProfile">Maximum allowed profile</param>
        /// <returns>Returns the <c>root</c> relative paths of the files to be used, and the common root for them 
        /// to help preserving the package's directory structure</returns>
        Tuple<string, IEnumerable<string>> InstallPackage(string name, string version, IFileSystemDirectory root, string relativeTargetDirectory, bool dllsOnly, NugetLibraryProfile maxProfile);

        /// <summary>
        /// Creates a package based on a nuspec
        /// </summary>
        /// <param name="targetRoot">Target root directory</param>
        /// <param name="packageName">Name of the package to be generated</param>
        /// <param name="nuspec">The package's nuspec in XML</param>
        void CreatePackage(IFileSystemDirectory targetRoot, string packageName, string nuspec);
    }
}