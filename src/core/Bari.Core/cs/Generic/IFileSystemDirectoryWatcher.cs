using System;

namespace Bari.Core.Generic
{
    public interface IFileSystemDirectoryWatcher: IDisposable
    {
        event EventHandler<FileSystemChangedEventArgs> Changed;
        void Stop();
    }
}

