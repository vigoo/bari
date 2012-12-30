using System.Collections.Generic;
using System.IO;
using Bari.Core.Generic;
using Bari.Plugins.Csharp.VisualStudio.CsprojSections;

namespace Bari.Plugins.Csharp.VisualStudio
{
    /// <summary>
    /// Contextual information available for CSproj section generators (<see cref="ICsprojSection"/>)
    /// </summary>
    public interface ICsprojGeneratorContext
    {
        /// <summary>
        /// Gets the set of references for the given project
        /// </summary>
        ISet<TargetRelativePath> References { get; }

        /// <summary>
        /// Gets the text writer used to generate version information C# file
        /// </summary>
        TextWriter VersionOutput { get; }

        /// <summary>
        /// Gets the name of the file the <see cref="VersionOutput"/> writer generates
        /// </summary>
        string VersionFileName { get; }
    }
}