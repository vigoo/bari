using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.MergingTag;
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
        private readonly ICoreBuilderFactory coreBuilderFactory;
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
        /// <param name="coreBuilderFactory">Factory to create core builder instances</param>
        public VsProjectBuilderFactory(Suite suite, ISlnBuilderFactory slnBuilderFactory, IMSBuildRunnerFactory msBuildRunnerFactory, IReferenceBuilderFactory referenceBuilderFactory, 
            [TargetRoot] IFileSystemDirectory targetRoot, IEnumerable<IPostProcessorFactory> postProcessorFactories, ICoreBuilderFactory coreBuilderFactory)
        {
            this.suite = suite;
            this.slnBuilderFactory = slnBuilderFactory;
            this.msBuildRunnerFactory = msBuildRunnerFactory;
            this.referenceBuilderFactory = referenceBuilderFactory;
            this.targetRoot = targetRoot;
            this.postProcessorFactories = postProcessorFactories;
            this.coreBuilderFactory = coreBuilderFactory;
        }

        /// <summary>
        /// Creates a builder (<see cref="IBuilder"/>) which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="projects">Projects to be built</param>
        public IBuilder Create(IEnumerable<Project> projects)
        {
            var prjs = projects.ToArray();

            var slnBuilder = GenerateSolutionFile(prjs);
            var msbuild = BuildSolution(slnBuilder);
            
            var copyRuntimeDeps = CopyRuntimeDependencies(prjs);
            var result = MergeSteps(copyRuntimeDeps.ToList(), msbuild, prjs);

            return RunPostProcessors(prjs, result, projects);
        }

        private IBuilder MergeSteps(IList<IBuilder> additionalSteps, MSBuildRunner msbuild, IEnumerable<Project> projects)
        {
            if (additionalSteps.Count > 0)
            {
                return coreBuilderFactory.CreateMergingBuilder(additionalSteps.Concat(new[] {msbuild}), new ProjectBuilderTag(projects));
            }
            else
            {
                return msbuild;
            }
        }

        private IBuilder RunPostProcessors(Project[] prjs, IBuilder input, IEnumerable<Project> projects)
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
                return coreBuilderFactory.CreateMergingBuilder(resultBuilders.Concat(new[] {input}), new ProjectBuilderTag(projects));
            }
        }

        private IEnumerable<IBuilder> CopyRuntimeDependencies(IEnumerable<Project> prjs)
        {
            foreach (var project in prjs)
            {
                foreach (var reference in project.References.Where(r => r.Type == ReferenceType.Runtime))
                {
                    var refBuilder = CreateReferenceBuilder(project, reference);

                    if (refBuilder.IsEffective)
                    {
                        var refDeploy = CreateRuntimeReferenceDeployment(project, refBuilder);
                        yield return refDeploy;
                    }
                }
            }
        }

        private MSBuildRunner BuildSolution(SlnBuilder slnBuilder)
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
            return msbuild;
        }

        private SlnBuilder GenerateSolutionFile(IEnumerable<Project> prjs)
        {
            MSBuildParameters msbuildParams;
            if (suite.HasParameters("msbuild"))
                msbuildParams = suite.GetParameters<MSBuildParameters>("msbuild");
            else
                msbuildParams = new MSBuildParameters();

            // Generating the solution file
            return slnBuilderFactory.CreateSlnBuilder(prjs, msbuildParams.Version);
        }

        private IBuilder CreateRuntimeReferenceDeployment(Project project, IReferenceBuilder refBuilder)
        {
            return coreBuilderFactory.CreateCopyResultBuilder(refBuilder, targetRoot.GetChildDirectory(project.RelativeTargetPath, createIfMissing: true));
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