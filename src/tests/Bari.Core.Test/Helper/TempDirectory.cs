using System;
using System.IO;

namespace Bari.Core.Test.Helper
{
    public class TempDirectory : IDisposable
    {
        private readonly string path;

        public TempDirectory()
        {
            path = Path.GetTempFileName();
            File.Delete(path);
            Directory.CreateDirectory(path);
        }

        public static implicit operator string(TempDirectory dir)
        {
            return dir.path;
        }

        public void Dispose()
        {
            Directory.Delete(path, true);
        }
    }
}