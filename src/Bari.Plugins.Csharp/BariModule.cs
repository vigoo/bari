using Bari.Core.Build;
using Bari.Core.Commands;
using Bari.Plugins.Csharp.Build;
using Bari.Plugins.Csharp.Commands;
using Bari.Plugins.Csharp.Tools;
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

            Bind<ICommand>().To<VisualStudioCommand>().Named("vs");
            Bind<IProjectBuilderFactory>().To<VsProjectBuilderFactory>();

            Bind<IProjectGuidManagement>().To<DefaultProjectGuidManagement>().InSingletonScope();
            Bind<IMSBuild>().To<MSBuild>();

            Bind<IReferenceBuilder>().To<GacReferenceBuilder>().Named("gac");
        }
    }
}
