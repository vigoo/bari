using System;
using System.IO;
using System.Linq;

namespace Bari.Core.Generic
{
    public static class PathOperations
    {
        public static string FindCommonRoot(string[] paths)
        {
            if (paths.Length > 0)
            {
                var dirs = paths.Select(Path.GetDirectoryName).ToArray();
                var maxLength = dirs.Max(p => p.Length);
                var firstLongest = dirs.First(p => p.Length == maxLength);
                var longestSplitted = firstLongest.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                string currentRoot = String.Empty;
                foreach (var segment in longestSplitted)
                {
                    var proposedRoot = currentRoot + segment + Path.DirectorySeparatorChar;
                    if (paths.All(p => p.StartsWith(proposedRoot)))
                        currentRoot = proposedRoot;
                    else
                        break;
                }

                return currentRoot;
            }
            else
            {
                return String.Empty;
            }
        } 
    }
}