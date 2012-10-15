using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands
{
    /// <summary>
    /// Implements the 'clean' command, which removes all the generated and cached files
    /// from the suite file system.
    /// </summary>
    public class CleanCommand: ICommand
    {
        private readonly IFileSystemDirectory suiteRoot;

        /// <summary>
        /// Constructs the command
        /// </summary>
        /// <param name="suiteRoot">Suite root directory</param>
        public CleanCommand([SuiteRoot] IFileSystemDirectory suiteRoot)
        {
            this.suiteRoot = suiteRoot;
        }

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "clean"; }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get { return "cleans that target dir and the bari cache"; }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get
            {
                return
@"=Clean command=

When used without parameter, it deletes the `target` and `cache` directories.
Example: `bari clean`
"; 
       
            }
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public void Run(Suite suite, string[] parameters)
        {
            if (parameters.Length == 0)
            {
                // TODO: do not use hard coded directory names here
                suiteRoot.DeleteDirectory("target");
                suiteRoot.DeleteDirectory("cache");
            }
            else
            {
                throw new InvalidCommandParameterException("clean",
                                                           "The 'clean' command must be called without any parameters!");
            }
        }
    }
}