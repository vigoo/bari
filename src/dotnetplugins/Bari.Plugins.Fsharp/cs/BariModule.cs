using Bari.Core.Build.BuilderStore;
using Bari.Core.Commands.Clean;
using Bari.Core.Model.Loader;
using Bari.Plugins.Fsharp.Build;
using Bari.Plugins.Fsharp.Build.BuilderStore;
using Bari.Plugins.Fsharp.Commands.Clean;
using Bari.Plugins.Fsharp.Model.Loader;
using Bari.Plugins.Fsharp.VisualStudio;
using Bari.Plugins.Fsharp.VisualStudio.FsprojSections;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;
using Ninject;
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

            Bind<ICleanExtension>().To<FsprojCleaner>();

            Bind<IYamlProjectParametersLoader>().To<FsharpParametersLoader>();
            Bind<IYamlProjectParametersLoader>().To<FsharpFileOrderLoader>();

            Bind<IMSBuildProjectSection>().To<PropertiesSection>().WhenInjectedInto<FsprojGenerator>();
            Bind<IMSBuildProjectSection>().To<ReferencesSection>().WhenInjectedInto<FsprojGenerator>().WithConstructorArgument("sourceSetName", "fs");
            Bind<IMSBuildProjectSection>().To<VersionSection>().WhenInjectedInto<FsprojGenerator>(); 
            Bind<IMSBuildProjectSection>().To<SourceItemsSection>().WhenInjectedInto<FsprojGenerator>();

            var store = Kernel.Get<IBuilderStore>();
            var fsprojBuilderFactory = Kernel.Get<IFsprojBuilderFactory>();
            Rebind<IFsprojBuilderFactory>().ToConstant(
                new StoredFsprojBuilderFactory(fsprojBuilderFactory, store));
        }
    }
}
