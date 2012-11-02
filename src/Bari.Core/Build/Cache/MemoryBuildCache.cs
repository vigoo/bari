using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Bari.Core.Generic;

namespace Bari.Core.Build.Cache
{
    /// <summary>
    /// Simple build cache which only stores build outputs until the process is running
    /// </summary>
    public class MemoryBuildCache : IBuildCache
    {
        private readonly IDictionary<BuildKey, MemoryCacheItem> cache = new Dictionary<BuildKey, MemoryCacheItem>();

        /// <summary>
        /// Locks the cache for a given builder. 
        /// 
        /// <para>Until calling <see cref="IBuildCache.UnlockForBuilder"/>, it is guaranteed that no
        /// <see cref="IBuildCache.Store"/> operation will be ran for the given builder from other
        /// threads.</para>
        /// </summary>
        /// <param name="builder">Builder key</param>
        public void LockForBuilder(BuildKey builder)
        {
            GetOrCreate(builder).EnterUpgradeableLock();
        }

        /// <summary>
        /// Removes the lock put by the <see cref="IBuildCache.LockForBuilder"/> method.
        /// </summary>
        /// <param name="builder">Builder key</param>
        public void UnlockForBuilder(BuildKey builder)
        {
            GetOrCreate(builder).ExitUpgradeableLock();
        }

        /// <summary>
        /// Store build outputs in the cache by reading them from the file system
        /// </summary>
        /// <param name="builder">Builder key (first part of the key)</param>
        /// <param name="fingerprint">Dependency fingerprint created when the builder was executed (second part of the key)</param>
        /// <param name="outputs">Target-relative path of the build outputs to be cached</param>
        /// <param name="targetRoot">File system abstraction of the root target directory</param>
        public void Store(BuildKey builder, IDependencyFingerprint fingerprint, IEnumerable<TargetRelativePath> outputs, IFileSystemDirectory targetRoot)
        {
            MemoryCacheItem item = GetOrCreate(builder);            

            var map = new ConcurrentDictionary<TargetRelativePath, byte[]>();

            Parallel.ForEach(outputs, outputPath =>
                {
                    if (targetRoot.Exists(outputPath))
                    {
                        using (var stream = targetRoot.ReadBinaryFile(outputPath))
                        {
                            var buf = new byte[stream.Length];
                            stream.Read(buf, 0, buf.Length);

                            map.TryAdd(outputPath, buf);
                        }
                    }
                    else
                    {
                        map.TryAdd(outputPath, null);
                    }
                });
            
            item.Update(fingerprint, map);
        }

        /// <summary>
        /// Checks if the cache contains stored outputs for a given builder with a given dependency fingerprint
        /// 
        /// <para>If <see cref="IBuildCache.Restore"/> will be also called, the cache must be locked first using
        /// the <see cref="IBuildCache.LockForBuilder"/> method.</para>
        /// </summary>
        /// <param name="builder">Builder key</param>
        /// <param name="fingerprint">Current dependency fingerprint</param>
        /// <returns>Returns <c>true</c> if there are stored outputs for the given builder and fingerprint combination.</returns>
        public bool Contains(BuildKey builder, IDependencyFingerprint fingerprint)
        {
            lock (cache)
            {
                MemoryCacheItem item;
                if (cache.TryGetValue(builder, out item))
                {
                    return item.MatchesFingerprint(fingerprint);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Restores the stored files for a given builder to a file system directory
        /// 
        /// <para>The cache only stores the latest stored results and this is what will be restored
        /// to the target directory. To verify if it was generated with the correct dependency fingerprint,
        /// use <see cref="IBuildCache.Contains"/>.</para>
        /// <para>To ensure thread safety, use <see cref="IBuildCache.LockForBuilder"/>.</para>
        /// </summary>
        /// <param name="builder">Builder key</param>
        /// <param name="targetRoot">Target file system directory</param>
        /// <returns>Returns the target root relative paths of all the restored files</returns>
        public ISet<TargetRelativePath> Restore(BuildKey builder, IFileSystemDirectory targetRoot)
        {
            MemoryCacheItem item;
            lock (cache)
                cache.TryGetValue(builder, out item);

            if (item != null)
            {                
                var outputs = item.Outputs;
                var paths = new HashSet<TargetRelativePath>();
                foreach (var pair in outputs)
                {
                    if (pair.Value != null)
                    {
                        using (var stream = targetRoot.CreateBinaryFile(pair.Key))
                            stream.Write(pair.Value, 0, pair.Value.Length);
                    }

                    paths.Add(pair.Key);
                }

                return paths;
            }
            else
            {
                return new HashSet<TargetRelativePath>();
            }
        }

        private MemoryCacheItem GetOrCreate(BuildKey builder)
        {
            Contract.Requires(builder != null);
            Contract.Ensures(Contract.Result<MemoryCacheItem>() != null);

            lock (cache)
            {
                MemoryCacheItem item;
                if (!cache.TryGetValue(builder, out item))
                {
                    item = new MemoryCacheItem();
                    cache.Add(builder, item);
                }

                Contract.Assume(item != null);
                return item;
            }
        }
    }
}