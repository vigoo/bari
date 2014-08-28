using System;
using Bari.Core.Model;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Factory interface for creating <see cref="SourceSetDependencies"/> and <see cref="SourceSetStructureDependency"/> instances
    /// </summary>
    public interface ISourceSetDependencyFactory
    {
        /// <summary>
        /// Creates a new dependency object
        /// </summary>
        /// <param name="sourceSet">The source set on which we are depending on</param>
        /// <param name="exclusions">Exclusion function, if returns true for a file name, it won't be taken into account as a dependency</param>
        /// <returns>Returns the new instance</returns>
        SourceSetDependencies CreateSourceSetDependencies(ISourceSet sourceSet, Func<string, bool> exclusions);

        /// <summary>
        /// Creates a new dependency on source set's structure
        /// </summary>
        /// <param name="sourceSet">Source set on which we are depending on</param>
        /// <param name="exclusions">Exclusion function, if returns true for a file name, it won't be taken into account as a dependency</param>
        /// <returns>Returns the new instance</returns>
        SourceSetStructureDependency CreateSourceSetStructureDependency(ISourceSet sourceSet, Func<string, bool> exclusions);
    }
}