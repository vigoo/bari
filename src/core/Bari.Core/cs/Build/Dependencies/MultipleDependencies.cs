using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.UI;
using System;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Represents a set of builder dependencies, where each individual dependency
    /// has to be met.
    /// </summary>
    public class MultipleDependencies: DependenciesBase
    {
        private readonly ISet<IDependencies> dependencies;

        /// <summary>
        /// Creates the dependency
        /// </summary>
        /// <param name="deps">The dependencies to combine into one</param>
        public MultipleDependencies(params IDependencies[] deps)
        {
            Contract.Requires(deps != null);

            dependencies = new HashSet<IDependencies>(deps);
        }

        /// <summary>
        /// Creates the dependency
        /// </summary>
        /// <param name="deps">The dependencies to combine into one</param>
        public MultipleDependencies(IEnumerable<IDependencies> deps)
        {
            Contract.Requires(deps != null);

            dependencies = new HashSet<IDependencies>(deps);
        }

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        protected override IDependencyFingerprint CreateFingerprint()
        {
            return new CombinedFingerprint(dependencies);
        }

        public override void Dump(IUserOutput output)
        {
            output.Message(String.Format("Multiple dependencies ({0} subdeps)", dependencies.Count));
            output.Indent();
            try 
            {
                foreach (var dep in dependencies)
                {
                    dep.Dump(output);
                }
            }
            finally
            {
                output.Unindent();
            }
        }
    }
}