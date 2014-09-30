using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Core.Model
{
    public interface ISourceSet
    {
        /// <summary>
        /// Gets the type of sources in this set
        /// </summary>
        SourceSetType Type { get; }

        /// <summary>
        /// Gets the files belonging to this set
        /// 
        /// <para>The file names are stored in relative path form, relative to the suite root</para>
        /// </summary>
        IEnumerable<SuiteRelativePath> Files { get; }
    }
}