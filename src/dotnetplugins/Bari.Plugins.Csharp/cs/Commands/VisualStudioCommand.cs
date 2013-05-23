using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Commands;
using Bari.Core.Commands.Helper;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.Build;
using Bari.Plugins.VsCore.Build;

namespace Bari.Plugins.Csharp.Commands
{
    /// <summary>
    /// Implements the 'vs' command, which generates visual studio solution and project
    /// files for a given module or product, and launches Microsoft Visual Studio loading the generated
    /// solution.
    /// </summary>
    public class VisualStudioCommand : ICommand
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(VisualStudioCommand));

        private readonly IBuildContextFactory buildContextFactory;
        private readonly ISlnBuilderFactory slnBuilderFactory;
        private readonly IFileSystemDirectory targetDir;
        private readonly ICommandTargetParser targetParser;

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
        /// <param name="buildContextFactory">Interface to create new build contexts</param>
        /// <param name="slnBuilderFactory">Interface to create new SLN builders</param>
        /// <param name="targetDir">Target root directory</param>
        /// <param name="targetParser">Parser used for parsing the target parameter</param>
        public VisualStudioCommand(IBuildContextFactory buildContextFactory, ISlnBuilderFactory slnBuilderFactory, [TargetRoot] IFileSystemDirectory targetDir, ICommandTargetParser targetParser)
        {
            this.buildContextFactory = buildContextFactory;
            this.slnBuilderFactory = slnBuilderFactory;
            this.targetDir = targetDir;
            this.targetParser = targetParser;
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public void Run(Suite suite, string[] parameters)
        {
            bool openSolution = false;
            string targetStr;

            if (parameters.Length == 0)
                targetStr = String.Empty;
            else if (parameters.Length < 3)
            {
                if (parameters[0] == "--open")
                    openSolution = true;

                targetStr = parameters.Last();
            }
            else
            {
                throw new InvalidCommandParameterException("vs", "Must be called with zero, one or two parameters");
            }

            try
            {
                var target = targetParser.ParseTarget(targetStr);
                Run(target, openSolution);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidCommandParameterException("vs", ex.Message);
            }
        }

        private void Run(CommandTarget target, bool openSolution)
        {
            Run(target.Projects.Concat(target.TestProjects), openSolution);
        }

        private void Run(IEnumerable<Project> projects, bool openSolution)
        {
            var buildContext = buildContextFactory.CreateBuildContext();
            var slnBuilder = slnBuilderFactory.CreateSlnBuilder(projects);
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