namespace Bari.Core.UI
{
    /// <summary>
    /// Interface describing the parameters given by the user which specifies the task
    /// to be performed by bari.
    /// </summary>
    public interface IParameters
    {
        /// <summary>
        /// Gets the name of the command to be performed
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Gets the parameters given to the command specified by the <see cref="Command"/> property
        /// </summary>
        string[] CommandParameters { get; }
    }
}