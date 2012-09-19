using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Core.Build
{
    /// <summary>
    /// Builder represents a set of the build process where a given set of dependencies
    /// is used to create a set of outputs.
    /// </summary>
    public interface IBuilder
    {
        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        IDependencies Dependencies { get; }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        ISet<TargetRelativePath> Run();
    }
}