using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Generic;

namespace Bari.Core.Build.Cache
{
    /// <summary>
    /// Build cache storing build outputs together with their actual dependency fingerprint
    /// in the file system.
    /// </summary>
    public class FileBuildCache : IBuildCache
    {
        private const string DepsFileName = ".deps";
        private const string NamesFileName = ".names";

        private readonly IFileSystemDirectory cacheRoot;
        private readonly IProtocolSerializer protocolSerializer;
        private readonly IDictionary<BuildKey, ReaderWriterLockSlim> locks;

        /// <summary>
        /// Constructs the cache
        /// </summary>
        /// <param name="cacheRoot">Root directory where the cache will store its contents.</param>
        /// <param name="protocolSerializer">The serializer to be used for saving dependency fingerprint protocols</param>
        public FileBuildCache(IFileSystemDirectory cacheRoot, IProtocolSerializer protocolSerializer)
        {
            this.cacheRoot = cacheRoot;
            this.protocolSerializer = protocolSerializer;
            locks = new Dictionary<BuildKey, ReaderWriterLockSlim>();
        }

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
            var lck = GetOrCreateLock(builder);
            lck.EnterUpgradeableReadLock();
        }

        /// <summary>
        /// Removes the lock put by the <see cref="IBuildCache.LockForBuilder"/> method.
        /// </summary>
        /// <param name="builder">Builder key</param>
        public void UnlockForBuilder(BuildKey builder)
        {
            ReaderWriterLockSlim lck;
            if (locks.TryGetValue(builder, out lck))
                lck.ExitUpgradeableReadLock();
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
            var lck = GetOrCreateLock(builder);
            lck.EnterWriteLock();
            try
            {
                var cacheDir = cacheRoot.GetChildDirectory(GetCacheDirectoryName(builder), createIfMissing: true);

                SaveDependencyFingerprint(fingerprint, cacheDir);
                SaveOutputs(outputs, targetRoot, cacheDir);
            }
            finally
            {
                lck.ExitWriteLock();
            }
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
            var lck = GetOrCreateLock(builder);
            lck.EnterReadLock();
            try
            {
                var dirName = GetCacheDirectoryName(builder);
                if (cacheRoot.ChildDirectories.Contains(dirName))
                {
                    var cacheDir = cacheRoot.GetChildDirectory(dirName);
                    if (cacheDir.Files.Contains(DepsFileName))
                    {
                        using (var depsStream = cacheDir.ReadBinaryFile(DepsFileName))
                        {
                            var fpType = fingerprint.GetType();
                            var storedFp = Activator.CreateInstance(fpType, protocolSerializer, depsStream);
                            return fingerprint.Equals(storedFp);
                        }
                    }
                }

                return false;
            }
            finally
            {
                lck.ExitReadLock();
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
            var lck = GetOrCreateLock(builder);
            lck.EnterReadLock();
            try
            {
                var dirName = GetCacheDirectoryName(builder);
                if (cacheRoot.ChildDirectories.Contains(dirName))
                {
                    var cacheDir = cacheRoot.GetChildDirectory(dirName);
                    if (cacheDir.Files.Contains(NamesFileName))
                    {
                        using (var reader = cacheDir.ReadTextFile(NamesFileName))
                        {
                            var result = new HashSet<TargetRelativePath>();

                            int idx = 0;
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                var cacheFileName = idx.ToString(CultureInfo.InvariantCulture);

                                // It is possible that only a file name (a virtual file) was cached without any contents:
                                if (cacheDir.Exists(cacheFileName))
                                {
                                    using (var source = cacheDir.ReadBinaryFile(cacheFileName))
                                    using (var target = targetRoot.CreateBinaryFileWithDirectories(line))
                                        Copy(source, target);
                                }

                                result.Add(new TargetRelativePath(line));
                                idx++;
                            }

                            return result;
                        }
                    }
                }

                return new HashSet<TargetRelativePath>();
            }
            finally
            {
                lck.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets an existing lock or creates a new one
        /// </summary>
        /// <param name="builder">Builder key used as a key to get locks</param>
        /// <returns>Returns a reader-writer lock</returns>
        private ReaderWriterLockSlim GetOrCreateLock(BuildKey builder)
        {
            ReaderWriterLockSlim lck;
            if (!locks.TryGetValue(builder, out lck))
                locks.Add(builder, lck = new ReaderWriterLockSlim());
            return lck;
        }

        /// <summary>
        /// Copies output files to the cache directory, and also saves a '.names' file referring to he original target relative
        /// paths of these files.
        /// </summary>
        /// <param name="outputs">Build outputs to be copied</param>
        /// <param name="targetRoot">Root directory for the build outputs</param>
        /// <param name="cacheDir">Target directory for the copy operation</param>
        private void SaveOutputs(IEnumerable<TargetRelativePath> outputs, IFileSystemDirectory targetRoot, IFileSystemDirectory cacheDir)
        {
            using (var names = cacheDir.CreateTextFile(NamesFileName))
            {
                int idx = 0;
                foreach (var outputPath in outputs)
                {
                    // It is possible that the returned path is a special path and does not refer to an existing file
                    // In this case we only have to save the filename, without its contents
                    if (targetRoot.Exists(outputPath))
                    {
                        using (var source = targetRoot.ReadBinaryFile(outputPath))
                        using (var target = cacheDir.CreateBinaryFile(idx.ToString(CultureInfo.InvariantCulture)))
                        {
                            Copy(source, target);
                        }
                    }

                    names.WriteLine(outputPath);
                    idx++;
                }
            }
        }

        /// <summary>
        /// Saves a dependency fingerprint to the cache directory
        /// </summary>
        /// <param name="fingerprint">Fingerprint to be saved</param>
        /// <param name="cacheDir">Target directory</param>
        private void SaveDependencyFingerprint(IDependencyFingerprint fingerprint, IFileSystemDirectory cacheDir)
        {
            using (var depStream = cacheDir.CreateBinaryFile(DepsFileName))
                fingerprint.Save(protocolSerializer, depStream);
        }

        /// <summary>
        /// Copies a stream to another one
        /// </summary>
        /// <param name="source">Source stream</param>
        /// <param name="target">Target stream</param>
        private void Copy(Stream source, Stream target)
        {
            const int localBufferSize = 4096;

            var buf = new byte[localBufferSize];

            int count;
            do
            {
                count = source.Read(buf, 0, localBufferSize);
                target.Write(buf, 0, count);
            } while (count == localBufferSize);
        }

        /// <summary>
        /// Gets the directory name associated with a given Builder key
        /// </summary>
        /// <param name="builder">Builder key</param>
        /// <returns>Returns a valid directory name</returns>
        private static string GetCacheDirectoryName(BuildKey builder)
        {
            return builder.ToString();
        }
    }
}