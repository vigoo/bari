using Bari.Core.Commands.Clean;
using Bari.Core.Model.Loader;
using Bari.Plugins.VCpp.Build;
using Bari.Plugins.VCpp.Commands.Clean;
using Bari.Plugins.VCpp.Model.Loader;
using Bari.Plugins.VCpp.VisualStudio;
using Bari.Plugins.VCpp.VisualStudio.VcxprojSections;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Bari.Plugins.VCpp
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
            log.Info("Visual C++ plugin loaded");

            Bind<ISlnProject>().To<CppSlnProject>();
            Bind<IVcxprojBuilderFactory>().ToFactory();

            Bind<ICleanExtension>().To<VcxprojCleaner>();

            Bind<IYamlProjectParametersLoader>().To<VCppCompilerParametersLoader>();
            Bind<IYamlProjectParametersLoader>().To<VCppLinkerParametersLoader>();

            Bind<IMSBuildProjectSection>().To<SourceItemsSection>().WhenInjectedInto<VcxprojGenerator>(); 
        }
    }
}
