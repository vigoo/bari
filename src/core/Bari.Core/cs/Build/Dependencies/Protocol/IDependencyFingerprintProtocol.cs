using System.Diagnostics.Contracts;

namespace Bari.Core.Build.Dependencies.Protocol
{
    /// <summary>
    /// Common interface for dependency fingerprint serialization protocols
    /// </summary>
    [ContractClass(typeof(IDependencyFingerprintProtocolContracts))]
    public interface IDependencyFingerprintProtocol
    {
        /// <summary>
        /// Creates a new fingerprint from the data stored in the protocol
        /// </summary>
        /// <returns>Returns a fingerprint object which would save the same protocol as this one.</returns>
        IDependencyFingerprint CreateFingerprint();

        void Load(IProtocolDeserializerContext context);
        void Save(IProtocolSerializerContext context);
    }

    /// <summary>
    /// Contract for <see cref="IDependencyFingerprintProtocol"/>
    /// </summary>
    [ContractClassFor(typeof(IDependencyFingerprintProtocol))]
    abstract class IDependencyFingerprintProtocolContracts: IDependencyFingerprintProtocol
    {
        /// <summary>
        /// Creates a new fingerprint from the data stored in the protocol
        /// </summary>
        /// <returns>Returns a fingerprint object which would save the same protocol as this one.</returns>
        public IDependencyFingerprint CreateFingerprint()
        {
            Contract.Ensures(Contract.Result<IDependencyFingerprint>() != null);
            return null; // dummy value
        }

        public void Load(IProtocolDeserializerContext context)
        {
            Contract.Requires(context != null);
        }

        public void Save(IProtocolSerializerContext context)
        {
            Contract.Requires(context != null);
        }
    }
}