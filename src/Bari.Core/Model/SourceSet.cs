using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Bari.Core.Generic;

namespace Bari.Core.Model
{
    /// <summary>
    /// A set of source files, belonging to the same set
    /// </summary>
    public class SourceSet
    {
        private readonly string type;
        private readonly ISet<SuiteRelativePath> files = new SortedSet<SuiteRelativePath>();

        /// <summary>
        /// Gets the type of sources in this set
        /// </summary>
        public string Type
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                
                return type;
            }
        }

        /// <summary>
        /// Gets the files belonging to this set
        /// 
        /// <para>The file names are stored in relative path form, relative to the suite root</para>
        /// </summary>
        public IEnumerable<SuiteRelativePath> Files
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<SuiteRelativePath>>() != null);
                
                return files;
            }
        }

        /// <summary>
        /// Creates an empty source set
        /// </summary>
        /// <param name="type">Type of sources in this set</param>
        public SourceSet(string type)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(type));

            this.type = type;
        }

        /// <summary>
        /// Adds a file to the set
        /// </summary>
        /// <param name="path">Path of the file relative to the suite root</param>
        public void Add(SuiteRelativePath path)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(path));
            Contract.Requires(!Path.IsPathRooted(path));
            Contract.Ensures(files.Contains(path));

            files.Add(path);
        }

        /// <summary>
        /// Removes a file from the set
        /// </summary>
        /// <param name="path">Path of the file relative to the suite root</param>
        public void Remove(SuiteRelativePath path)
        {
            Contract.Requires(Files.Contains(path));
            Contract.Ensures(!files.Contains(path));

            files.Remove(path);
        }
    }
}