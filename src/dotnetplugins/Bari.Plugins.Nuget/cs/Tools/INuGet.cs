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
        /// Installs a package and returns the path to the DLLs to be linked
        /// </summary>
        /// <param name="name">Package name</param>
        /// <param name="version">Package version, if null or empty then the latest one will be used</param>
        /// <param name="root">Root directory for storing the downloaded packages</param>
        /// <param name="relativeTargetDirectory">Path relative to <c>root</c> where the downloaded package should be placed</param>
        /// <returns>Returns the <c>root</c> relative paths of the DLL files to be used</returns>
        IEnumerable<TargetRelativePath> InstallPackage(string name, string version, IFileSystemDirectory root, string relativeTargetDirectory);
    }
}