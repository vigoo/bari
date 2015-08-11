using Bari.Core.Build;
using Bari.Core.Build.BuilderStore;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Commands;
using Bari.Core.Commands.Clean;
using Bari.Core.Model.Loader;
using Bari.Plugins.Csharp.Build;
using Bari.Plugins.Csharp.Build.BuilderStore;
using Bari.Plugins.Csharp.Commands.Clean;
using Bari.Plugins.Csharp.Model;
using Bari.Plugins.Csharp.Model.Loader;
using Bari.Plugins.Csharp.VisualStudio;
using Bari.Plugins.Csharp.VisualStudio.CsprojSections;
using Bari.Plugins.VsCore.Build;
using Bari.Plugins.VsCore.Commands;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;
using Bari.Plugins.VsCore.VisualStudio.SolutionName;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Bari.Plugins.Csharp
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
            log.Info("Csharp plugin loaded");

            Bind<ICommand>().To<VisualStudioCommand>().Named("vs");

            Bind<ICleanExtension>().To<CsprojCleaner>();

            Bind<IProjectGuidManagement>().To<DefaultProjectGuidManagement>().InSingletonScope();
            Bind<ICsprojBuilderFactory>().ToFactory();

            Bind<ISlnProject>().To<CsharpSlnProject>();

            Bind<IMSBuildProjectSection>().To<PropertiesSection>().WhenInjectedInto<CsprojGenerator>(); 
            Bind<IMSBuildProjectSection>().To<ReferencesSection>().WhenInjectedInto<CsprojGenerator>().WithConstructorArgument("sourceSetName", "cs"); 
            Bind<IMSBuildProjectSection>().To<SourceItemsSection>().WhenInjectedInto<CsprojGenerator>(); 
            Bind<IMSBuildProjectSection>().To<EmbeddedResourcesSection>().WhenInjectedInto<CsprojGenerator>(); 
            Bind<IMSBuildProjectSection>().To<VersionSection>().WhenInjectedInto<CsprojGenerator>(); 
            Bind<IMSBuildProjectSection>().To<WCFMetadataSection>().WhenInjectedInto<CsprojGenerator>(); 
            
            Bind<IYamlProjectParametersLoader>().To<CsharpParametersLoader>();
            Bind<IYamlProjectParametersLoader>().To<WPFParametersLoader>();

            Bind<ISuiteContentsAnalyzer>().To<DefaultSuiteContentsAnalyzer>();
            Bind<ISlnNameGenerator>().To<HashBasedSlnNameGenerator>().WhenInjectedInto<ReadableSlnNameGenerator>();
            Bind<ISlnNameGenerator>().To<ReadableSlnNameGenerator>();

            var protocolRegistry = Kernel.Get<IDependencyFingerprintProtocolRegistry>();
            protocolRegistry.RegisterEnum(i => (CsharpLanguageVersion)i);

            var store = Kernel.Get<IBuilderStore>();
            var csprojBuilderFactory = Kernel.Get<ICsprojBuilderFactory>();
            Rebind<ICsprojBuilderFactory>().ToConstant(
                new StoredCsprojBuilderFactory(csprojBuilderFactory, store));
        }
    }
}
