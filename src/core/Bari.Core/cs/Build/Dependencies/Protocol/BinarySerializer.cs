using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace Bari.Core.Build.Dependencies.Protocol
{
    /// <summary>
    /// Binary serializer based serializer implementation
    /// </summary>
    public class BinarySerializer: IProtocolSerializer
    {
        private readonly IDependencyFingerprintProtocolRegistry registry;

        public BinarySerializer (IDependencyFingerprintProtocolRegistry registry)
        {
            this.registry = registry;
        }

        /// <summary>
        /// Deserializes an object from a stream
        /// </summary>
        /// <typeparam name="T">Type of the result object</typeparam>
        /// <param name="stream">Stream containing binary serialized object data</param>
        /// <returns>Returns the object casted to type <c>T</c></returns>
        public T Deserialize<T>(Stream stream)
            where T: IDependencyFingerprintProtocol
        {               
            var context = new BinaryProtocolDeserializerContext(stream, registry);
            var result = Activator.CreateInstance<T>();
            result.Load(context);
            return result;
        }

        /// <summary>
        /// Serializes an object to a stream
        /// </summary>
        /// <typeparam name="T">Type of the object to be serialized</typeparam>
        /// <param name="stream">Stream where object data will be put</param>
        /// <param name="obj">Object to be serialized</param>
        public void Serialize<T>(Stream stream, T obj)
            where T: IDependencyFingerprintProtocol
        { 
            var context = new BinaryProtocolSerializerContext(stream, registry);
            obj.Save(context);
        }
    }
}