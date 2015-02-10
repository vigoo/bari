using Bari.Core.UI;


namespace Bari.Core.Build.Dependencies
{
    public abstract class DependenciesBase: IDependencies
    {        
        private IDependencyFingerprint fingerprint; // Fingerprints are calculated ONCE per run

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        public IDependencyFingerprint Fingerprint 
        { 
            get 
            {
                if (fingerprint == null)
                    fingerprint = CreateFingerprint();

                return fingerprint;
            }
        }

        protected abstract IDependencyFingerprint CreateFingerprint();

        /// <summary>
        /// Dumps debug information about this dependency to the output
        /// </summary>
        public abstract void Dump(IUserOutput output);
    }
}

