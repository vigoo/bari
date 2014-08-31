using System.Linq;
using Bari.Core.Build;
using Bari.Core.Commands.Clean;
using Bari.Core.Commands.Helper;
using Bari.Core.Generic;
using Bari.Core.Model;
using Ninject;

namespace Bari.Core.Commands
{
    /// <summary>
    /// Implements 'rebuild' command, which runs one or more builder (<see cref="IBuilder"/>) for a <see cref="Project"/>,
    /// <see cref="Module"/> or product after cleaning the suite.
    /// </summary>
    public class RebuildCommand : ICommand, IHasBuildTarget
    {
        private readonly ICommand cleanCommand;
        private readonly ICommand buildCommand;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IFileSystemDirectory cacheRoot;

        public RebuildCommand([Named("clean")] ICommand cleanCommand, [Named("build")] ICommand buildCommand, [TargetRoot] IFileSystemDirectory targetRoot, [CacheRoot] IFileSystemDirectory cacheRoot)
        {
            this.cleanCommand = cleanCommand;
            this.buildCommand = buildCommand;
            this.targetRoot = targetRoot;
            this.cacheRoot = cacheRoot;
        }

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "rebuild"; }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get { return "cleans and builds a project, module or product"; }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get
            {
                return
                    @"=Rebuild command=

Rebuild is a combination of the `clean` and `build` commands. The cleaning step always cleans the *whole suite*, not only the build target!

When used without parameter, it cleans and builds every module in the *suite*. 
Example: `bari rebuild`

When used with a *module* or *product* name, it first cleans the *whole suite*, then builds the specified module together with every required dependency of it.
Example: `bari rebuild HelloWorldModule`

When used with a *project* name prefixed by its module, it first cleans the *whole suite*, then builds the specified project only, together with every required dependency of it.
Example: `bari rebuild HelloWorldModule.HelloWorld`

When used with the `--keep-references` option, it keeps the 3rd party references in the cache.
Example: `bari rebuild --keep-references HelloWorldModule`
";
            }
        }

        /// <summary>
        /// If <c>true</c>, the target goal is important for this command and must be explicitly specified by the user 
        /// (if the available goal set is not the default)
        /// </summary>
        public bool NeedsExplicitTargetGoal
        {
            get { return true; }
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public bool Run(Suite suite, string[] parameters)
        {
            var cleanParams = new CleanParameters(new string[0]);

            var cleanParameters = parameters.Where(cleanParams.IsKeepReferencesParameter).ToArray();
            var buildParameters = parameters.Where(p => !cleanParams.IsKeepReferencesParameter(p)).ToArray();

            var cleanSucceeded = cleanCommand.Run(suite, cleanParameters);

            targetRoot.Remake();
            cacheRoot.Remake();

            var buildSucceeded = buildCommand.Run(suite, buildParameters);

            return cleanSucceeded && buildSucceeded;
        }

        public string BuildTarget
        {
            get { return ((IHasBuildTarget)buildCommand).BuildTarget; }
        }
    }
}