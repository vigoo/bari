using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Stores data about a full source set (<see cref="SourceSet"/> using every file's name and last modified date
    /// 
    /// <para>Two source set fingerprints are equal if the set of names are equal, and for each file the last
    /// modified date also equals.</para>
    /// </summary>
    public sealed class SourceSetFingerprint : IDependencyFingerprint
    {
        private readonly ISet<SuiteRelativePath> fileNames;
        private readonly IDictionary<SuiteRelativePath, DateTime> lastModifiedDates;

        /// <summary>
        /// Constructs the fingreprint by getting the file modification dates from the file system
        /// </summary>
        /// <param name="root">The suite's root directory</param>
        /// <param name="files">The files in the source set, in suite relative path form</param>
        public SourceSetFingerprint([SuiteRoot] IFileSystemDirectory root, IEnumerable<SuiteRelativePath> files)
        {
            Contract.Requires(root != null);
            Contract.Requires(files != null);

            fileNames = new SortedSet<SuiteRelativePath>(files);
            lastModifiedDates = new Dictionary<SuiteRelativePath, DateTime>();

            foreach (var file in fileNames)
            {
                lastModifiedDates.Add(file, root.GetLastModifiedDate(file));
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IDependencyFingerprint other)
        {
            var otherFingerprint = other as SourceSetFingerprint;
            if (otherFingerprint != null &&
                fileNames.SetEquals(otherFingerprint.fileNames))                 
                return fileNames.All(file => lastModifiedDates[file] == otherFingerprint.lastModifiedDates[file]);
            else
                return false;
        }
    }
}