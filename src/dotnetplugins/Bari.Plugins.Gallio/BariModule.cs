using Bari.Core.Commands.Test;
using Bari.Plugins.Gallio.Commands.Test;
using Bari.Plugins.Gallio.Tools;
using Ninject.Modules;

namespace Bari.Plugins.Gallio
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
            log.Info("Gallio plugin loaded");
            
            Bind<IGallio>().To<Tools.Gallio>();
            Bind<ITestRunner>().To<GallioTestRunner>();
        }
    }
}
