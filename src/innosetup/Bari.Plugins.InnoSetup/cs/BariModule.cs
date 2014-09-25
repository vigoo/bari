using Bari.Core.Commands.Pack;
using Bari.Core.Model.Discovery;
using Bari.Core.Model.Loader;
using Bari.Plugins.InnoSetup.Model.Discovery;
using Bari.Plugins.InnoSetup.Packager;
using Bari.Plugins.InnoSetup.Packager.Loader;
using Bari.Plugins.InnoSetup.Tools;
using Ninject.Modules;

namespace Bari.Plugins.InnoSetup
{
    /// <summary>
    /// The module definition of this bari plugin
    /// </summary>
    public class BariModule : NinjectModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BariModule));

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            log.Info("InnoSetup plugin loaded");

            Bind<IInnoSetupCompiler>().To<InnoSetupCompiler>();
            Bind<IProductPackager>().To<InnoSetupProductPackager>().Named("innosetup");
            Bind<IYamlProjectParametersLoader>().To<InnoSetupPackagerParametersLoader>();
            Bind<ISuiteExplorer>().To<InnoSetupScriptDiscovery>();
        }
    }
}
