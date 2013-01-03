using System;
using System.Diagnostics.Contracts;

namespace Bari.Core.Build.Cache
{
    /// <summary>
    /// Object used as a key in build cache
    /// </summary>
    public struct BuildKey : IEquatable<BuildKey>
    {
        private readonly Type type;
        private readonly string uid;

        /// <summary>
        /// Gets the builder type
        /// </summary>
        public Type Type
        {
            get
            {
                Contract.Ensures(Contract.Result<Type>() != null);
                
                return type;
            }
        }

        /// <summary>
        /// Gets the builder's unique ID 
        /// </summary>
        public string Uid
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return uid;
            }
        }

        /// <summary>
        /// Creates the key
        /// </summary>
        /// <param name="type">Builder key</param>
        /// <param name="uid">Builder's unique identifier in case if more than one builder of the same type is meaningful</param>
        public BuildKey(Type type, string uid)
        {
            Contract.Requires(type != null);
            Contract.Requires(uid != null);

            this.type = type;
            this.uid = uid;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(BuildKey other)
        {
            return type == other.type && string.Equals(uid, other.uid);
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
            return obj is BuildKey && Equals((BuildKey) obj);
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
            unchecked
            {
                return (type.GetHashCode()*397) ^ uid.GetHashCode();
            }
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(BuildKey left, BuildKey right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(BuildKey left, BuildKey right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            if (uid.Length > 0)
                return type.FullName + "__" + uid;
            else
                return type.FullName;
        }
    }
}