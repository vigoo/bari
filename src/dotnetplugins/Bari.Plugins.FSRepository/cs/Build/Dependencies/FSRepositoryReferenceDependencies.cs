using Bari.Core.Build;
using Bari.Plugins.FSRepository.Model;

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
            return fingerprintFactory.CreateFSRepositoryFingerprint(repository, path);
        }
    }
}