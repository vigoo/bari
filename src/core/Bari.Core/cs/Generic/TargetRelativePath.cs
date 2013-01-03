using System;
using System.Monads;

namespace Bari.Core.Generic
{
    /// <summary>
    /// Represents a path relative to the build target's root directory
    /// </summary>
    public class TargetRelativePath : IComparable<TargetRelativePath>, IEquatable<TargetRelativePath>
    {
        private readonly string relativePath;

        /// <summary>
        /// Defines a target relative path from its string form
        /// </summary>
        /// <param name="relativePath"></param>
        public TargetRelativePath(string relativePath)
        {
            this.relativePath = relativePath;
        }

        /// <summary>
        /// Gets the relative path as a string
        /// </summary>
        /// <returns>The target-relative path as a string</returns>
        public static implicit operator string(TargetRelativePath srp)
        {
            return srp.relativePath;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(TargetRelativePath other)
        {
            return String.Compare(relativePath, other.relativePath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(TargetRelativePath other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(relativePath, other.relativePath);
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
            return Equals((TargetRelativePath) obj);
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
            return relativePath.With(p => p.GetHashCode());
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
            return "target://" + relativePath;
        }
    }
}