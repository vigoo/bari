using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Commands.Helper;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands
{
    /// <summary>
    /// Implements 'build' command, which runs one or more builder (<see cref="IBuilder"/>) for a <see cref="Project"/>,
    /// <see cref="Module"/> or product.
    /// </summary>
    public class BuildCommand: ICommand
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (BuildCommand));

        private readonly IBuildContextFactory buildContextFactory;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IEnumerable<IProjectBuilderFactory> projectBuilders;
        private readonly ICommandTargetParser targetParser;

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "build"; }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get { return "builds a project, module or product"; }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get { return
@"=Build command=

When used without parameter, it builds every module in the *suite*. 
Example: `bari build`

When used with a *module* or *product* name, it builds the specified module together with every required dependency of it.
Example: `bari build HelloWorldModule`

When used with a *project* name prefixed by its module, it builds the specified project only, together with every required dependency of it.
Example: `bari build HelloWorldModule.HelloWorld`

When the special `--dump` argument is specified, the build is not executed, but the build graph and the dependency graph will be dumped
to GraphViz dot files.
Example: `bari build --dump` or `bari build HelloWorldModule --dump`
"; }
        }

        /// <summary>
        /// Constructs the build command
        /// </summary>
        /// <param name="buildContextFactory">Interface for creating new build contexts</param>
        /// <param name="projectBuilders">The set of registered project builder factories</param>
        /// <param name="targetRoot">Build target root directory </param>
        /// <param name="targetParser">Command target parser implementation to be used</param>
        public BuildCommand(IBuildContextFactory buildContextFactory, IEnumerable<IProjectBuilderFactory> projectBuilders, [TargetRoot] IFileSystemDirectory targetRoot, ICommandTargetParser targetParser)
        {
            this.buildContextFactory = buildContextFactory;
            this.projectBuilders = projectBuilders;
            this.targetRoot = targetRoot;
            this.targetParser = targetParser;
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public void Run(Suite suite, string[] parameters)
        {
            int effectiveLength = parameters.Length;
            bool dumpMode = false;

            if (effectiveLength > 0)
                dumpMode = parameters[effectiveLength - 1] == "--dump";

            if (dumpMode)
                effectiveLength--;

            if (effectiveLength < 2)
            {
                string targetStr;
                if (effectiveLength == 0)
                    targetStr = String.Empty;
                else
                    targetStr = parameters[0];

                try
                {
                    var target = targetParser.ParseTarget(targetStr);
                    RunWithProjects(target, dumpMode);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidCommandParameterException("build", ex.Message);
                }
            }
            else
            {
                throw new InvalidCommandParameterException("build",
                                                           "The 'build' command must be called with zero or one module/project name parameter!");
            }
        }

        private void RunWithProjects(CommandTarget target, bool dumpMode)
        {
            var context = buildContextFactory.CreateBuildContext();

            IBuilder rootBuilder = null;
            var projects = target.Projects.ToList();

            foreach (var projectBuilder in projectBuilders)
            {
                rootBuilder = projectBuilder.AddToContext(context, projects);
                // TODO: we have to make one builder above all the returned project builders and use it as root
            }

            if (dumpMode)
            {
                using (var builderGraph = targetRoot.CreateBinaryFile("builders.dot"))
                    context.Dump(builderGraph, rootBuilder);
            }
            else
            {
                context.Run(rootBuilder);

                var outputs = context.GetResults(rootBuilder);
                foreach (var outputPath in outputs)
                    log.DebugFormat("Generated output for build: {0}", outputPath);

                var productTarget = target as ProductTarget;
                if (productTarget != null)
                {
                    MergeOutputForProduct(productTarget.Product, outputs);
                }
            }
        }

        private void MergeOutputForProduct(Product product, ISet<TargetRelativePath> outputs)
        {            
            log.InfoFormat("Merging {0} files to product output directory {1}...",
                outputs.Count,
                new TargetRelativePath(product.Name));

            var productOutput = targetRoot.GetChildDirectory(product.Name, createIfMissing: true);

            foreach (var sourcePath in outputs)
            {
                using (var source = targetRoot.ReadBinaryFile(sourcePath))
                using (var target = productOutput.CreateBinaryFile(Path.GetFileName(sourcePath)))
                    StreamOperations.Copy(source, target);
            }
        }
    }
}