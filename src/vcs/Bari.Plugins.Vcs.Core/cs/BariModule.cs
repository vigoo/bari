using Ninject.Modules;

namespace Bari.Plugins.Vcs.Core
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
            log.Info("Vcs core plugin loaded");
        }
    }
}
