using System;
using System.Collections.Generic;

namespace Bari.Core.Build.Dependencies.Protocol
{
    /// <summary>
    /// Object containing serialization data for <see cref="ObjectPropertiesFingerprint"/>
    /// </summary>
    [Serializable]
    public class ObjectPropertiesProtocol: IDependencyFingerprintProtocol
    {
        /// <summary>
        /// Gets or sets the stored property values
        /// </summary>
        public IDictionary<string, object> Values { get; set; }

        /// <summary>
        /// Creates a new fingerprint from the data stored in the protocol
        /// </summary>
        /// <returns>Returns a fingerprint object which would save the same protocol as this one.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            return new ObjectPropertiesFingerprint(this);
        }
    }
}