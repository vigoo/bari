using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// Factory interface for <see cref="IReferenceBuilder"/> instances
    /// </summary>
    public interface IReferenceBuilderFactory
    {
        /// <summary>
        /// Creates a new reference builder
        /// </summary>
        /// <param name="reference">Reference, the URI scheme determines the reference builder's type</param>
        /// <param name="project">Project the reference builder belongs to</param>
        /// <returns>Returns the reference builder or <c>null</c> if it not available</returns>
        IReferenceBuilder CreateReferenceBuilder(Reference reference, Project project);
    }
}