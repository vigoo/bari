using System.Diagnostics.Contracts;
using Bari.Core.UI;
using System;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Represents dependency on another builder (<see cref="IBuilder"/>)
    /// </summary>
    public class SubtaskDependency: DependenciesBase
    {
        private readonly IBuilder subtask;

        /// <summary>
        /// Constructs the dependency object 
        /// </summary>
        /// <param name="subtask"></param>
        public SubtaskDependency(IBuilder subtask)
        {
            Contract.Requires(subtask != null);

            this.subtask = subtask;
        }

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        protected override IDependencyFingerprint CreateFingerprint()
        {
            return subtask.Dependencies.Fingerprint;
        }

        public override void Dump(IUserOutput output)
        {
            output.Message(String.Format("Subtask {0}", subtask));
            output.Indent();
            try 
            {
                subtask.Dependencies.Dump(output);
            }
            finally
            {
                output.Unindent();
            }
        }
    }
}