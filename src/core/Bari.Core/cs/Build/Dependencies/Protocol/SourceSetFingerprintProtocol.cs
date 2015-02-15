using System;
using System.Collections.Generic;

namespace Bari.Core.Build.Dependencies.Protocol
{
    /// <summary>
    /// Object containing serialization data for <see cref="SourceSetFingerprint"/>
    /// </summary>
    [Serializable]
    public struct SourceSetFingerprintProtocol : IDependencyFingerprintProtocol
    {
        /// <summary>
        /// Serializable class holding all the data for one particular file in the source set
        /// </summary>
        [Serializable]
        public class FileFingerprint
        {
            /// <summary>
            /// Gets or sets the last modified date/time of the file
            /// </summary>
            public DateTime LastModifiedDate { get; set; }

            /// <summary>
            /// Gets or sets the last size of the file
            /// </summary>
            public long LastSize { get; set; }
        }

        /// <summary>
        /// Gets or sets the stored file fingerprints for each relative path
        /// </summary>
        public IDictionary<string, FileFingerprint> Files { get; set; }

        /// <summary>
        /// Gets or sets whether the date and size values are valid or not
        /// </summary>
        public bool FullDependency { get; set; }

        /// <summary>
        /// Creates a new fingerprint from the data stored in the protocol
        /// </summary>
        /// <returns>Returns a fingerprint object which would save the same protocol as this one.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            return new SourceSetFingerprint(this);
        }

        public void Load(IProtocolDeserializerContext context)
        {
            FullDependency = context.ReadBool();
            int count = context.ReadInt();

            Files = new Dictionary<string, FileFingerprint>();
            for (int i = 0; i < count; i++)
            {
                var name = context.ReadString();
                if (FullDependency)
                {
                    var date = context.ReadDateTime();
                    var size = context.ReadLong();

                    var fileFingerprint = new FileFingerprint
                    {
                        LastSize = size,
                        LastModifiedDate = date
                    };
                    Files.Add(name, fileFingerprint);
                }
                else
                {
                    Files.Add(name, new FileFingerprint());
                }
            }
        }


        public void Save(IProtocolSerializerContext context)
        {
            context.Write(FullDependency);
            context.Write(Files.Count);
            foreach (var pair in Files)
            {
                context.Write(pair.Key);
                if (FullDependency)
                {
                    context.Write(pair.Value.LastModifiedDate);
                    context.Write(pair.Value.LastSize);
                }
            }
        }

    }
}