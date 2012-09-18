using System;

namespace Bari.Core.Generic
{
    /// <summary>
    /// Represents a path relative to the suite's root directory
    /// </summary>
    public class SuiteRelativePath: IComparable<SuiteRelativePath>, IEquatable<SuiteRelativePath>
    {
        private readonly string relativePath;

        /// <summary>
        /// Defines a suite relative path from its string form
        /// </summary>
        /// <param name="relativePath"></param>
        public SuiteRelativePath(string relativePath)
        {
            this.relativePath = relativePath;
        }
        
        /// <summary>
        /// Gets the relative path as a string
        /// </summary>
        /// <returns>The suite-relative path as a string</returns>
        public static implicit operator string(SuiteRelativePath srp)
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
        public int CompareTo(SuiteRelativePath other)
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
        public bool Equals(SuiteRelativePath other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(relativePath, other.relativePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SuiteRelativePath) obj);
        }

        public override int GetHashCode()
        {
            return (relativePath != null ? relativePath.GetHashCode() : 0);
        }



        public override string ToString()
        {
            return "suite://" + relativePath;
        }
    }
}