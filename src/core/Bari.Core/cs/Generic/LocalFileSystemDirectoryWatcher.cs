using System.IO;
using System;


namespace Bari.Core.Generic
{
    public class LocalFileSystemDirectoryWatcher: IFileSystemDirectoryWatcher
	{
        private readonly FileSystemWatcher watcher;
        private readonly string path;

        public event EventHandler<FileSystemChangedEventArgs> Changed;

        public LocalFileSystemDirectoryWatcher(string path)
        {
            this.path = path;

            watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true, 
                NotifyFilter = NotifyFilters.FileName
            };
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnChanged;
            watcher.EnableRaisingEvents = true;
        }            

        public void Stop()
        {
            watcher.Dispose();
        }     

        public void Dispose()
        {
            Stop();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, new FileSystemChangedEventArgs(e.FullPath.Substring(path.Length).TrimStart(Path.DirectorySeparatorChar)));
            }
        }
	}

}