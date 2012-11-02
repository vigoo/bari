using System;
using System.Diagnostics.Contracts;

namespace Bari.Core.Model
{
    public class Reference : IEquatable<Reference>
    {
        private readonly Uri uri;

        public Uri Uri
        {
            get { return uri; }
        }

        public Reference(Uri uri)
        {
            Contract.Requires(uri != null);

            this.uri = uri;
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
            return uri.Equals(other.uri);
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
            if (obj.GetType() != this.GetType()) return false;
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
            return uri.GetHashCode();
        }

        public static bool operator ==(Reference left, Reference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Reference left, Reference right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return uri.ToString();
        }
    }
}