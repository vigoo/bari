using System;

namespace Bari.Core.Exceptions
{
    /// <summary>
    /// Exception thrown if a project reference is unsolvable
    /// </summary>
    [Serializable]
    public class InvalidReferenceException: Exception
    {
        /// <summary>
        /// Creates the exception
        /// </summary>
        /// <param name="message">Message with details of the problem</param>
        public InvalidReferenceException(string message) : base(message)
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
            return Message;
        }
    }
}