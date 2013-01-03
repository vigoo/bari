using System;
using System.Collections.Generic;
using System.Monads;
using System.Threading;
using Bari.Core.Generic;

namespace Bari.Core.Build.Cache
{
    /// <summary>
    /// Support class for <see cref="MemoryBuildCache"/>, stores fingerprint and build outputs in memory
    /// for one builder.
    /// 
    /// <para>The cache item is protected by a reader/writer lock.</para>
    /// </summary>
    public sealed class MemoryCacheItem: IDisposable
    {
        private IDependencyFingerprint fingerprint;
        private IDictionary<TargetRelativePath, byte[]> outputs;
        private readonly ReaderWriterLockSlim rwlock;

        /// <summary>
        /// Constructs an empty cache item for a builder
        /// 
        /// <para>Empty cache items are valid, but meaningless,
        /// <see cref="Update"/> should be called immediately to store data.</para>
        /// </summary>
        public MemoryCacheItem()
        {
            rwlock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        /// <summary>
        /// Gets the outputs stored currently in this item.
        /// 
        /// <para>Using the returned dictionary is safe, as cache updates create new
        /// dictionaries instead of updating this one.</para>
        /// </summary>
        public IDictionary<TargetRelativePath, byte[]> Outputs
        {
            get
            {
                try
                {
                    rwlock.EnterReadLock();
                    return outputs;
                }
                finally
                {
                    rwlock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Acquires an upgradeable reader lock to the cache item
        /// 
        /// <para>Use <see cref="ExitUpgradeableLock"/> in pair with this call.</para>
        /// </summary>
        public void EnterUpgradeableLock()
        {
            rwlock.EnterUpgradeableReadLock();
        }

        /// <summary>
        /// Releases the upgradeable reader lock to the cache item,
        /// which was previously acquired by <see cref="EnterUpgradeableLock"/>
        /// </summary>
        public void ExitUpgradeableLock()
        {
            rwlock.ExitUpgradeableReadLock();
        }

        /// <summary>
        /// Updates the cached data for this item
        /// </summary>
        /// <param name="newFingerprint">The new dependency fingerprint</param>
        /// <param name="newOutputs">File name - binary data map of all the build outputs</param>
        public void Update(IDependencyFingerprint newFingerprint, IDictionary<TargetRelativePath, byte[]> newOutputs)
        {
            rwlock.EnterWriteLock();
            try
            {
                fingerprint = newFingerprint;
                outputs = new Dictionary<TargetRelativePath, byte[]>();
                foreach (var pair in newOutputs)                
                    outputs.Add(pair);                
            }
            finally
            {
                rwlock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Compares the stored fingerprint with another one
        /// </summary>
        /// <param name="otherFingerprint">The fingerprint to compare to</param>
        /// <returns>Returns <c>true</c> if the stored fingerprint matches the given one</returns>
        public bool MatchesFingerprint(IDependencyFingerprint otherFingerprint)
        {
            rwlock.EnterReadLock();
            try
            {
                return fingerprint.With(fp => fp.Equals(otherFingerprint));
            }
            finally
            {
                rwlock.ExitReadLock();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            rwlock.Dispose();
        }
    }
}