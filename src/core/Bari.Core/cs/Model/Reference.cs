using System;
using System.Diagnostics.Contracts;
using Bari.Core.Build;

namespace Bari.Core.Model
{
    /// <summary>
    /// Represent a project reference
    /// 
    /// <para>A project reference is described by an URI, where the scheme part (before ://)
    /// identifies the <see cref="IReferenceBuilder"/> to be used, an the other components
    /// of the URI are processed by the selected builder.</para>
    /// </summary>
    public class Reference : IEquatable<Reference>
    {
        private readonly Uri uri;
        private readonly ReferenceType type;

        /// <summary>
        /// Gets the reference URI
        /// </summary>
        public Uri Uri
        {
            get { return uri; }
        }

        /// <summary>
        /// Gets the reference type
        /// </summary>
        public ReferenceType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Constructs the reference
        /// </summary>
        /// <param name="uri">The reference URI</param>
        /// <param name="type">Reference type</param>
        public Reference(Uri uri, ReferenceType type)
        {
            Contract.Requires(uri != null);

            this.uri = uri;
            this.type = type;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Reference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return uri.Equals(other.uri) && type == other.type;
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
            return Equals((Reference) obj);
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
            return uri.GetHashCode() ^ type.GetHashCode();
        }

        /// <summary>
        /// Equality operator for references
        /// </summary>
        public static bool operator ==(Reference left, Reference right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality operator for references
        /// </summary>
        public static bool operator !=(Reference left, Reference right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0} ({1})", uri, type);
        }
    }
}