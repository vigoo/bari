using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.MergingTag;
using Bari.Plugins.VCpp.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio;

namespace Bari.Plugins.VCpp.Build
{
    /// <summary>
    /// A <see cref="IProjectBuilderFactory"/> implementation that creates <see cref="AppConfigBuilder"/>
    /// instances for Cpp projects having a source set named <c>appconfig</c>.
    /// </summary>
    /// <seealso cref="Bari.Core.Build.IProjectBuilderFactory" />
    public class AppConfigProjectBuilderFactory : IProjectBuilderFactory
    {
        private readonly ICoreBuilderFactory coreBuilderFactory;
        private readonly IAppConfigBuilderFactory appConfigBuilderFactory;
        private readonly IEnumerable<ISlnProject> supportedSlnProjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigProjectBuilderFactory"/> class.
        /// </summary>
        /// <param name="coreBuilderFactory">Factory for core builders</param>
        /// <param name="appConfigBuilderFactory">Factory for application config builders</param>
        /// <param name="supportedSlnProjects">All the supported SLN project implementations</param>
        public AppConfigProjectBuilderFactory(ICoreBuilderFactory coreBuilderFactory, IAppConfigBuilderFactory appConfigBuilderFactory, IEnumerable<ISlnProject> supportedSlnProjects)
        {
            this.coreBuilderFactory = coreBuilderFactory;
            this.appConfigBuilderFactory = appConfigBuilderFactory;
            this.supportedSlnProjects = supportedSlnProjects;
        }

        public IBuilder Create(IEnumerable<Core.Model.Project> projects)
        {
            var prjs = projects.ToList();
            IBuilder[] builders = prjs
                .Where(prj => supportedSlnProjects.FirstOrDefault(sprj => sprj.SupportsProject(prj)) is CppSlnProject)
                .Where(prj => prj.HasNonEmptySourceSet("appconfig"))
                .Select(prj => (IBuilder)appConfigBuilderFactory.CreateAppConfigBuilder(prj)).ToArray();

            return coreBuilderFactory.Merge(
               builders,
               new ProjectBuilderTag(
                   String.Format("Application config builders of {0}", String.Join(", ", prjs.Select(p => p.Name))),
                   prjs));
        }
    }
}
