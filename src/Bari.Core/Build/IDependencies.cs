namespace Bari.Core.Build
{
    /// <summary>
    /// Represents a set of dependencies for a <see cref="IBuilder"/>
    /// </summary>
    public interface IDependencies
    {
        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        IDependencyFingerprint CreateFingerprint();
    }
}