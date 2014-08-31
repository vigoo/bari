using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Test
{
    /// <summary>
    /// Implements 'test' command, which builds and executes test projects 
    /// </summary>
    public class TestCommand : ICommand
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TestCommand));

        private readonly IBuildContextFactory buildContextFactory;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IEnumerable<IProjectBuilderFactory> projectBuilders;
        private readonly IEnumerable<ITestRunner> testRunners;

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "test"; }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get { return "builds and runs the unit tests in the suite"; }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get
            {
                return
@"=Test command=

When used without parameter, it builds and runs every unit test project in the suite. 
Example: `bari test`

When the special `--dump` argument is specified, the tests are not executed, but the build graph and the dependency graph will be dumped
to GraphViz dot files.
Example: `bari test --dump`
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
        /// Constructs the test command
        /// </summary>
        /// <param name="buildContextFactory">Factory interface to create build contexts</param>
        /// <param name="targetRoot">Target file system directory</param>
        /// <param name="projectBuilders">Available project builders</param>
        /// <param name="testRunners">Available test runners</param>
        public TestCommand(IBuildContextFactory buildContextFactory, [TargetRoot] IFileSystemDirectory targetRoot, IEnumerable<IProjectBuilderFactory> projectBuilders, IEnumerable<ITestRunner> testRunners)
        {
            this.buildContextFactory = buildContextFactory;
            this.targetRoot = targetRoot;
            this.projectBuilders = projectBuilders;
            this.testRunners = testRunners;
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

            if (effectiveLength == 0)
            {
                var projects = (from module in suite.Modules
                                from project in module.TestProjects
                                select project).ToList();

                log.InfoFormat("Building the full suite ({0} projects)", projects.Count);

                var buildOutputs = RunWithProjects(projects, dumpMode);
                return RunTests(projects, buildOutputs);
            }
            else
            {
                throw new InvalidCommandParameterException("test", "Test must be called without any parameters");
            }
        }

        private bool RunTests(IEnumerable<TestProject> projects, IEnumerable<TargetRelativePath> buildOutputs)
        {
            var testProjects = projects as List<TestProject> ?? projects.ToList();
            var buildOutputPaths = buildOutputs as List<TargetRelativePath> ?? buildOutputs.ToList();

            return testRunners
                .Select(testRunner => testRunner.Run(testProjects, buildOutputPaths))
                .All(result => result);
        }

        private IEnumerable<TargetRelativePath> RunWithProjects(IEnumerable<TestProject> projects, bool dumpMode)
        {
            var context = buildContextFactory.CreateBuildContext();

            IBuilder rootBuilder = projectBuilders.Select(pb => pb.AddToContext(context, projects))
                                                  .Where(b => b != null).ToArray().Merge();

            if (dumpMode)
            {
                using (var builderGraph = targetRoot.CreateBinaryFile("builders.dot"))
                    context.Dump(builderGraph, rootBuilder);

                return new TargetRelativePath[0];
            }
            else 
            {
                context.Run(rootBuilder);
                return context.GetResults(rootBuilder);
            }        
        }
    }
}