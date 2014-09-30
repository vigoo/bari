using System.Collections.Generic;
using System.Linq;
using Bari.Core.Generic;

namespace Bari.Core.Model
{
    /// <summary>
    /// Base class for source set filtering
    /// </summary>
    public abstract class FilteredSourceSet : ISourceSet
    {
        private readonly ISourceSet baseSourceSet;

        protected FilteredSourceSet(ISourceSet baseSourceSet)
        {
            this.baseSourceSet = baseSourceSet;
        }

        /// <summary>
        /// Gets the type of sources in this set
        /// </summary>
        public SourceSetType Type { get { return baseSourceSet.Type; } }

        /// <summary>
        /// Gets the files belonging to this set
        /// 
        /// <para>The file names are stored in relative path form, relative to the suite root</para>
        /// </summary>
        public IEnumerable<SuiteRelativePath> Files
        {
            get 
            {
                return baseSourceSet.Files.Where(file => !FilterOut(file));
            }
        }

        /// <summary>
        /// The filter function
        /// </summary>
        /// <param name="path">Path to the file in the source set</param>
        /// <returns>Returns <c>true</c> if the file should be filtered out</returns>
        protected abstract bool FilterOut(SuiteRelativePath path);
    }
}