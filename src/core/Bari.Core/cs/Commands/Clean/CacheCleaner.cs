using System;
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

        private readonly Lazy<IFileSystemDirectory> cacheDir;
        private readonly IBuilderEnumerator builderEnumerator;
        private readonly Func<ISoftCleanPredicates> predicatesFactory;

        /// <summary>
        /// Constructs the cleaner
        /// </summary>
        /// <param name="cacheDir">Directory to be deleted</param>
        /// <param name="builderEnumerator">All the registered reference builders</param>
        /// <param name="predicatesFactory">Factory for soft-clean predicate registry</param>
        public CacheCleaner(Lazy<IFileSystemDirectory> cacheDir, IBuilderEnumerator builderEnumerator, Func<ISoftCleanPredicates> predicatesFactory)
        {
            this.cacheDir = cacheDir;
            this.builderEnumerator = builderEnumerator;
            this.predicatesFactory = predicatesFactory;
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

                foreach (var directory in cacheDir.Value.ChildDirectories)
                {
                    foreach (var prefix in persistentBuilderPrefixes)
                    {
                        if (!directory.StartsWith(prefix))
                        {
                            DeleteDirectory(cacheDir.Value.GetChildDirectory(directory), parameters);
                            break;
                        }
                        else
                        {
                            log.InfoFormat("Keeping {0} because keep-references option is active", directory);
                        }
                    }
                }

                if (!cacheDir.Value.ChildDirectories.Any())
                    DeleteDirectory(cacheDir.Value, parameters);
            }
            else
            {
                DeleteDirectory(cacheDir.Value, parameters);
            }
        }

        private void DeleteDirectory(IFileSystemDirectory directory, ICleanParameters parameters)
        {
            var predicates = predicatesFactory();

            if (parameters.SoftClean)
                directory.Delete(predicates.ShouldDelete);
            else
                directory.Delete();
        }
    }
}