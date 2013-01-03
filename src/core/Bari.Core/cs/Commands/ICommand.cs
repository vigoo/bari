using System;
using System.Diagnostics.Contracts;
using Bari.Core.Model;

namespace Bari.Core.Commands
{
    /// <summary>
    /// Describes a bari command which can be performed on a suite
    /// </summary>
    [ContractClass(typeof(ICommandContracts))]
    public interface ICommand
    {
        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        string Help { get; }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        void Run(Suite suite, string[] parameters);
    }

    /// <summary>
    /// Contracts for the <see cref="ICommand"/> interface
    /// </summary>
    [ContractClassFor(typeof(ICommand))]
    public abstract class ICommandContracts: ICommand
    {
        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get
            {
                Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<string>()));
                return null; // dummy value
            }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get
            {
                Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<string>()));
                return null; // dummy value
            }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get
            {
                Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<string>()));
                return null; // dummy value
            }
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public void Run(Suite suite, string[] parameters)
        {
            Contract.Requires(suite != null);
            Contract.Requires(parameters != null);
        }
    }
}