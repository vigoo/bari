using System;
using Bari.Core.Model;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Represents dependency on a full <see cref="SourceSet"/>
    /// </summary>
    public class SourceSetDependencies: IDependencies
    {
        private readonly ISourceSetFingerprintFactory fingerprintFactory;
        private readonly SourceSet sourceSet;
        private readonly Func<string, bool> exclusions;

        /// <summary>
        /// Constructs the dependency object
        /// </summary>
        /// <param name="fingerprintFactory">The interface to create new fingerprint instances</param>
        /// <param name="sourceSet">The source set on which we are depending on</param>
        /// <param name="exclusions">Exclusion function, if returns true for a file name, it won't be taken into account as a dependency</param>
        public SourceSetDependencies(ISourceSetFingerprintFactory fingerprintFactory, SourceSet sourceSet, Func<string, bool> exclusions = null)
        {
            this.fingerprintFactory = fingerprintFactory;
            this.sourceSet = sourceSet;
            this.exclusions = exclusions;
        }

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            return fingerprintFactory.CreateSourceSetFingerprint(sourceSet.Files, exclusions);
        }
    }
}