using Bari.Core.Model;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Represents dependency on a full <see cref="SourceSet"/>
    /// </summary>
    public class SourceSetDependencies: IDependencies
    {
        private readonly IResolutionRoot kernel;
        private readonly SourceSet sourceSet;

        /// <summary>
        /// Constructs the dependency object
        /// </summary>
        /// <param name="kernel">The interface to create new instances</param>
        /// <param name="sourceSet">The source set on which we are depending on</param>
        public SourceSetDependencies(IResolutionRoot kernel, SourceSet sourceSet)
        {
            this.kernel = kernel;
            this.sourceSet = sourceSet;
        }

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            return kernel.Get<SourceSetFingerprint>(new Parameter("files", sourceSet.Files, false));
        }
    }
}