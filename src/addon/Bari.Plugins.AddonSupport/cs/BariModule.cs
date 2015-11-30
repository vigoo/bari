using Bari.Core.Commands;
using Bari.Core.Model.Loader;
using Bari.Plugins.AddonSupport.Commands;
using Bari.Plugins.AddonSupport.Model.Loader;
using Bari.Plugins.AddonSupport.VisualStudio.SolutionItems;
using Bari.Plugins.VsCore.VisualStudio.SolutionItems;
using Ninject.Modules;

namespace Bari.Plugins.AddonSupport
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
            log.Info("AddonSupport plugin loaded");

            Kernel.Bind<ISolutionItemProvider>().To<AddonSupportSolutionItemProvider>();
            Bind<IYamlProjectParametersLoader>().To<StartupModuleParametersLoader>();
            
            Bind<ICommand>().To<AddonInfoCommand>().Named("addon-info");
            Bind<ICommandPrerequisites>().To<DefaultCommandPrerequisites>().Named("addon-info");
        }
    }
}
