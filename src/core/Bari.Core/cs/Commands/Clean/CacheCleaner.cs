using System.Collections.Generic;
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
        private readonly IEnumerable<IReferenceBuilder> referenceBuilders;

        /// <summary>
        /// Constructs the cleaner
        /// </summary>
        /// <param name="cacheDir">Directory to be deleted</param>
        /// <param name="referenceBuilders">All the registered reference builders</param>
        public CacheCleaner(IFileSystemDirectory cacheDir, IEnumerable<IReferenceBuilder> referenceBuilders)
        {
            this.cacheDir = cacheDir;
            this.referenceBuilders = referenceBuilders;
        }

        /// <summary>
        /// Performs the additional cleaning step
        /// </summary>
        /// <param name="parameters"></param>
        public void Clean(ICleanParameters parameters)
        {
            if (parameters.KeepReferences)
            {
                var persistentReferenceBuilders = referenceBuilders.Where(IsPersistentReferenceBuilder).ToList();
                var persistentBuilderPrefixes = persistentReferenceBuilders.Select(b => b.GetType().FullName + "_").ToList();

                foreach (var directory in cacheDir.ChildDirectories)
                {
                    foreach (var prefix in persistentBuilderPrefixes)
                    {
                        if (!directory.StartsWith(prefix))
                        {
                            cacheDir.GetChildDirectory(directory).Delete();
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

        private bool IsPersistentReferenceBuilder(IReferenceBuilder referenceBuilder)
        {
            return
                referenceBuilder.GetType()
                    .GetCustomAttributes(typeof (PersistentReferenceAttribute), true)
                    .OfType<PersistentReferenceAttribute>()
                    .Any();
        }
    }
}