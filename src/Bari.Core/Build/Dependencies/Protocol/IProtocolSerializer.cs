using System.IO;

namespace Bari.Core.Build.Dependencies.Protocol
{
    /// <summary>
    /// Serialization abstraction for saving and loading fingerprint protocols
    /// </summary>
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
}