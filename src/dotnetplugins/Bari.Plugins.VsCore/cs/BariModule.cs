using Bari.Core.Build;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Commands.Clean;
using Bari.Core.Model.Discovery;
using Bari.Core.UI;
using Bari.Plugins.VsCore.Build;
using Bari.Plugins.VsCore.Model;
using Bari.Plugins.VsCore.Model.Discovery;
using Bari.Plugins.VsCore.Tools;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.SolutionItems;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Bari.Plugins.VsCore
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
            log.Info("VsCore plugin loaded");

            Bind<IProjectBuilderFactory>().To<VsProjectBuilderFactory>();
            Bind<IProjectPlatformManagement>().To<DefaultProjectPlatformManagement>().InSingletonScope();
            Bind<IProjectPathManagement>().To<DefaultProjectPathManagement>().InSingletonScope();

            Bind<ISolutionItemProvider>().To<SuiteDefinitionSolutionItemProvider>();

            Bind<ISuiteExplorer>().To<ProjectPathExplorer>();

            var parameters = Kernel.Get<IParameters>();

            if (parameters.UseMono)
                Bind<IMSBuild>().To<XBuild>();
            else
                Bind<IMSBuild>().To<MSBuild>();
            Bind<IMSBuildRunnerFactory>().ToFactory();

            // Extending soft-clean behavior
            var predicates = Kernel.Get<ISoftCleanPredicates>();
            predicates.Add(path => path.EndsWith(".vshost.exe"));
            predicates.Add(path => path.EndsWith(".suo"));
            predicates.Add(path => path == "guids");

            var protocolRegistry = Kernel.Get<IDependencyFingerprintProtocolRegistry>();
            protocolRegistry.RegisterEnum(i => (CLRPlatform)i);
            protocolRegistry.RegisterEnum(i => (DebugLevel)i);
            protocolRegistry.RegisterEnum(i => (FrameworkProfile)i);
            protocolRegistry.RegisterEnum(i => (FrameworkVersion)i);
            protocolRegistry.RegisterEnum(i => (WarningLevel)i);
        }
    }
}
