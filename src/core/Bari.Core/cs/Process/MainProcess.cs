using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using Bari.Core.Commands;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Core.UI;
using Ninject.Syntax;

namespace Bari.Core.Process
{
    /// <summary>
    /// This class controls the main process which bari can perform
    /// 
    /// <para>
    /// It is responsible to load a suite model and perform a command with all the necessary
    /// setup.
    /// </para>
    /// </summary>
    public class MainProcess
    {
        private readonly IUserOutput output;
        private readonly IParameters parameters;
        private readonly ISuiteLoader loader;
        private readonly ICommandFactory commandFactory;
        private readonly ExplorerRunner explorer;
        private readonly IBindingRoot binding;

        /// <summary>
        /// Initializes the main bari process
        /// </summary>
        /// <param name="output">User output interface to write messages to</param>
        /// <param name="parameters">User defined parameters describing the process to be performed</param>
        /// <param name="loader">The suite model loader implementation to be used</param>
        /// <param name="commandFactory">Factory for command objects</param>
        /// <param name="explorer">Suite explorer runner</param>
        /// <param name="binding">Interface to bind new dependencies</param>
        public MainProcess(IUserOutput output, IParameters parameters, ISuiteLoader loader, ICommandFactory commandFactory, ExplorerRunner explorer, IBindingRoot binding)
        {
            Contract.Requires(output != null);
            Contract.Requires(parameters != null);
            Contract.Requires(commandFactory != null);
            Contract.Requires(loader != null);
            Contract.Requires(explorer != null);

            this.output = output;
            this.parameters = parameters;
            this.loader = loader;
            this.commandFactory = commandFactory;
            this.explorer = explorer;
            this.binding = binding;
        }

        /// <summary>
        /// Runs the main bari process
        /// </summary>
        public bool Run()
        {
            output.Message("bari version {0}\n", Assembly.GetAssembly(typeof(MainProcess)).GetName().Version.ToString());

            var cmdPrereq = commandFactory.CreateCommandPrerequisites(parameters.Command);

            Suite suite;
            if (cmdPrereq == null || cmdPrereq.RequiresSuite)
            {
                suite = loader.Load(parameters.Suite);
                binding.Bind<Suite>().ToConstant(suite);

                explorer.RunAll(suite);
                suite.CheckForWarnings(output);
            }
            else
            {
                suite = new Suite(new LocalFileSystemDirectory(Path.GetTempPath()));
            }

            var cmd = commandFactory.CreateCommand(parameters.Command);
            if (cmd != null)
            {
                binding.Bind<ICommand>().ToConstant(cmd).WhenTargetHas<CurrentAttribute>();
                return cmd.Run(suite, parameters.CommandParameters);
            }
            else
            {
                throw new InvalidCommandException(parameters.Command, "Unknown command");
            }
        }
    }
}
