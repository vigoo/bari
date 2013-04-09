using Bari.Core.Build;
using Bari.Core.Commands;
using Bari.Core.Commands.Clean;
using Bari.Core.Model.Loader;
using Bari.Plugins.Csharp.Build;
using Bari.Plugins.Csharp.Commands;
using Bari.Plugins.Csharp.Commands.Clean;
using Bari.Plugins.Csharp.Model.Loader;
using Bari.Plugins.Csharp.Tools;
using Bari.Plugins.Csharp.VisualStudio;
using Bari.Plugins.Csharp.VisualStudio.CsprojSections;
using Bari.Plugins.Csharp.VisualStudio.SolutionName;
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
            Bind<IProjectBuilderFactory>().To<VsProjectBuilderFactory>();

            Bind<ICleanExtension>().To<CsprojCleaner>();

            Bind<IProjectGuidManagement>().To<DefaultProjectGuidManagement>().InSingletonScope();
            Bind<IMSBuild>().To<MSBuild>();
            
            Bind<IMSBuildRunnerFactory>().ToFactory();
            Bind<ISlnBuilderFactory>().ToFactory();

            Bind<IReferenceBuilder>().To<GacReferenceBuilder>().Named("gac");
            Bind<IInSolutionReferenceBuilderFactory>().ToFactory();

            Bind<ICsprojSection>().To<PropertiesSection>();
            Bind<ICsprojSection>().To<ReferencesSection>();
            Bind<ICsprojSection>().To<SourceItemsSection>();
            Bind<ICsprojSection>().To<VersionSection>();
            
            Bind<IYamlProjectParametersLoader>().To<CsharpParametersLoader>();

            Bind<ISuiteContentsAnalyzer>().To<DefaultSuiteContentsAnalyzer>();
            Bind<ISlnNameGenerator>().To<HashBasedSlnNameGenerator>().WhenInjectedInto<ReadableSlnNameGenerator>();
            Bind<ISlnNameGenerator>().To<ReadableSlnNameGenerator>();
        }
    }
}
