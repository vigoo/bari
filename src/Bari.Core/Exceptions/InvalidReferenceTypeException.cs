using System;

namespace Bari.Core.Exceptions
{
    /// <summary>
    /// Thrown if there is no registered handler for a reference type
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/></PermissionSet>
        public override string ToString()
        {
            return string.Format("Reference type {0} is not supported", referenceType);
        }
    }
}