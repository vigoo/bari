using Bari.Core.Generic;

namespace Bari.Core.Commands.Clean
{
    /// <summary>
    /// Deletes the given cache directory as an additional clean step
    /// </summary>
    public class CacheCleaner: ICleanExtension
    {
        private readonly IFileSystemDirectory cacheDir;

        /// <summary>
        /// Constructs the cleaner
        /// </summary>
        /// <param name="cacheDir">Directory to be deleted</param>
        public CacheCleaner(IFileSystemDirectory cacheDir)
        {
            this.cacheDir = cacheDir;
        }

        /// <summary>
        /// Performs the additional cleaning step
        /// </summary>
        public void Clean()
        {
            cacheDir.Delete();
        }
    }
}