using System;
using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Factory interface for creating <see cref="SourceSetFingerprint"/> instances
    /// </summary>
    public interface ISourceSetFingerprintFactory
    {
        /// <summary>
        /// Creates a new source set fingerprint
        /// </summary>
        /// <param name="files">The files in the source set, in suite relative path form</param>
        /// <param name="exclusions">Exclusion function, if returns true for a file name, it won't be taken into account as a dependency</param>
        /// <returns>Returns the new instance</returns>
        SourceSetFingerprint CreateSourceSetFingerprint(IEnumerable<SuiteRelativePath> files,
                                                        Func<string, bool> exclusions);
    }
}