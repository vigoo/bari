using System;
using Bari.Core.Model;
using Bari.Core.UI;
using System.Linq;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Represents dependency on the items of a <see cref="SourceSet"/>, but not on the source files' content
    /// </summary>
    public class SourceSetStructureDependency : IDependencies
    {
        private readonly ISourceSetFingerprintFactory fingerprintFactory;
        private readonly ISourceSet sourceSet;
        private readonly Func<string, bool> exclusions;

        /// <summary>
        /// Constructs the dependency object
        /// </summary>
        /// <param name="fingerprintFactory">The interface to create new fingerprint instances</param>
        /// <param name="sourceSet">The source set on which we are depending on</param>
        /// <param name="exclusions">Exclusion function, if returns true for a file name, it won't be taken into account as a dependency</param>
        public SourceSetStructureDependency(ISourceSetFingerprintFactory fingerprintFactory, ISourceSet sourceSet, Func<string, bool> exclusions = null)
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
            return fingerprintFactory.CreateSourceSetFingerprint(sourceSet.Files, exclusions, fullDependency: false);
        }

        public void Dump(IUserOutput output)
        {
            output.Message(String.Format("Source set structure *{0}* ({1} files)", sourceSet.Type, sourceSet.Files.Count()));
        }
    }
}