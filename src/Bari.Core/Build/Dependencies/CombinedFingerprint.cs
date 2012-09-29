using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

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
    }
}