using System;
using System.IO;
using Bari.Core.Build.Dependencies.Protocol;

namespace Bari.Core.Build
{
    /// <summary>
    /// Represents a generic fingerprint of a dependency set, which can be compared to other fingerprints
    /// to determine cache validity.
    /// </summary>
    public interface IDependencyFingerprint: IEquatable<IDependencyFingerprint>
    {
        /// <summary>
        /// Saves the fingerprint to the given target stream
        /// </summary>
        /// <param name="serializer">The serializer implementation to be used</param>
        /// <param name="targetStream">The stream to be used when serializing the fingerprint</param>
        void Save(IProtocolSerializer serializer, Stream targetStream);

        /// <summary>
        /// Gets the raw data used for serialization
        /// </summary>
        IDependencyFingerprintProtocol Protocol { get; }
    }
}