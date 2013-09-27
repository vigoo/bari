using Bari.Plugins.FSRepository.Model;

namespace Bari.Plugins.FSRepository.Build.Dependencies
{
    public interface IFSRepositoryFingerprintFactory
    {
        FSRepositoryFingerprint CreateFSRepositoryFingerprint(IFileSystemRepositoryAccess repository, string path);
    }
}