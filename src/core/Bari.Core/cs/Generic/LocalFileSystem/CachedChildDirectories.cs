using System.Collections.Generic;

namespace Bari.Core.Generic.LocalFileSystem
{
    public class CachedChildDirectories
    {
        private readonly IDictionary<string, LocalFileSystemDirectory> fileInfos = new Dictionary<string, LocalFileSystemDirectory>();

        public LocalFileSystemDirectory Get(string absolutePath)
        {
            LocalFileSystemDirectory result;
            if (!fileInfos.TryGetValue(absolutePath, out result))
            {
                result = new LocalFileSystemDirectory(absolutePath);
                fileInfos.Add(absolutePath, result);
            }

            return result;
        } 
    }
}