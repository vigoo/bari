using System.Collections.Generic;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Clean
{
    /// <summary>
    /// Implements the 'clean' command, which removes all the generated and cached files
    /// from the suite file system.
    /// </summary>
    public class CleanCommand: ICommand
    {
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IEnumerable<ICleanExtension> extensions;

        /// <summary>
        /// Constructs the command
        /// </summary>
        /// <param name="suiteRoot">Suite root directory</param>
        /// <param name="targetRoot">Target root directory</param>
        /// <param name="extensions">Additional cleaning steps to be performed </param>
        public CleanCommand([SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot, IEnumerable<ICleanExtension> extensions)
        {
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
            this.extensions = extensions;
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
                targetRoot.Delete();
 
                foreach (var cleanExtension in extensions)
                {
                    cleanExtension.Clean();
                }
            }
            else
            {
                throw new InvalidCommandParameterException("clean",
                                                           "The 'clean' command must be called without any parameters!");
            }
        }
    }
}