using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace Bari.Core.Model
{
    /// <summary>
    /// A set of source files, belonging to the same set
    /// </summary>
    public class SourceSet
    {
        private readonly string type;
        private readonly ISet<string> files = new SortedSet<string>(StringComparer.InvariantCultureIgnoreCase);

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
        public IEnumerable<string> Files
        {
            get { return files; }
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
        public void Add(string path)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(path));
            Contract.Requires(!Path.IsPathRooted(path));
            Contract.Ensures(files.Contains(path));

            files.Add(path);
        }
    }
}