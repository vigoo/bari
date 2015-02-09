using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Plugins.FSRepository.Model;
using Bari.Core.UI;

namespace Bari.Plugins.FSRepository.Build.Dependencies
{
    /// <summary>
    /// Represents dependency on a file in a file system repository
    /// </summary>
    public class FSRepositoryReferenceDependencies: IDependencies
    {
        private readonly IFSRepositoryFingerprintFactory fingerprintFactory;
        private readonly IFileSystemRepositoryAccess repository;
        private readonly string path;

        /// <summary>
        /// Constructs the dependency object
        /// </summary>
        /// <param name="fingerprintFactory">The interface to create new fingerprint instances</param>
        /// <param name="repository">The interface to access the file system based repository</param>
        /// <param name="path">Resolved path of the dependency</param>
        public FSRepositoryReferenceDependencies(IFSRepositoryFingerprintFactory fingerprintFactory, IFileSystemRepositoryAccess repository, string path)
        {
            this.fingerprintFactory = fingerprintFactory;
            this.repository = repository;
            this.path = path;
        }

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            if (Path.GetFileName(path) == "*.*")
            {
                var files = repository.ListFiles(Path.GetDirectoryName(path)).ToList();
                if (files.Count == 1)
                    return fingerprintFactory.CreateFSRepositoryFingerprint(repository, files[0]);
                else
                    return new CombinedFingerprint(files.Select(file => new FSRepositoryReferenceDependencies(fingerprintFactory, repository, file)));
            }
            else
            {
                return fingerprintFactory.CreateFSRepositoryFingerprint(repository, path);
            }
        }

        public void Dump(IUserOutput output)
        {
            output.Message(string.Format("FS repo: `{0}`", path));
        }
    }
}