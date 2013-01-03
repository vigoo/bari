using Bari.Core.Model.Loader;
using Bari.Plugins.CodeContracts.Model.Loader;
using Bari.Plugins.CodeContracts.VisualStudio.CsprojSections;
using Bari.Plugins.Csharp.VisualStudio.CsprojSections;
using Ninject.Modules;

namespace Bari.Plugins.CodeContracts
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
            log.Info("CodeContracts plugin loaded");

            Bind<ICsprojSection>().To<CodeContractsSection>();
            Bind<IYamlProjectParametersLoader>().To<YamlContractsParametersLoader>();
        }
    }
}
