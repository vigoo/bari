using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Plugins.Nuget.Tools
{
    public interface INuGet
    {
        IEnumerable<TargetRelativePath> InstallPackage(string name, IFileSystemDirectory root, string relativeTargetDirectory);
    }
}