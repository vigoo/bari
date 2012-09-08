using Bari.Core.Model.Loader;

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

        /// <summary>
        /// Gets the name (or url, etc.) of the suite specification to be loaded.
        /// 
        /// <para>Every registered <see cref="IModelLoader"/> will be asked to interpret the
        /// given suite name.</para>
        /// </summary>
        string Suite { get; }
    }
}