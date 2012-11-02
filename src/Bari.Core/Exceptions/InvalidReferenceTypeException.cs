using System;

namespace Bari.Core.Exceptions
{
    /// <summary>
    /// Thrown if there is no registered handler for a reference type
    /// </summary>
    public class InvalidReferenceTypeException: Exception
    {
        private readonly string referenceType;

        /// <summary>
        /// Gets the unsupported reference's type
        /// </summary>
        public string ReferenceType
        {
            get { return referenceType; }
        }

        /// <summary>
        /// Creates the exception object
        /// </summary>
        /// <param name="referenceType">Reference type which is not supported</param>
        public InvalidReferenceTypeException(string referenceType)
        {
            this.referenceType = referenceType;
        }

        public override string ToString()
        {
            return string.Format("Reference type {0} is not supported", referenceType);
        }
    }
}