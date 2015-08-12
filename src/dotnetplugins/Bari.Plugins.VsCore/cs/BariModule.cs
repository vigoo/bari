using Bari.Core.Build;
using Bari.Core.Build.BuilderStore;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Commands.Clean;
using Bari.Core.Model.Discovery;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using Bari.Plugins.VsCore.Build;
using Bari.Plugins.VsCore.Build.BuilderStore;
using Bari.Plugins.VsCore.Model;
using Bari.Plugins.VsCore.Model.Discovery;
using Bari.Plugins.VsCore.Model.Loader;
using Bari.Plugins.VsCore.Tools.Versions;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.SolutionItems;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using System.Collections.Generic;

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
            Bind<ISlnBuilderFactory>().ToFactory();

            Bind<ISuiteExplorer>().To<ProjectPathExplorer>();

            var parameters = Kernel.Get<IParameters>();

            if (parameters.UseMono)
                Bind<IMSBuildFactory>().To<XBuildFactory>().InSingletonScope();
            else
                Bind<IMSBuildFactory>().To<MSBuildFactory>().InSingletonScope();
            Bind<IMSBuildRunnerFactory>().ToFactory();

            Bind<IYamlProjectParametersLoader>().To<MSBuildParametersLoader>();

            Bind<IReferenceBuilder>().To<GacReferenceBuilder>().Named("gac");
            Bind<IInSolutionReferenceBuilderFactory>().ToFactory();

            var buildContextFactory = Kernel.Get<IBuildContextFactory>();
            Rebind<IBuildContextFactory>().ToConstant(
                new OptimizingBuildContextFactory(
                    buildContextFactory, 
                    Kernel.Get<ICoreBuilderFactory>(),
                    Kernel.Get<IInSolutionReferenceBuilderFactory>(),
                    Kernel.GetAll<IProjectBuilderFactory>()
                )
            ).InSingletonScope();

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
            protocolRegistry.RegisterEnum(i => (MSBuildVersion) i);

            // Stored sln builder
            var store = Kernel.Get<IBuilderStore>();

            var slnBuilderFactory = Kernel.Get<ISlnBuilderFactory>();
            Rebind<ISlnBuilderFactory>().ToConstant(
                new StoredSlnBuilderFactory(slnBuilderFactory, store));
            var msbuildRunnerFactory = Kernel.Get<IMSBuildRunnerFactory>();
            Rebind<IMSBuildRunnerFactory>().ToConstant(
                new StoredMSBuildRunnerFactory(msbuildRunnerFactory, store));
        }
    }
}
