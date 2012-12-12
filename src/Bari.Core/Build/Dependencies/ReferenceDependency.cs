using Bari.Core.Model;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Dependency on a project reference's uri
    /// </summary>
    public class ReferenceDependency: IDependencies
    {
        private readonly Reference reference;

        /// <summary>
        /// Creates the dependency on the given reference
        /// </summary>
        /// <param name="reference">Reference to depend on</param>
        public ReferenceDependency(Reference reference)
        {
            this.reference = reference;
        }

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            return new ObjectPropertiesFingerprint(reference, new[] { "Uri" });
        }
    }
}