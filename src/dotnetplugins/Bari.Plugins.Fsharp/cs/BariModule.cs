using Bari.Core.Model.Loader;
using Bari.Plugins.Fsharp.Build;
using Bari.Plugins.Fsharp.Model.Loader;
using Bari.Plugins.Fsharp.VisualStudio;
using Bari.Plugins.Fsharp.VisualStudio.FsprojSections;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Bari.Plugins.Fsharp
{
    /// <summary>
    /// The module definition of this bari plugin
    /// </summary>
    public class BariModule: NinjectModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (BariModule));

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            log.Info("Fsharp plugin loaded");

            Bind<ISlnProject>().To<FsharpSlnProject>();
            Bind<IFsprojBuilderFactory>().ToFactory();

            Bind<IYamlProjectParametersLoader>().To<FsharpParametersLoader>();

            Bind<IMSBuildProjectSection>().To<PropertiesSection>().WhenInjectedInto<FsprojGenerator>();
            Bind<IMSBuildProjectSection>().To<ReferencesSection>().WhenInjectedInto<FsprojGenerator>(); 
            Bind<IMSBuildProjectSection>().To<SourceItemsSection>().WhenInjectedInto<FsprojGenerator>(); 
            Bind<IMSBuildProjectSection>().To<VersionSection>().WhenInjectedInto<FsprojGenerator>(); 
        }
    }
}
