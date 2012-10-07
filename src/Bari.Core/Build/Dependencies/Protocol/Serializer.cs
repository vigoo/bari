using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bari.Core.Build.Dependencies.Protocol
{
    /// <summary>
    /// Helper class for serializing and deserializing dependency fingerprint protocols
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Deserializes an object from a stream
        /// </summary>
        /// <typeparam name="T">Type of the result object</typeparam>
        /// <param name="stream">Stream containing binary serialized object data</param>
        /// <returns>Returns the object casted to type <c>T</c></returns>
        public static T Deserialize<T>(Stream stream)
        {
            var formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Serializes an object to a stream
        /// </summary>
        /// <typeparam name="T">Type of the object to be serialized</typeparam>
        /// <param name="stream">Stream where object data will be put</param>
        /// <param name="obj">Object to be serialized</param>
        public static void Serialize<T>(Stream stream, T obj)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }
    }
}