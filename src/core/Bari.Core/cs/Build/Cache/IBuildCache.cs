using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Core.Build.Cache
{
    /// <summary>
    /// Cache interface for storing and restoring build outputs
    /// 
    /// <para>Cached items have a combined key consisting of the builder's type and
    /// its dependencies' fingerprint.
    /// </para>
    /// </summary>
    public interface IBuildCache
    {
        /// <summary>
        /// Locks the cache for a given builder. 
        /// 
        /// <para>Until calling <see cref="UnlockForBuilder"/>, it is guaranteed that no
        /// <see cref="Store"/> operation will be ran for the given builder from other
        /// threads.</para>
        /// </summary>
        /// <param name="builder">Builder key</param>
        void LockForBuilder(BuildKey builder);

        /// <summary>
        /// Removes the lock put by the <see cref="LockForBuilder"/> method.
        /// </summary>
        /// <param name="builder">Builder key</param>
        void UnlockForBuilder(BuildKey builder);

        /// <summary>
        /// Store build outputs in the cache by reading them from the file system
        /// </summary>
        /// <param name="builder">Builder key (first part of the key)</param>
        /// <param name="fingerprint">Dependency fingerprint created when the builder was executed (second part of the key)</param>
        /// <param name="outputs">Target-relative path of the build outputs to be cached</param>
        /// <param name="targetRoot">File system abstraction of the root target directory</param>
        void Store(BuildKey builder, IDependencyFingerprint fingerprint, IEnumerable<TargetRelativePath> outputs,
                   IFileSystemDirectory targetRoot);

        /// <summary>
        /// Checks if the cache contains stored outputs for a given builder with a given dependency fingerprint
        /// 
        /// <para>If <see cref="Restore"/> will be also called, the cache must be locked first using
        /// the <see cref="LockForBuilder"/> method.</para>
        /// </summary>
        /// <param name="builder">Builder key</param>
        /// <param name="fingerprint">Current dependency fingerprint</param>
        /// <returns>Returns <c>true</c> if there are stored outputs for the given builder and fingerprint combination.</returns>
        bool Contains(BuildKey builder, IDependencyFingerprint fingerprint);

        /// <summary>
        /// Restores the stored files for a given builder to a file system directory
        /// 
        /// <para>The cache only stores the latest stored results and this is what will be restored
        /// to the target directory. To verify if it was generated with the correct dependency fingerprint,
        /// use <see cref="Contains"/>.</para>
        /// <para>To ensure thread safety, use <see cref="LockForBuilder"/>.</para>
        /// </summary>
        /// <param name="builder">Builder key</param>
        /// <param name="targetRoot">Target file system directory</param>
        /// <returns>Returns the target root relative paths of all the restored files</returns>
        ISet<TargetRelativePath> Restore(BuildKey builder, IFileSystemDirectory targetRoot);
    }
}