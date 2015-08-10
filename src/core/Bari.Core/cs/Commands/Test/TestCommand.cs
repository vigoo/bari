using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Commands.Helper;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.UI;

namespace Bari.Core.Commands.Test
{
    /// <summary>
    /// Implements 'test' command, which builds and executes test projects 
    /// </summary>
    public class TestCommand : ICommand, IHasBuildTarget
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TestCommand));

        private readonly IBuildContextFactory buildContextFactory;
        private readonly ICoreBuilderFactory coreBuilderFactory;
        private readonly IFileSystemDirectory targetRoot;
        private readonly ICommandTargetParser targetParser;
        private readonly IEnumerable<IProjectBuilderFactory> projectBuilders;
        private readonly IEnumerable<ITestRunner> testRunners;       
        private readonly IUserOutput output;
        private string lastBuildTarget;

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

When used with a *module* or *product* name, it builds and runs the tests belonging to the specified module(s).
Example: `bari test HelloWorldModule`

When used with a *project* name prefixed by its module, it builds and runs the tests in defined in the given project only.
Example: `bari test HelloWorldModule.HelloWorldTest`

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
        /// <param name="output">Output interface for the dependency dump functionality</param>
        /// <param name="targetParser">User-given target string parser</param>
        /// <param name="coreBuilderFactory">Factory for core builder types</param>
        public TestCommand(IBuildContextFactory buildContextFactory, [TargetRoot] IFileSystemDirectory targetRoot, IEnumerable<IProjectBuilderFactory> projectBuilders, IEnumerable<ITestRunner> testRunners, IUserOutput output, ICommandTargetParser targetParser, ICoreBuilderFactory coreBuilderFactory)
        {
            this.buildContextFactory = buildContextFactory;
            this.targetRoot = targetRoot;
            this.projectBuilders = projectBuilders;
            this.testRunners = testRunners;
            this.output = output;
            this.targetParser = targetParser;
            this.coreBuilderFactory = coreBuilderFactory;
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
            bool dumpDepsMode = false;

            if (effectiveLength > 0)
            {
                dumpMode = parameters[effectiveLength - 1] == "--dump";
                dumpDepsMode = parameters[effectiveLength - 1] == "--dump-deps";
            }

            if (dumpMode || dumpDepsMode)
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
                    lastBuildTarget = targetStr;
                    var target = targetParser.ParseTarget(targetStr);

                    var projects = target.TestProjects.ToList();

                    var tests = suite.HasParameters("test") ? suite.GetParameters<Tests>("test") : new Tests();
                    var buildOutputs = RunWithProjects(projects, dumpMode, dumpDepsMode).ToList();

                    if (buildOutputs.Any())
                        return RunTests(tests, projects, buildOutputs);
                    else
                        return false;
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidCommandParameterException("build", ex.Message);
                }
            }
            else
            {
                throw new InvalidCommandParameterException("test", "Test must be called without any parameters");
            }
        }

        private bool RunTests(Tests tests, IEnumerable<TestProject> projects, IEnumerable<TargetRelativePath> buildOutputs)
        {
            var testProjects = projects as List<TestProject> ?? projects.ToList();
            var buildOutputPaths = buildOutputs as List<TargetRelativePath> ?? buildOutputs.ToList();

            return testRunners
                .Where(testRunner => IsRunnerEnabled(tests, testRunner))
                .Select(testRunner => testRunner.Run(testProjects, buildOutputPaths))
                .All(result => result);
        }

        private bool IsRunnerEnabled(Tests tests, ITestRunner runner)
        {
            bool enabled = tests.IsRunnerEnabled(runner.Name);

            log.DebugFormat("Test runner {0} is {1}", runner.Name, enabled ? "enabled" : "disabled");

            return enabled;
        }

        private IEnumerable<TargetRelativePath> RunWithProjects(IEnumerable<TestProject> projects, bool dumpMode, bool dumpDepsMode)
        {
            var context = buildContextFactory.CreateBuildContext();

            IBuilder rootBuilder = coreBuilderFactory.Merge(
                projectBuilders
                    .Select(pb => pb.Create(projects))
                    .Where(b => b != null).ToArray());

            if (dumpMode)
            {
                using (var builderGraph = targetRoot.CreateBinaryFile("builders.dot"))
                    context.Dump(builderGraph, rootBuilder);

                return new TargetRelativePath[0];
            } 
            else if (dumpDepsMode)
            {
                context.DumpDependencies(rootBuilder, output);

                return new TargetRelativePath[0];
            }
            else 
            {
                var result = context.Run(rootBuilder);
                if (result.Count != 0)
                {
                    return context.GetResults(rootBuilder);
                }
                else
                {
                    log.WarnFormat("No tests to run");
                    return new TargetRelativePath[0];
                }                
            }        
        }

        public string BuildTarget
        {
            get { return lastBuildTarget; }
        }
    }
}