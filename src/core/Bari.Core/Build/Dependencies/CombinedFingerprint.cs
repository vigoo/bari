using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Bari.Core.Build.Dependencies.Protocol;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Combines a set of fingerprints into one
    /// 
    /// <para>Two combined fingerprints are equal, if both their two sets of fingerprints
    /// are equal.</para>
    /// <para>A combined fingerprint is NOT equal to a single fingerprint, even if it has
    /// only one item. In this case the caller should return the single item instead. This
    /// is verified by a contract.</para>
    /// </summary>
    public class CombinedFingerprint: IDependencyFingerprint, IEquatable<CombinedFingerprint>
    {
        private readonly ISet<IDependencyFingerprint> fingerprints;

        /// <summary>
        /// Creates the fingerprint
        /// </summary>
        /// <param name="deps">The combined dependencies, each dependency will be asked
        /// for a fingerprint.</param>
        public CombinedFingerprint(IEnumerable<IDependencies> deps)
        {
            Contract.Requires(deps != null);
            Contract.Requires(deps.Count() > 1);
            Contract.Ensures(fingerprints != null);
            Contract.Ensures(fingerprints.Count > 1);

            fingerprints = new HashSet<IDependencyFingerprint>(
                deps.Select(dep => dep.CreateFingerprint()));
        }

        /// <summary>
        /// Creates the fingerprint by loading it from a stream containing data previously created
        /// by the <see cref="Save"/> method.
        /// </summary>
        /// <param name="serializer">The serializer implementation to be used</param>
        /// <param name="sourceStream">Deserialization stream</param>
        public CombinedFingerprint(IProtocolSerializer serializer, Stream sourceStream)
            : this(serializer.Deserialize<CombinedFingerprintProtocol>(sourceStream))
        {
            Contract.Requires(sourceStream != null);
        }

        /// <summary>
        /// Creates the fingerprint by its deserialized protocol data
        /// </summary>
        /// <param name="proto">The deserialized protocol data</param>
        public CombinedFingerprint(CombinedFingerprintProtocol proto)
        {
            fingerprints = new HashSet<IDependencyFingerprint>();
            var noDeps = new NoDependencies();
            var noDepsFp = noDeps.CreateFingerprint();

            foreach (var childProtocol in proto.Items)
            {
                var childDependencyFingerprint = childProtocol != null ? childProtocol.CreateFingerprint() : noDepsFp;
                fingerprints.Add(childDependencyFingerprint);
            }
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
            var combinedFingerprint = other as CombinedFingerprint;
            if (combinedFingerprint != null)
                return Equals(combinedFingerprint);
            else
                return false;
        }

        /// <summary>
        /// Saves the fingerprint to the given target stream
        /// </summary>
        /// <param name="serializer">The serializer implementation to be used</param>
        /// <param name="targetStream">The stream to be used when serializing the fingerprint</param>
        public void Save(IProtocolSerializer serializer, Stream targetStream)
        {
            serializer.Serialize(targetStream, Protocol);
        }

        /// <summary>
        /// Gets the raw protocol data used for serialization
        /// </summary>
        public IDependencyFingerprintProtocol Protocol
        {
            get
            {
                var items = new HashSet<IDependencyFingerprintProtocol>();
                
                foreach (var child in fingerprints)
                {
                    items.Add(child.Protocol);
                }

                return new CombinedFingerprintProtocol
                    {                        
                        Items = items
                    };
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(CombinedFingerprint other)
        {
            return fingerprints.SetEquals(other.fingerprints);
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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CombinedFingerprint && Equals((CombinedFingerprint)obj);
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
            return fingerprints.Aggregate(19, (h, fp) => h ^ fp.GetHashCode());
        }

        /// <summary>
        /// Equality test
        /// </summary>
        public static bool operator ==(CombinedFingerprint left, CombinedFingerprint right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality test
        /// </summary>
        public static bool operator !=(CombinedFingerprint left, CombinedFingerprint right)
        {
            return !Equals(left, right);
        }
    }
}