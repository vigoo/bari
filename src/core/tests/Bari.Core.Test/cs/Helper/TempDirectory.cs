using System;
using System.IO;

namespace Bari.Core.Test.Helper
{
    public sealed class TempDirectory : IDisposable
    {
        private static readonly object globalLock = new object();
        private readonly string path;

        public TempDirectory()
        {
            lock (globalLock)
            {
                path = Path.GetTempFileName();
                File.Delete(path);
                Directory.CreateDirectory(path);
            }
        }

        public static implicit operator string(TempDirectory dir)
        {
            return dir.path;
        }

        public void Dispose()
        {
            lock (globalLock)
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
        }
    }
}