using System;

namespace Bari.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a task cannot be performed because no registered
    /// plugins can handle it.
    /// </summary>
    [Serializable]
    public class NoPluginForTaskException: Exception
    {
        /// <summary>
        /// Creates the exception object
        /// </summary>
        /// <param name="message">Explanation of the task which cannot be performed</param>
        public NoPluginForTaskException(string message) : base(message)
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