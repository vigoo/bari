namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Represents dependency on another builde (<see cref="IBuilder"/>)
    /// </summary>
    public class SubtaskDependency: IDependencies
    {
        private readonly IBuilder subtask;

        /// <summary>
        /// Constructs the dependency object 
        /// </summary>
        /// <param name="subtask"></param>
        public SubtaskDependency(IBuilder subtask)
        {
            this.subtask = subtask;
        }

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            throw new System.NotImplementedException();
        }
    }
}