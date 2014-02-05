using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.Build
{
    /// <summary>
    /// The <see cref="IProjectBuilderFactory"/> implementation supporting C# projects compiled using MSBuild
    /// </summary>
    public class VsProjectBuilderFactory: IProjectBuilderFactory
    {
        private readonly ISlnBuilderFactory slnBuilderFactory;
        private readonly IMSBuildRunnerFactory msBuildRunnerFactory;
        private readonly IReferenceBuilderFactory referenceBuilderFactory;
        private readonly IFileSystemDirectory targetRoot;

        /// <summary>
        /// Constructs the project builder factory
        /// </summary>
        /// <param name="slnBuilderFactory">Interface for creating new SLN builders</param>
        /// <param name="msBuildRunnerFactory">Interface to create new MSBuild runners</param>
        /// <param name="referenceBuilderFactory">Interface to create new reference builders</param>
        public VsProjectBuilderFactory(ISlnBuilderFactory slnBuilderFactory, IMSBuildRunnerFactory msBuildRunnerFactory, IReferenceBuilderFactory referenceBuilderFactory, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.slnBuilderFactory = slnBuilderFactory;
            this.msBuildRunnerFactory = msBuildRunnerFactory;
            this.referenceBuilderFactory = referenceBuilderFactory;
            this.targetRoot = targetRoot;
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

            // Generating the solution file
            var slnBuilder = slnBuilderFactory.CreateSlnBuilder(prjs);
            slnBuilder.AddToContext(context);

            // Building the solution
            var msbuild = msBuildRunnerFactory.CreateMSBuildRunner(slnBuilder, new TargetRelativePath(String.Empty, slnBuilder.Uid + ".sln"));
            msbuild.AddToContext(context);

            // Copying runtime dependencies
            var runtimeDeps = new List<IBuilder>();

            foreach (var project in prjs)
            {
                foreach (var reference in project.References.Where(r => r.Type == ReferenceType.Runtime))
                {
                    var refBuilder = CreateReferenceBuilder(project, reference);

                    if (refBuilder.IsEffective)
                    {
                        refBuilder.AddToContext(context);

                        var refDeploy = CreateRuntimeReferenceDeployment(project, refBuilder, msbuild);
                        refDeploy.AddToContext(context);

                        runtimeDeps.Add(refDeploy);
                    }
                }
            }

            if (runtimeDeps.Count > 0)
            {
                return new MergingBuilder(runtimeDeps.Concat(new [] {  msbuild }));
            }
            else
            {
                return msbuild;
            }
        }

        private IBuilder CreateRuntimeReferenceDeployment(Project project, IReferenceBuilder refBuilder, IBuilder prerequisite)
        {
            return new CopyResultBuilder(refBuilder, targetRoot,  targetRoot.GetChildDirectory(project.Module.Name, createIfMissing: true), new[] { prerequisite });
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