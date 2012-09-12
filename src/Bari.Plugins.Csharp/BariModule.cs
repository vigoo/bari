using Bari.Core.Commands;
using Bari.Plugins.Csharp.Commands;
using Bari.Plugins.Csharp.VisualStudio;
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

            Bind<ICommand>().To<VisualStudioCommand>().Named("visualstudio");

            Bind<IProjectGuidManagement>().To<DefaultProjectGuidManagement>().InSingletonScope();
        }
    }
}
