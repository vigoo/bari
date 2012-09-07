using System;

namespace Bari.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a bari command was called with invalid parameters
    /// </summary>
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
            this.commandName = commandName;
        }
    }
}