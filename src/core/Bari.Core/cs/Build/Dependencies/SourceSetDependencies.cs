using System;
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
        private readonly Func<string, bool> exclusions;

        /// <summary>
        /// Constructs the dependency object
        /// </summary>
        /// <param name="kernel">The interface to create new instances</param>
        /// <param name="sourceSet">The source set on which we are depending on</param>
        /// <param name="exclusions">Exclusion function, if returns true for a file name, it won't be taken into account as a dependency</param>
        public SourceSetDependencies(IResolutionRoot kernel, SourceSet sourceSet, Func<string, bool> exclusions = null)
        {
            this.kernel = kernel;
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
            return kernel.Get<SourceSetFingerprint>(
                new ConstructorArgument("files", sourceSet.Files, false),
                new ConstructorArgument("exclusions", exclusions, false));
        }
    }
}