using System;

namespace Bari.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a task cannot be performed because no registered
    /// plugins can handle it.
    /// </summary>
    public class NoPluginForTaskException: Exception
    {
        /// <summary>
        /// Creates the exception object
        /// </summary>
        /// <param name="message">Explanation of the task which cannot be performed</param>
        public NoPluginForTaskException(string message) : base(message)
        {
        }
    }
}