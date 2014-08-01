using System.Linq;
using Bari.Core.Build;
using Bari.Core.Generic;

namespace Bari.Core.Commands.Clean
{
    /// <summary>
    /// Deletes the given cache directory as an additional clean step
    /// </summary>
    public class CacheCleaner: ICleanExtension
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (CacheCleaner));

        private readonly IFileSystemDirectory cacheDir;
        private readonly IBuilderEnumerator builderEnumerator;

        /// <summary>
        /// Constructs the cleaner
        /// </summary>
        /// <param name="cacheDir">Directory to be deleted</param>
        /// <param name="builderEnumerator">All the registered reference builders</param>
        public CacheCleaner(IFileSystemDirectory cacheDir, IBuilderEnumerator builderEnumerator)
        {
            this.cacheDir = cacheDir;
            this.builderEnumerator = builderEnumerator;
        }

        /// <summary>
        /// Performs the additional cleaning step
        /// </summary>
        /// <param name="parameters"></param>
        public void Clean(ICleanParameters parameters)
        {
            if (parameters.KeepReferences)
            {
                var persistentReferenceBuilders = builderEnumerator.GetAllPersistentBuilders().ToList();
                var persistentBuilderPrefixes = persistentReferenceBuilders.Select(b => b.FullName + "_").ToList();

                foreach (var directory in cacheDir.ChildDirectories)
                {
                    foreach (var prefix in persistentBuilderPrefixes)
                    {
                        if (!directory.StartsWith(prefix))
                        {
                            cacheDir.GetChildDirectory(directory).Delete();
                            break;
                        }
                        else
                        {
                            log.InfoFormat("Keeping {0} because keep-references option is active", directory);
                        }
                    }
                }

                if (!cacheDir.ChildDirectories.Any())
                    cacheDir.Delete();
            }
            else
            {
                cacheDir.Delete();
            }
        }        
    }
}