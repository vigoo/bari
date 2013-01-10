using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Monads;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Generic;
using Bari.Core.Model;
using Ninject;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Stores data about a full source set (<see cref="SourceSet"/> using every file's name, size 
    /// and last modified date
    /// 
    /// <para>Two source set fingerprints are equal if the set of names are equal, and for each file the last
    /// modified date and the file size are also equals.</para>
    /// </summary>
    public sealed class SourceSetFingerprint : IDependencyFingerprint, IEquatable<SourceSetFingerprint>
    {
        private readonly ISet<SuiteRelativePath> fileNames;
        private readonly IDictionary<SuiteRelativePath, DateTime> lastModifiedDates;
        private readonly IDictionary<SuiteRelativePath, long> lastSizes;

        /// <summary>
        /// Constructs the fingerprint by getting the file modification dates from the file system
        /// </summary>
        /// <param name="root">The suite's root directory</param>
        /// <param name="files">The files in the source set, in suite relative path form</param>
        /// <param name="exclusions">Exclusion function, if returns true for a file name, it won't be taken into account as a dependency</param>
        [Inject]
        public SourceSetFingerprint([SuiteRoot] IFileSystemDirectory root, IEnumerable<SuiteRelativePath> files, Func<string, bool> exclusions)
        {
            Contract.Requires(root != null);
            Contract.Requires(files != null);

            fileNames = new SortedSet<SuiteRelativePath>();
            lastModifiedDates = new Dictionary<SuiteRelativePath, DateTime>();
            lastSizes = new Dictionary<SuiteRelativePath, long>();

            foreach (var file in files)
            {
                if (exclusions == null || !exclusions(file))
                {
                    fileNames.Add(file);
                    lastModifiedDates.Add(file, root.GetLastModifiedDate(file));
                    lastSizes.Add(file, root.GetFileSize(file));
                }
            }
        }

        /// <summary>
        /// Constructs the fingerprint by deserializing it from a stream containing data previously 
        /// created by the <see cref="Save"/> method.
        /// </summary>
        /// <param name="serializer">The serialization implementation to be used</param>
        /// <param name="sourceStream">Deserialization stream</param>
        public SourceSetFingerprint(IProtocolSerializer serializer, Stream sourceStream)
            : this(serializer.Deserialize<SourceSetFingerprintProtocol>(sourceStream))
        {
            Contract.Requires(serializer != null);
            Contract.Requires(sourceStream != null);
        }

        /// <summary>
        /// Constructs the fingerprint based on the deserialized protocol data
        /// </summary>
        /// <param name="proto">The protocol data which was deserialized from a stream</param>
        public SourceSetFingerprint(SourceSetFingerprintProtocol proto)
        {
            fileNames = new SortedSet<SuiteRelativePath>();
            lastModifiedDates = new Dictionary<SuiteRelativePath, DateTime>();
            lastSizes = new Dictionary<SuiteRelativePath, long>();

            proto.Files.Do(pair =>
                {
                    var path = new SuiteRelativePath(pair.Key);
                    fileNames.Add(path);
                    lastModifiedDates.Add(path, pair.Value.LastModifiedDate);
                    lastSizes.Add(path, pair.Value.LastSize);
                });        
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
            if (otherFingerprint != null)
                return Equals(otherFingerprint);
            else
                return false;
        }

        /// <summary>
        /// Saves the fingerprint to the given target stream
        /// </summary>
        /// <param name="serializer">The serialization implementation to be used</param>
        /// <param name="targetStream">The stream to be used when serializing the fingerprint</param>
        public void Save(IProtocolSerializer serializer, Stream targetStream)
        {
            serializer.Serialize(targetStream, Protocol);
        }

        /// <summary>
        /// Gets the raw protocol data used for serialization
        /// </summary>
        public IDependencyFingerprintProtocol Protocol
        {
            get
            {
                var proto = new SourceSetFingerprintProtocol
                {
                    Files = new Dictionary<string, SourceSetFingerprintProtocol.FileFingerprint>()
                };
                foreach (var path in fileNames)
                {
                    proto.Files.Add(path, new SourceSetFingerprintProtocol.FileFingerprint
                    {
                        LastModifiedDate = lastModifiedDates[path],
                        LastSize = lastSizes[path]
                    });
                }

                return proto;
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(SourceSetFingerprint other)
        {
            if (fileNames.SetEquals(other.fileNames))
                return fileNames.All(file => 
                    lastModifiedDates[file] == other.lastModifiedDates[file] &&
                    lastSizes[file] == other.lastSizes[file]);
            else
                return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SourceSetFingerprint && Equals((SourceSetFingerprint)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return lastModifiedDates.Aggregate(11, (n, pair) => pair.Key.GetHashCode() ^ pair.Value.GetHashCode() ^ n) ^
                   lastSizes.Aggregate(11, (n, pair) => pair.Key.GetHashCode() ^ pair.Value.GetHashCode() ^ n);
        }

        /// <summary>
        /// Equality test
        /// </summary>
        public static bool operator ==(SourceSetFingerprint left, SourceSetFingerprint right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality test
        /// </summary>
        public static bool operator !=(SourceSetFingerprint left, SourceSetFingerprint right)
        {
            return !Equals(left, right);
        }
    }
}