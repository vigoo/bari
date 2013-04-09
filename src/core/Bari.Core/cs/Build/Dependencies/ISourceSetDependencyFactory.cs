using System;
using Bari.Core.Model;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Factory interface for creating <see cref="SourceSetDependencies"/> instances
    /// </summary>
    public interface ISourceSetDependencyFactory
    {
        /// <summary>
        /// Creates a new dependency object
        /// </summary>
        /// <param name="sourceSet">The source set on which we are depending on</param>
        /// <param name="exclusions">Exclusion function, if returns true for a file name, it won't be taken into account as a dependency</param>
        /// <returns>Returns the new instance</returns>
        SourceSetDependencies CreateSourceSetDependencies(SourceSet sourceSet, Func<string, bool> exclusions);
    }
}