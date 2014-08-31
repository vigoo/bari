using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.UI;

namespace Bari.Core.Commands
{
    /// <summary>
    /// Implements the <c>info</c> command, providing a textual description of the suite
    /// with all the configured and discovered information.
    /// </summary>
    public class InfoCommand: ICommand
    {
        private readonly IUserOutput output;

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "info"; }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get { return "shows all known information about the current suite"; }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get
            {
                return
@"=Info command=

When used without a parameter, it shows all available information about the current suite.
Example: `bari info`
";
            }
        }

        /// <summary>
        /// If <c>true</c>, the target goal is important for this command and must be explicitly specified by the user 
        /// (if the available goal set is not the default)
        /// </summary>
        public bool NeedsExplicitTargetGoal
        {
            get { return false; }
        }

        /// <summary>
        /// Constructs the info command instance
        /// </summary>
        /// <param name="output">Interface for writing messages to the user</param>
        public InfoCommand(IUserOutput output)
        {
            Contract.Requires(output != null);

            this.output = output;
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public bool Run(Suite suite, string[] parameters)
        {   
            if (parameters.Length != 0)
                throw new InvalidCommandParameterException("info",
                                                           "The 'info' command must be called without parameters!");

            output.Message("*Suite name:* {0}\n", suite.Name);            
            
            output.Message("*Modules:*");
            foreach (var module in suite.Modules)
            {
                PrintModuleDetails(module);
            }

            output.Message("*Products:*");
            foreach (var product in suite.Products)
            {
                PrintProductDetails(product);
            }

            return true;
        }

        private void PrintProductDetails(Product product)
        {
            Contract.Requires(product != null);

            output.Message("  *Name:* {0}", product.Name);
            output.Message("  *Modules:*");
            
            foreach (var module in product.Modules)
            {
                output.Message("    - {0}", module.Name);
            }
        }

        /// <summary>
        /// Prints information about one particular module
        /// </summary>
        /// <param name="module">The module to print information of</param>
        private void PrintModuleDetails(Module module)
        {
            Contract.Requires(module != null);

            output.Message("  *Name:* {0}", module.Name);
            output.Message("  *Projects:*");

            foreach (var project in module.Projects)
            {
                PrintProjectDetails(project);
            }

            output.Message("  *Test projects:*");

            foreach (var project in module.TestProjects)
            {
                PrintProjectDetails(project);
            }
        }

        /// <summary>
        /// Prints information about one particular project
        /// </summary>
        /// <param name="project">The project to print information of</param>
        private void PrintProjectDetails(Project project)
        {
            Contract.Requires(project != null);

            output.Message("    *Name:* {0}", project.Name);
            output.Message("    *Type:* {0}", project.Type);

            if (project.References.Any())
            {
                output.Message("    *References:*");

                foreach (var reference in project.References)
                {
                    output.Message("      `{0}` ({1})", reference.Uri, reference.Type);
                }
            }

            if (project.SourceSets.Any())
            {
                output.Message("    *Source sets:");

                foreach (var sourceSet in project.SourceSets)
                {
                    if (sourceSet.Files.Any())
                        output.Message("      `{0}`\t{1} files", sourceSet.Type, sourceSet.Files.Count());
                }
            }
        }
    }
}