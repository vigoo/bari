using System;

namespace Bari.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when the suite specification to be load is invalid
    /// </summary>
    public class InvalidSpecificationException: Exception
    {
        /// <summary>
        /// Creates the exception object
        /// </summary>
        /// <param name="message">Message describing the specification error</param>
        public InvalidSpecificationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates the exception object
        /// </summary>
        /// <param name="message">Message describing the specification error</param>
        /// <param name="innerException">Details of the error</param>
        public InvalidSpecificationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}