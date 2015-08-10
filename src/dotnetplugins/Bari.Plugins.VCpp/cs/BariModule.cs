using Bari.Core.Build.BuilderStore;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Commands.Clean;
using Bari.Core.Generic;
using Bari.Core.Model.Loader;
using Bari.Plugins.VCpp.Build;
using Bari.Plugins.VCpp.Build.BuilderStore;
using Bari.Plugins.VCpp.Commands.Clean;
using Bari.Plugins.VCpp.Model;
using Bari.Plugins.VCpp.Model.Loader;
using Bari.Plugins.VCpp.VisualStudio;
using Bari.Plugins.VCpp.VisualStudio.VcxprojSections;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Bari.Plugins.VCpp
{
    /// <summary>
    /// The module definition of this bari plugin
    /// </summary>
    [DependsOn(typeof(VsCore.BariModule))]
    public class BariModule : NinjectModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BariModule));

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            log.Info("Visual C++ plugin loaded");

            Bind<ISlnProject>().To<CppSlnProject>();
            Bind<IVcxprojBuilderFactory>().ToFactory();

            Bind<ICleanExtension>().To<VcxprojCleaner>();

            Bind<IYamlProjectParametersLoader>().To<VCppCompilerParametersLoader>();
            Bind<IYamlProjectParametersLoader>().To<VCppLinkerParametersLoader>();
            Bind<IYamlProjectParametersLoader>().To<VCppATLParametersLoader>();
            Bind<IYamlProjectParametersLoader>().To<VCppMIDLParametersLoader>();
            Bind<IYamlProjectParametersLoader>().To<VCppManifestParametersLoader>();
            Bind<IYamlProjectParametersLoader>().To<VCppCLIParametersLoader>();
            Bind<IYamlProjectParametersLoader>().To<VCppToolchainParametersLoader>();

            Bind<IMSBuildProjectSection>().To<SourceItemsSection>().WhenInjectedInto<VcxprojGenerator>();
            Bind<IMSBuildProjectSection>().To<PropertiesSection>().WhenInjectedInto<VcxprojGenerator>();
            Bind<IMSBuildProjectSection>().To<ReferencesSection>().WhenInjectedInto<VcxprojGenerator>().WithConstructorArgument("sourceSetName", "cpp");
            Bind<IMSBuildProjectSection>().To<StaticLibraryReferencesSection>().WhenInjectedInto<VcxprojGenerator>();

            var oldPlatformManagement = Kernel.Get<IProjectPlatformManagement>();
            Rebind<IProjectPlatformManagement>().ToConstant(new CppProjectPlatformManagement(oldPlatformManagement)).InSingletonScope();

            var protocolRegistry = Kernel.Get<IDependencyFingerprintProtocolRegistry>();
            protocolRegistry.RegisterEnum(i => (AssemblerOutputType)i);
            protocolRegistry.RegisterEnum(i => (CharType)i);
            protocolRegistry.RegisterEnum(i => (CLanguage)i);
            protocolRegistry.RegisterEnum(i => (CLRImageType)i);
            protocolRegistry.RegisterEnum(i => (CLRSupportLastError)i);
            protocolRegistry.RegisterEnum(i => (CppCliMode)i);
            protocolRegistry.RegisterEnum(i => (CppWarningLevel)i);
            protocolRegistry.RegisterEnum(i => (DebugInformationFormat)i);
            protocolRegistry.RegisterEnum(i => (EnhancedInstructionSet)i);
            protocolRegistry.RegisterEnum(i => (ExceptionHandlingType)i);
            protocolRegistry.RegisterEnum(i => (FloatingPointModel)i);
            protocolRegistry.RegisterEnum(i => (InlineExpansion)i);
            protocolRegistry.RegisterEnum(i => (LinkerDriverOption)i);
            protocolRegistry.RegisterEnum(i => (LinkerForceOption)i);
            protocolRegistry.RegisterEnum(i => (LinkerHotPatchingOption)i);
            protocolRegistry.RegisterEnum(i => (LinkerSubSystemOption)i);
            protocolRegistry.RegisterEnum(i => (LinkerTargetMachine)i);
            protocolRegistry.RegisterEnum(i => (ManagedCppType)i);
            protocolRegistry.RegisterEnum(i => (MidlErrorChecks)i);
            protocolRegistry.RegisterEnum(i => (MidlTargetEnvironment)i);
            protocolRegistry.RegisterEnum(i => (OptimizationFavor)i);
            protocolRegistry.RegisterEnum(i => (OptimizationLevel)i);
            protocolRegistry.RegisterEnum(i => (PlatformToolSet)i);
            protocolRegistry.RegisterEnum(i => (RuntimeCheckType)i);
            protocolRegistry.RegisterEnum(i => (RuntimeLibraryType)i);
            protocolRegistry.RegisterEnum(i => (UACExecutionLevel)i);
            protocolRegistry.RegisterEnum(i => (UseOfATL)i);

            var store = Kernel.Get<IBuilderStore>();
            var vxprojBuilderFactory = Kernel.Get<IVcxprojBuilderFactory>();
            Rebind<IVcxprojBuilderFactory>().ToConstant(
                new StoredVcxprojBuilderFactory(vxprojBuilderFactory, store));
        }
    }
}
