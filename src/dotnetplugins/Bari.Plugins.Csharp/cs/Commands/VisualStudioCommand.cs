using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Commands;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.Build;
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

        private readonly IResolutionRoot root;
        private readonly IFileSystemDirectory targetDir;

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

Optionally bari can start and load the generated solution into Visual Studio immediately:

Example: `bari vs --open HelloWorld`

If called without any module or product name, it adds *every module* to the generated solution.
";
            }
        }

        /// <summary>
        /// Initializes the command
        /// </summary>
        /// <param name="root">The path to resolve instances</param>
        /// <param name="targetDir">Target root directory</param>
        public VisualStudioCommand(IResolutionRoot root, [TargetRoot] IFileSystemDirectory targetDir)
        {
            this.root = root;
            this.targetDir = targetDir;
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public void Run(Suite suite, string[] parameters)
        {
            bool openSolution = false;
            if (parameters.Length > 0)
            {
                if (parameters[0] == "--open")
                    openSolution = true;

                if (parameters.Length == 2)
                {
                    var module = suite.Modules.FirstOrDefault(m => String.Equals(m.Name, parameters.Last(), StringComparison.InvariantCultureIgnoreCase));
                    if (module == null)
                        throw new InvalidCommandParameterException("vs",
                                                                   String.Format(
                                                                       "The parameter `{0}` is not a valid module name!",
                                                                       parameters.Last()));

                    Run(module, openSolution);    
                }
                else
                {
                    throw new InvalidCommandParameterException("vs", "Must be called with zero, one or two parameters");
                }
            }

            Run(suite.Modules, openSolution);
        }

        private void Run(Module module, bool openSolution)
        {
            Run(new[] {module}, openSolution);
        }

        private void Run(IEnumerable<Module> modules, bool openSolution)
        {
            var buildContext = root.Get<IBuildContext>();
            var slnBuilder = root.Get<SlnBuilder>(new ConstructorArgument("projects", modules.SelectMany(m => m.Projects.Concat(m.TestProjects))));
            slnBuilder.AddToContext(buildContext);
            
            buildContext.Run(slnBuilder);

            if (openSolution)
            {                
                var slnRelativePath = buildContext.GetResults(slnBuilder).FirstOrDefault();
                if (slnRelativePath != null)
                {
                    log.InfoFormat("Opening {0} with Visual Studio...", slnRelativePath);

                    var localTargetDir = targetDir as LocalFileSystemDirectory;
                    if (localTargetDir != null)
                    {
                        Process.Start(Path.Combine(localTargetDir.AbsolutePath, slnRelativePath));
                    }
                    else
                    {
                        log.Warn("The --open command only works with local target directory!");
                    }
                }
            }
        }
    }
}