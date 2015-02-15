using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Model.Loader;
using Bari.Plugins.CodeContracts.Model;
using Bari.Plugins.CodeContracts.Model.Loader;
using Bari.Plugins.CodeContracts.VisualStudio.CsprojSections;
using Bari.Plugins.Csharp.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;
using Ninject;
using Ninject.Modules;

namespace Bari.Plugins.CodeContracts
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
            log.Info("CodeContracts plugin loaded");

            Bind<IMSBuildProjectSection>().To<CodeContractsSection>().WhenInjectedInto<CsprojGenerator>();
            Bind<IYamlProjectParametersLoader>().To<YamlContractsParametersLoader>();

            var protocolRegistry = Kernel.Get<IDependencyFingerprintProtocolRegistry>();
            protocolRegistry.RegisterEnum(i => (ContractAssemblyMode) i);
            protocolRegistry.RegisterEnum(i => (ContractCheckingLevel)i);
            protocolRegistry.RegisterEnum(i => (ContractReferenceMode)i);
        }
    }
}
