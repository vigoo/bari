using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bari.Plugins.Nuget.Generic
{
    public static class DirectoryInfoExtensions
    {
        public static IEnumerable<FileInfo> RecursiveGetFiles(this DirectoryInfo root)
        {
            return root.GetFiles().Concat(root.GetDirectories().SelectMany(dir => dir.RecursiveGetFiles()));
        }
    }
}