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

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/></PermissionSet>
        public override string ToString()
        {
            return String.Format("Invalid suite specification: {0}", Message);
        }
    }
}