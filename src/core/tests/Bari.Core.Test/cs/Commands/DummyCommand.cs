using Bari.Core.Commands;
using Bari.Core.Model;

namespace Bari.Core.Test.Commands
{
    public class DummyCommand: ICommand
    {
        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help { get; set; }

        public bool NeedsExplicitTargetGoal
        {
            get; set;
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public bool Run(Suite suite, string[] parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}