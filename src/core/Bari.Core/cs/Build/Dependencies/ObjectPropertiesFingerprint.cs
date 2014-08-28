using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Monads;
using System.Text;
using Bari.Core.Build.Dependencies.Protocol;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// General purpose dependency fingerprint implementation holding actual values for a set of properties of an object
    /// </summary>
    public class ObjectPropertiesFingerprint: IDependencyFingerprint, IEquatable<ObjectPropertiesFingerprint>
    {
        private readonly IDictionary<string, object> values;

        /// <summary>
        /// Constructs the fingerprint by getting the actual property values of an object
        /// and storing them in a dictionary
        /// </summary>
        /// <param name="obj">The object to make fingerprint of</param>
        /// <param name="properties">Set of property names to get from the object</param>
        public ObjectPropertiesFingerprint(object obj, IEnumerable<string> properties)
        {
            Contract.Requires(obj != null);
            Contract.Requires(properties != null);

            var T = obj.GetType();
            values = new Dictionary<string, object>();
            foreach (var propertyName in properties)
            {
                var propertyInfo = T.GetProperty(propertyName);
                var value = propertyInfo.GetValue(obj, index: null);
                values.Add(propertyName, value);
            }
        }

        /// <summary>
        /// Constructs the fingerprint by deserializing it from a stream containing data
        /// previously created by the <see cref="Save"/> method.
        /// </summary>
        /// <param name="serializer">The serialization implementation to be used</param>
        /// <param name="sourceStream">Deserialization stream</param>
        public ObjectPropertiesFingerprint(IProtocolSerializer serializer, Stream sourceStream)
            : this(serializer.Deserialize<ObjectPropertiesProtocol>(sourceStream))
        {
            Contract.Requires(serializer != null);
            Contract.Requires(sourceStream != null);
        }

        /// <summary>
        /// Constructs the fingerprint based on the deserialized protocol data
        /// </summary>
        /// <param name="proto">The protocol data which was deserialized from a stream</param>
        public ObjectPropertiesFingerprint(ObjectPropertiesProtocol proto)
        {
            values = proto.Values;
        }

        /// <summary>
        /// Saves the fingerprint to the given target stream
        /// </summary>
        /// <param name="serializer">The serializer implementation to be used</param>
        /// <param name="targetStream">The stream to be used when serializing the fingerprint</param>
        public void Save(IProtocolSerializer serializer, Stream targetStream)
        {
            serializer.Serialize(targetStream, Protocol);
        }

        /// <summary>
        /// Gets the raw data used for serialization
        /// </summary>
        public IDependencyFingerprintProtocol Protocol
        {
            get
            {
                return new ObjectPropertiesProtocol
                    {
                        Values = values
                    };
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ObjectPropertiesFingerprint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return values.SequenceEqual(other.values);
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
            var opf = other as ObjectPropertiesFingerprint;
            return opf != null && Equals(opf);
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
            if (obj.GetType() != GetType()) return false;
            return Equals((ObjectPropertiesFingerprint) obj);
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
            return values.Aggregate(
                11, (a, pair) => a ^ pair.Key.GetHashCode() ^ pair.Value.Return(x => x.GetHashCode(), 17));
        }

        /// <summary>
        /// Equality test
        /// </summary>
        public static bool operator ==(ObjectPropertiesFingerprint left, ObjectPropertiesFingerprint right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality test
        /// </summary>
        public static bool operator !=(ObjectPropertiesFingerprint left, ObjectPropertiesFingerprint right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("{");
            foreach (var pair in values)
            {
                sb.AppendFormat("\t{0}: {1}\n", pair.Key, pair.Value);
            }
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}