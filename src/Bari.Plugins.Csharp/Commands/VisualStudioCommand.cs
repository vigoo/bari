using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Commands;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.VisualStudio;

namespace Bari.Plugins.Csharp.Commands
{
    /// <summary>
    /// Implements the 'visualstudio' command, which generates visual studio solution and project
    /// files for a given module or product, and launches Microsoft Visual Studio loading the generated
    /// solution.
    /// </summary>
    public class VisualStudioCommand: ICommand
    {
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IProjectGuidManagement projectGuidManagement;

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "visualstudio"; }
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

Example: `bari visualstudio HelloWorld`
";
            }
        }

        /// <summary>
        /// Initializes the command
        /// </summary>
        /// <param name="suiteRoot">Root directory of the suite</param>
        /// <param name="projectGuidManagement">The project-guid mapper to be used</param>
        public VisualStudioCommand([SuiteRoot] IFileSystemDirectory suiteRoot, IProjectGuidManagement projectGuidManagement)
        {
            this.suiteRoot = suiteRoot;
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
                throw new InvalidCommandParameterException("visualstudio", "Must be called with one parameter");

            // TODO: project support
            var module = suite.Modules.FirstOrDefault(m => String.Equals(m.Name, parameters[0], StringComparison.InvariantCultureIgnoreCase));
            if (module == null)
                throw new InvalidCommandParameterException("visualstudio", 
                    String.Format("The parameter `{0}` is not a valid module name!", parameters[0]));

            Run(module);
        }

        private void Run(Module module)
        {
            // TODO: these should be dependent actions to be perfomred by the bari engine
            // TODO: generated files should be managed by bari and their validity verified using the source files' last modified date

            var targetDir = suiteRoot.CreateDirectory("target");

            foreach (var project in module.Projects)
            {
                if (project.HasSourceSet("cs"))
                {
                    using (var csproj = targetDir.CreateTextFile(project.Name + ".csproj"))
                    {
                        var generator = new CsprojGenerator(projectGuidManagement, "..", project, csproj);
                        generator.Generate();
                    }
                }
            }

            using (var sln = targetDir.CreateTextFile(module.Name + ".sln"))
            {
                var slnGenerator = new SlnGenerator(projectGuidManagement, module.Projects, sln);
                slnGenerator.Generate();
            }
        }
    }
}