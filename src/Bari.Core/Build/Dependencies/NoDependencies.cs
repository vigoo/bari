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
        }
    }
}