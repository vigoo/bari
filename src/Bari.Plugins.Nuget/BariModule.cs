using Bari.Core.Build;
using Bari.Plugins.Nuget.Build;
using Bari.Plugins.Nuget.Tools;
using Ninject.Modules;

namespace Bari.Plugins.Nuget
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
            log.Info("Nuget plugin loaded");

            Bind<IReferenceBuilder>().To<NugetReferenceBuilder>().Named("nuget");
            Bind<INuGet>().To<NuGet>();
        }
    }
}
