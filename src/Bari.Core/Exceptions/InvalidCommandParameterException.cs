using System;
using System.Diagnostics.Contracts;

namespace Bari.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a bari command was called with invalid parameters
    /// </summary>
    [Serializable]
    public class InvalidCommandParameterException: Exception
    {
        private readonly string commandName;

        /// <summary>
        /// Gets the name of the command which was called with invalid parameters
        /// </summary>
        public string CommandName
        {
            get { return commandName; }
        }

        /// <summary>
        /// Creates the exeption object
        /// </summary>
        /// <param name="commandName">Name of the command which was called with invalid parameters</param>
        /// <param name="message">Message describing the problem</param>
        public InvalidCommandParameterException(string commandName, string message) : base(message)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(commandName));
            Contract.Requires(message != null);
            Contract.Ensures(CommandName == commandName);

            this.commandName = commandName;
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
            return String.Format("Command '{0}' has invalid parameters: {1}", commandName, Message);
        }
    }
}