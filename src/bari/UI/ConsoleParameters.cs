using System.Diagnostics.Contracts;
using Bari.Core.UI;

namespace Bari.Console.UI
{
    /// <summary>
    /// Implements the <see cref="IParameters"/> interface by parsing the command line    
    /// </summary>
    public class ConsoleParameters: IParameters
    {
        private readonly string cmd;
        private readonly string[] cmdParams;

        /// <summary>
        /// Creates the parameter object and immediately parse the arguments
        /// </summary>
        /// <param name="args">The command line arguments to be parsed</param>
        public ConsoleParameters(string[] args)
        {
            Contract.Requires(args != null);

            if (args.Length > 0)
            {
                cmd = args[0];
                cmdParams = new string[args.Length-1];
                for (int i = 1; i < args.Length; i++)
                    cmdParams[i - 1] = args[i];
            }
            else
            {
                // Default command is 'help':
                cmd = "help";
                cmdParams = new string[0];
            }
        }

        /// <summary>
        /// Gets the name of the command to be performed
        /// </summary>
        public string Command
        {
            get { return cmd; }
        }

        /// <summary>
        /// Gets the parameters given to the command specified by the <see cref="IParameters.Command"/> property
        /// </summary>
        public string[] CommandParameters
        {
            get { return cmdParams; }
        }
    }
}