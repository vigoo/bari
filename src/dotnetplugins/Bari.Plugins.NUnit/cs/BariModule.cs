using Bari.Core.Commands.Test;
using Bari.Plugins.NUnit.Commands.Test;
using Bari.Plugins.NUnit.Tools;
using Ninject.Modules;

namespace Bari.Plugins.NUnit
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
            log.Info("NUnit plugin loaded");

            Bind<INUnit>().To<Tools.NUnit>();
            Bind<ITestRunner>().To<NUnitTestRunner>();
        }
    }
}
