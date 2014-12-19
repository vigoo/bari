using System;

namespace Bari.Core.Generic
{
	public class FileSystemChangedEventArgs : EventArgs
	{
        private readonly string relativePath;

        public string RelativePath
        {
            get
            {
                return this.relativePath;
            }
        }

        public FileSystemChangedEventArgs(string relativePath)
        {
            this.relativePath = relativePath;
        }        
	}
}

