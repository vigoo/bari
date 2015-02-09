using System.IO;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.UI;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// A dependency implementation representing no dependency
    /// </summary>
    public sealed class NoDependencies: IDependencies
    {
        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            return NoDependencyFingerprint.Instance;
        }

        private class NoDependencyFingerprint: IDependencyFingerprint
        {
            public static readonly IDependencyFingerprint Instance = new NoDependencyFingerprint();

            private NoDependencyFingerprint()
            {
            }

            /// <summary>
            /// Serialization constructor for <see cref="NoDependencyFingerprint"/>
            /// </summary>
            /// <param name="serializer">Serializer implementation to be used</param>
            /// <param name="sourceStream">Stream representing this object</param>
            public NoDependencyFingerprint(IProtocolSerializer serializer, Stream sourceStream)
            {                
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(IDependencyFingerprint other)
            {
                return other is NoDependencyFingerprint;
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
            /// </returns>
            /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
            public override bool Equals(object obj)
            {
                return obj is NoDependencyFingerprint;
            }

            /// <summary>
            /// Serves as a hash function for a particular type. 
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="T:System.Object"/>.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override int GetHashCode()
            {
                return 11;
            }

            /// <summary>
            /// Saves the fingerprint to the given target stream
            /// </summary>
            /// <param name="serializer">The serializer implementation to be used</param>
            /// <param name="targetStream">The stream to be used when serializing the fingerprint</param>
            public void Save(IProtocolSerializer serializer, Stream targetStream)
            {
            }

            /// <summary>
            /// Gets the raw protocol data used for serialization
            /// </summary>
            public IDependencyFingerprintProtocol Protocol
            {
                get { return null; }
            }
        }

        public void Dump(IUserOutput output)
        {
            output.Message("no dependencies");
        }
    }
}