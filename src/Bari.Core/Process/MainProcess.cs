using System.Diagnostics.Contracts;
using System.Reflection;
using Bari.Core.Commands;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.UI;
using Ninject;
using Ninject.Syntax;

namespace Bari.Core.Process
{
    /// <summary>
    /// This class controls the main process which bari can perform
    /// 
    /// <para>
    /// It is resposinble to load a suite model and perform a command with all the necessary
    /// setup.
    /// </para>
    /// </summary>
    public class MainProcess
    {
        private readonly IUserOutput output;
        private readonly IParameters parameters;
        private readonly IResolutionRoot root;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(output != null);
            Contract.Invariant(parameters != null);
            Contract.Invariant(root != null);
        }

        /// <summary>
        /// Initializes the main bari process
        /// </summary>
        /// <param name="output">User output interface to write messages to</param>
        /// <param name="parameters">User defined parameters describing the process to be performed</param>
        /// <param name="root">Path to resolve instances</param>
        public MainProcess(IUserOutput output, IParameters parameters, IResolutionRoot root)
        {
            Contract.Requires(output != null);
            Contract.Requires(parameters != null);
            Contract.Requires(root != null);

            this.output = output;
            this.parameters = parameters;
            this.root = root;
        }

        /// <summary>
        /// Runs the main bari process
        /// </summary>
        public void Run()
        {
            output.Message("bari version {0}\n", Assembly.GetAssembly(typeof(MainProcess)).GetName().Version.ToString());

            var suite = root.Get<Suite>(); // TODO

            var cmd = root.TryGet<ICommand>(parameters.Command);
            if (cmd != null)
            {
                cmd.Run(suite, parameters.CommandParameters);
            }
            else
            {
                throw new InvalidCommandException(parameters.Command, "Unknown command");
            }
        }
    }
}
