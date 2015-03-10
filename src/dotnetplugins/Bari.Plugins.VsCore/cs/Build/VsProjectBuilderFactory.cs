using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VsCore.Build
{
    /// <summary>
    /// The <see cref="IProjectBuilderFactory"/> implementation supporting C# projects compiled using MSBuild
    /// </summary>
    public class VsProjectBuilderFactory: IProjectBuilderFactory
    {
        private readonly Suite suite;
        private readonly ISlnBuilderFactory slnBuilderFactory;
        private readonly IMSBuildRunnerFactory msBuildRunnerFactory;
        private readonly IReferenceBuilderFactory referenceBuilderFactory;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IEnumerable<IPostProcessorFactory> postProcessorFactories;

        /// <summary>
        /// Constructs the project builder factory
        /// </summary>
        /// <param name="suite">The active suite</param>
        /// <param name="slnBuilderFactory">Interface for creating new SLN builders</param>
        /// <param name="msBuildRunnerFactory">Interface to create new MSBuild runners</param>
        /// <param name="referenceBuilderFactory">Interface to create new reference builders</param>
        /// <param name="targetRoot">Target root directory</param>
        /// <param name="postProcessorFactories">List of registered post processor factories</param>
        public VsProjectBuilderFactory(Suite suite, ISlnBuilderFactory slnBuilderFactory, IMSBuildRunnerFactory msBuildRunnerFactory, IReferenceBuilderFactory referenceBuilderFactory, 
            [TargetRoot] IFileSystemDirectory targetRoot, IEnumerable<IPostProcessorFactory> postProcessorFactories)
        {
            this.suite = suite;
            this.slnBuilderFactory = slnBuilderFactory;
            this.msBuildRunnerFactory = msBuildRunnerFactory;
            this.referenceBuilderFactory = referenceBuilderFactory;
            this.targetRoot = targetRoot;
            this.postProcessorFactories = postProcessorFactories;
        }

        /// <summary>
        /// Adds the builders (<see cref="IBuilder"/>) to the given build context which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="projects">Projects to be built</param>
        public IBuilder AddToContext(IBuildContext context, IEnumerable<Project> projects)
        {
            var prjs = projects.ToArray();

            var slnBuilder = GenerateSolutionFile(context, prjs);
            var msbuild = BuildSolution(context, slnBuilder);
            
            var additionalSteps = new List<IBuilder>();

            CopyRuntimeDependencies(context, prjs, additionalSteps);
            var result = MergeSteps(context, additionalSteps, msbuild);

            return RunPostProcessors(context, prjs, result);
        }

        private static IBuilder MergeSteps(IBuildContext context, List<IBuilder> additionalSteps, MSBuildRunner msbuild)
        {
            if (additionalSteps.Count > 0)
            {
                var merger = new MergingBuilder(additionalSteps.Concat(new[] {msbuild}));
                merger.AddToContext(context);
                return merger;
            }
            else
            {
                return msbuild;
            }
        }

        private IBuilder RunPostProcessors(IBuildContext context, Project[] prjs, IBuilder input)
        {
            var modules = prjs.Select(p => p.Module).Distinct().ToList();
            var postProcessableItems = prjs.Concat(modules.Cast<IPostProcessorsHolder>()).ToList();

            var factories = postProcessorFactories.ToList();
            var resultBuilders = new List<IPostProcessor>();

            foreach (var ppHolder in postProcessableItems)
            {
                foreach (var pp in ppHolder.PostProcessors)
                {
                    var postProcessor =
                        factories.Select(f => f.CreatePostProcessorFor(ppHolder, pp, new [] { input })).FirstOrDefault(p => p != null);
                    if (postProcessor != null)
                    {
                        postProcessor.AddToContext(context);
                        resultBuilders.Add(postProcessor);
                    }
                }
            }

            if (resultBuilders.Count == 0)
            {
                return input;
            }
            else
            {
                var merger = new MergingBuilder(resultBuilders.Concat(new[] {input}));
                merger.AddToContext(context);
                return merger;
            }
        }

        private void CopyRuntimeDependencies(IBuildContext context, IEnumerable<Project> prjs, List<IBuilder> additionalSteps)
        {
            foreach (var project in prjs)
            {
                foreach (var reference in project.References.Where(r => r.Type == ReferenceType.Runtime))
                {
                    var refBuilder = CreateReferenceBuilder(project, reference);

                    if (refBuilder.IsEffective)
                    {
                        refBuilder.AddToContext(context);

                        var refDeploy = CreateRuntimeReferenceDeployment(context, project, refBuilder);
                        refDeploy.AddToContext(context);

                        additionalSteps.Add(refDeploy);
                    }
                }
            }
        }

        private MSBuildRunner BuildSolution(IBuildContext context, SlnBuilder slnBuilder)
        {
            // Building the solution
            MSBuildParameters msbuildParams;
            if (suite.HasParameters("msbuild"))
                msbuildParams = suite.GetParameters<MSBuildParameters>("msbuild");
            else            
                msbuildParams = new MSBuildParameters();

            var msbuild = msBuildRunnerFactory.CreateMSBuildRunner(
                slnBuilder,
                new TargetRelativePath(String.Empty, slnBuilder.Uid + ".sln"), 
                msbuildParams.Version);
            msbuild.AddToContext(context);
            return msbuild;
        }

        private SlnBuilder GenerateSolutionFile(IBuildContext context, IEnumerable<Project> prjs)
        {
            MSBuildParameters msbuildParams;
            if (suite.HasParameters("msbuild"))
                msbuildParams = suite.GetParameters<MSBuildParameters>("msbuild");
            else
                msbuildParams = new MSBuildParameters();

            // Generating the solution file
            var slnBuilder = slnBuilderFactory.CreateSlnBuilder(prjs, msbuildParams.Version);
            slnBuilder.AddToContext(context);
            return slnBuilder;
        }

        private IBuilder CreateRuntimeReferenceDeployment(IBuildContext context, Project project, IReferenceBuilder refBuilder)
        {
            var copy = new CopyResultBuilder(refBuilder, targetRoot,  targetRoot.GetChildDirectory(project.RelativeTargetPath, createIfMissing: true));
            context.AddBuilder(copy, new[] { refBuilder });
            return copy;
        }

        private IReferenceBuilder CreateReferenceBuilder(Project project, Reference reference)
        {
            var builder = referenceBuilderFactory.CreateReferenceBuilder(reference, project);
            if (builder != null)
            {
                return builder;
            }
            else
                throw new InvalidReferenceTypeException(reference.Uri.Scheme);
        }
    }
}