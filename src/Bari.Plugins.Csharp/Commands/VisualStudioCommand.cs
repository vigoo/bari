using System;
using System.Linq;
using Bari.Core.Commands;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.Build;
using Bari.Plugins.Csharp.VisualStudio;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Bari.Plugins.Csharp.Commands
{
    /// <summary>
    /// Implements the 'vs' command, which generates visual studio solution and project
    /// files for a given module or product, and launches Microsoft Visual Studio loading the generated
    /// solution.
    /// </summary>
    public class VisualStudioCommand: ICommand
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (VisualStudioCommand));

        private readonly IFileSystemDirectory targetDir;
        private readonly IResolutionRoot root;
        private readonly IProjectGuidManagement projectGuidManagement;

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "vs"; }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get { return "generates visual studio solution and projects for a module"; }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get
            {
                return
@"= VisualStudio command=

Generates visual studio solution and project files for the given module or product, 
and launches Visual Studio to open them.

Example: `bari vs HelloWorld`
";
            }
        }

        /// <summary>
        /// Initializes the command
        /// </summary>
        /// <param name="targetDir">The root directory of the build target, where the visual studio files shoulbe be put</param>
        /// <param name="root">The path to resolve instances</param>
        /// <param name="projectGuidManagement">The project-guid mapper to be used</param>
        public VisualStudioCommand([TargetRoot] IFileSystemDirectory targetDir, IResolutionRoot root, IProjectGuidManagement projectGuidManagement)
        {
            this.targetDir = targetDir;
            this.root = root;
            this.projectGuidManagement = projectGuidManagement;
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public void Run(Suite suite, string[] parameters)
        {
            if (parameters.Length != 1)
                throw new InvalidCommandParameterException("vs", "Must be called with one parameter");

            // TODO: project support
            var module = suite.Modules.FirstOrDefault(m => String.Equals(m.Name, parameters[0], StringComparison.InvariantCultureIgnoreCase));
            if (module == null)
                throw new InvalidCommandParameterException("vs", 
                    String.Format("The parameter `{0}` is not a valid module name!", parameters[0]));

            Run(module);
        }

        private void Run(Module module)
        {
            var builder = root.Get<SlnBuilder>(new ConstructorArgument("projects", module.Projects));
            var outputs = builder.Run();

            foreach (var outputPath in outputs)
                log.InfoFormat("Generated output solution for module {0}: {1}", module.Name, outputPath);
        }
    }
}