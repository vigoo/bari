using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Generic;

namespace Bari.Core.Build.Cache
{
    /// <summary>
    /// Wraps a builder so it is only ran if its dependencies has been modified,
    /// or the cache has been corrupted.
    /// </summary>
    public class CachedBuilder: IBuilder
    {
        private readonly IBuilder wrappedBuilder;
        private readonly IBuildCache cache;
        private readonly IFileSystemDirectory targetDir;

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return wrappedBuilder.Dependencies; }
        }

        /// <summary>
        /// Creates a cached builder
        /// </summary>
        /// <param name="wrappedBuilder">The builder instance to be wrapped</param>
        /// <param name="cache">The cache implementation to be used</param>
        /// <param name="targetDir">The target directory's file system abstraction</param>
        public CachedBuilder(IBuilder wrappedBuilder, IBuildCache cache, [TargetRoot] IFileSystemDirectory targetDir)
        {
            Contract.Requires(wrappedBuilder != null);
            Contract.Requires(cache != null);
            Contract.Requires(targetDir != null);

            this.wrappedBuilder = wrappedBuilder;
            this.cache = cache;
            this.targetDir = targetDir;
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in suite relative paths</returns>
        public ISet<TargetRelativePath> Run()
        {
            var currentFingerprint = wrappedBuilder.Dependencies.CreateFingerprint();
            var builderType = wrappedBuilder.GetType();

            cache.LockForBuilder(builderType);
            try
            {
                if (cache.Contains(builderType, currentFingerprint))
                    return cache.Restore(builderType, targetDir);
                else
                {
                    var files = wrappedBuilder.Run();
                    cache.Store(builderType, currentFingerprint, files, targetDir);
                    return files;
                }
            }
            finally
            {
                cache.UnlockForBuilder(wrappedBuilder.GetType());
            }
        }
    }
}