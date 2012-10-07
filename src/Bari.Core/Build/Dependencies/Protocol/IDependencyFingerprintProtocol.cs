namespace Bari.Core.Build.Dependencies.Protocol
{
    /// <summary>
    /// Common interface for dependency fingerprint serialization protocols
    /// </summary>
    public interface IDependencyFingerprintProtocol
    {
        /// <summary>
        /// Creates a new fingerprint from the data stored in the protocol
        /// </summary>
        /// <returns>Returns a fingerprint object which would save the same protocol as this one.</returns>
        IDependencyFingerprint CreateFingerprint();
    }
}