using System;
using System.Collections.Generic;

namespace Bari.Core.Build.Dependencies.Protocol
{
    /// <summary>
    /// Object containing serialization data for <see cref="CombinedFingerprint"/> 
    /// </summary>
    [Serializable]
    public struct CombinedFingerprintProtocol : IDependencyFingerprintProtocol
    {
        /// <summary>
        /// Gets or sets the child data
        /// </summary>
        public ISet<IDependencyFingerprintProtocol> Items { get; set; }

        /// <summary>
        /// Creates a new fingerprint from the data stored in the protocol
        /// </summary>
        /// <returns>Returns a fingerprint object which would save the same protocol as this one.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            return new CombinedFingerprint(this);
        }

        public void Load(IProtocolDeserializerContext context)
        {
            int count = context.ReadInt();

            Items = new HashSet<IDependencyFingerprintProtocol>();
            for (int i = 0; i < count; i++)
            {
                var item = context.ReadProtocol();
                Items.Add(item);
            }
        }

        public void Save(IProtocolSerializerContext context)
        {
            context.Write(Items.Count);
            foreach (var item in Items)
            {
                context.Write(item);
            }
        }
    }
}