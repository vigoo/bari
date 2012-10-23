using System.Diagnostics.Contracts;
using System.IO;

namespace Bari.Core.Build.Dependencies.Protocol
{
    /// <summary>
    /// Serialization abstraction for saving and loading fingerprint protocols
    /// </summary>
    [ContractClass(typeof(IProtocolSerializerContracts))]
    public interface IProtocolSerializer
    {
        /// <summary>
        /// Deserializes an object from a stream
        /// </summary>
        /// <typeparam name="T">Type of the result object</typeparam>
        /// <param name="stream">Stream containing binary serialized object data</param>
        /// <returns>Returns the object casted to type <c>T</c></returns>
        T Deserialize<T>(Stream stream);

        /// <summary>
        /// Serializes an object to a stream
        /// </summary>
        /// <typeparam name="T">Type of the object to be serialized</typeparam>
        /// <param name="stream">Stream where object data will be put</param>
        /// <param name="obj">Object to be serialized</param>
        void Serialize<T>(Stream stream, T obj);
    }

    /// <summary>
    /// Contract for <see cref="IProtocolSerializer"/> interface
    /// </summary>
    [ContractClassFor(typeof(IProtocolSerializer))]
    abstract class IProtocolSerializerContracts: IProtocolSerializer
    {
        /// <summary>
        /// Deserializes an object from a stream
        /// </summary>
        /// <typeparam name="T">Type of the result object</typeparam>
        /// <param name="stream">Stream containing binary serialized object data</param>
        /// <returns>Returns the object casted to type <c>T</c></returns>
        public T Deserialize<T>(Stream stream)
        {
            Contract.Requires(stream != null);
            Contract.Requires(!stream.CanSeek || stream.Length > 0);
            Contract.Ensures(Contract.Result<T>() != null);            

            return default(T); // dummy value
        }

        /// <summary>
        /// Serializes an object to a stream
        /// </summary>
        /// <typeparam name="T">Type of the object to be serialized</typeparam>
        /// <param name="stream">Stream where object data will be put</param>
        /// <param name="obj">Object to be serialized</param>
        public void Serialize<T>(Stream stream, T obj)
        {
            Contract.Requires(stream != null);
            Contract.Requires(obj != null);            
        }
    }
}