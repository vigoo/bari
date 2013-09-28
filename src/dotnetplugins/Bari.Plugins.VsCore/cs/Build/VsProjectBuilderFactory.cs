using System.Collections.Generic;
using Bari.Core.Build;
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

        /// <summary>
        /// Constructs the project builder factory
        /// </summary>
        /// <param name="slnBuilderFactory">Interface for creating new SLN builders</param>
        /// <param name="msBuildRunnerFactory">Interface to create new MSBuild runners</param>
        public VsProjectBuilderFactory(ISlnBuilderFactory slnBuilderFactory, IMSBuildRunnerFactory msBuildRunnerFactory)
        {
            this.slnBuilderFactory = slnBuilderFactory;
            this.msBuildRunnerFactory = msBuildRunnerFactory;
        }

        /// <summary>
        /// Adds the builders (<see cref="IBuilder"/>) to the given build context which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="projects">Projects to be built</param>
        public IBuilder AddToContext(IBuildContext context, IEnumerable<Project> projects)
        {
            var slnBuilder = slnBuilderFactory.CreateSlnBuilder(projects);
            slnBuilder.AddToContext(context);

            var msbuild = msBuildRunnerFactory.CreateMSBuildRunner(slnBuilder, new TargetRelativePath(slnBuilder.Uid + ".sln"));

            return msbuild;
        }
    }
}