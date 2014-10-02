using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Commands.Helper;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.UI;

namespace Bari.Core.Commands
{
    /// <summary>
    /// Implements 'build' command, which runs one or more builder (<see cref="IBuilder"/>) for a <see cref="Project"/>,
    /// <see cref="Module"/> or product.
    /// </summary>
    public class BuildCommand : ICommand, IHasBuildTarget
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (BuildCommand));

        private readonly IBuildContextFactory buildContextFactory;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IEnumerable<IProjectBuilderFactory> projectBuilders;
        private readonly ICommandTargetParser targetParser;
        private readonly IUserOutput output;
        private readonly IEnumerable<IPostProcessorFactory> postProcessorFactories;
        private string lastTargetStr;

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
        /// If <c>true</c>, the target goal is important for this command and must be explicitly specified by the user 
        /// (if the available goal set is not the default)
        /// </summary>
        public bool NeedsExplicitTargetGoal
        {
            get { return true; }
        }

        /// <summary>
        /// Constructs the build command
        /// </summary>
        /// <param name="buildContextFactory">Interface for creating new build contexts</param>
        /// <param name="projectBuilders">The set of registered project builder factories</param>
        /// <param name="targetRoot">Build target root directory </param>
        /// <param name="targetParser">Command target parser implementation to be used</param>
        public BuildCommand(IBuildContextFactory buildContextFactory, IEnumerable<IProjectBuilderFactory> projectBuilders, [TargetRoot] IFileSystemDirectory targetRoot, ICommandTargetParser targetParser, IUserOutput output, IEnumerable<IPostProcessorFactory> postProcessorFactories)
        {
            this.buildContextFactory = buildContextFactory;
            this.projectBuilders = projectBuilders;
            this.targetRoot = targetRoot;
            this.targetParser = targetParser;
            this.output = output;
            this.postProcessorFactories = postProcessorFactories;
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public bool Run(Suite suite, string[] parameters)
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
                    lastTargetStr = targetStr;
                    var target = targetParser.ParseTarget(targetStr);
                    RunWithProjects(target, dumpMode);

                    return true;
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
            log.InfoFormat("Building...");

            var context = buildContextFactory.CreateBuildContext();

            var projects = target.Projects.ToList();

            IBuilder rootBuilder = projectBuilders.Select(pb => pb.AddToContext(context, projects))
                                      .Where(b => b != null).ToArray().Merge();
            rootBuilder.AddToContext(context);

            if (dumpMode)
            {
                using (var builderGraph = targetRoot.CreateBinaryFile("builders.dot"))
                    context.Dump(builderGraph, rootBuilder);
            }
            else
            {
                var productTarget = target as ProductTarget;
                if (productTarget != null)
                {
                    rootBuilder = AddProductBuildStep(context, productTarget.Product, rootBuilder);
                }

                context.Run(rootBuilder);

                var outputs = context.GetResults(rootBuilder);
                foreach (var outputPath in outputs)
                    log.DebugFormat("Generated output for build: {0}", outputPath);
            }
            
            output.Message("Build completed.");
        }

        private IBuilder AddProductBuildStep(IBuildContext context, Product product, IBuilder rootBuilder)
        {
            var productOutput = targetRoot.GetChildDirectory(product.Name, createIfMissing: true);

            var copyResultsStep = new CopyResultBuilder(rootBuilder, targetRoot, productOutput);
            copyResultsStep.AddToContext(context);

            var resultBuilders = new List<IPostProcessor>();

            if (product.PostProcessors.Any())
            {
                var factories = postProcessorFactories.ToList();                

                foreach (var pp in product.PostProcessors)
                {
                    var postProcessor =
                        factories.Select(f => f.CreatePostProcessorFor(product, pp, new[] {copyResultsStep}))
                            .FirstOrDefault(p => p != null);
                    if (postProcessor != null)
                    {
                        postProcessor.AddToContext(context);
                        resultBuilders.Add(postProcessor);
                    }
                }
            }
            
            if (resultBuilders.Any())
            {
                var merger = new MergingBuilder(resultBuilders);
                merger.AddToContext(context);
                return merger;
            }
            else
            {
                return copyResultsStep;
            }
        }

        public string BuildTarget
        {
            get { return lastTargetStr; }
        }
    }
}