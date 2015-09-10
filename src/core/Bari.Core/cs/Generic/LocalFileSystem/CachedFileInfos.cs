using System.Collections.Generic;
using System.IO;

namespace Bari.Core.Generic.LocalFileSystem
{
    public class CachedFileInfos
    {
        private readonly IDictionary<string, FileInfo> fileInfos = new Dictionary<string, FileInfo>();

        public FileInfo Get(string absolutePath)
        {
            FileInfo result;
            if (!fileInfos.TryGetValue(absolutePath, out result))
            {
                result = new FileInfo(absolutePath);
                fileInfos.Add(absolutePath, result);
            }

            return result;
        }

        public void Invalidate()
        {
            fileInfos.Clear();
        }
    }
}