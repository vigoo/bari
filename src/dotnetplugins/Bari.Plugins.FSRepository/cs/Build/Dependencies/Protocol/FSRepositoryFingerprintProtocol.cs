using System;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies.Protocol;

namespace Bari.Plugins.FSRepository.Build.Dependencies.Protocol
{
    public class FSRepositoryFingerprintProtocol: IDependencyFingerprintProtocol
    {
        /// <summary>
        /// Gets or sets the path for the file in the repository
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the last modified date/time of the file
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the last size of the file
        /// </summary>
        public long LastSize { get; set; }

        /// <summary>
        /// Creates a new fingerprint from the data stored in the protocol
        /// </summary>
        /// <returns>Returns a fingerprint object which would save the same protocol as this one.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            return new FSRepositoryFingerprint(this);
        }
    }
}