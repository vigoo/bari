using Bari.Core.Build;
using Bari.Core.Model.Loader;
using Bari.Plugins.FSRepository.Build;
using Bari.Plugins.FSRepository.Build.Dependencies;
using Bari.Plugins.FSRepository.Model;
using Bari.Plugins.FSRepository.Model.Loader;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Bari.Plugins.FSRepository
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
            log.Info("FSRepository plugin loaded");

            Bind<IFileSystemRepositoryAccess>().To<LocalFileSystemRepositoryAccess>();
            Bind<IReferenceBuilder>().To<FSRepositoryReferenceBuilder>().Named("fsrepo");
            Bind<IYamlProjectParametersLoader>().To<YamlRepositoryPatternCollectionLoader>();
            Bind<IFSRepositoryFingerprintFactory>().ToFactory();
        }
    }
}
