using System.Diagnostics.Contracts;
using Bari.Core.Model.Loader;

namespace Bari.Core.UI
{
    /// <summary>
    /// Interface describing the parameters given by the user which specifies the task
    /// to be performed by bari.
    /// </summary>
    [ContractClass(typeof(IParametersContracts))]
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

        /// <summary>
        /// True if verbose output should be printed
        /// </summary>
        bool VerboseOutput { get; }

        /// <summary>
        /// Gets the goal name
        /// </summary>
        string Goal { get; }
    }

    /// <summary>
    /// Contracts for <see cref="IParameters"/> interface
    /// </summary>
    [ContractClassFor(typeof(IParameters))]
    public abstract class IParametersContracts: IParameters
    {
        /// <summary>
        /// Gets the name of the command to be performed
        /// </summary>
        public string Command
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                return null; // dummy value
            }
        }

        /// <summary>
        /// Gets the parameters given to the command specified by the <see cref="IParameters.Command"/> property
        /// </summary>
        public string[] CommandParameters
        {
            get
            {
                Contract.Ensures(Contract.Result<string[]>() != null);
                return null; // dummy value
            }
        }

        /// <summary>
        /// Gets the name (or url, etc.) of the suite specification to be loaded.
        /// 
        /// <para>Every registered <see cref="IModelLoader"/> will be asked to interpret the
        /// given suite name.</para>
        /// </summary>
        public string Suite
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                return null; // dummy value
            }
        }

        /// <summary>
        /// True if verbose output should be printed
        /// </summary>
        public abstract bool VerboseOutput { get; }

        public string Goal
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                return null; // dummy value
            }
        }
    }
}