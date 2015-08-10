using Bari.Core.Build;
using Bari.Core.Build.BuilderStore;
using Bari.Core.Model.Discovery;
using Bari.Plugins.PythonScripts.Build;
using Bari.Plugins.PythonScripts.Build.BuilderStore;
using Bari.Plugins.PythonScripts.Model.Discovery;
using Bari.Plugins.PythonScripts.Scripting;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Bari.Plugins.PythonScripts
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
            log.Info("PythonScripts plugin loaded");

            Bind<ISuiteExplorer>().To<PythonBuildScriptDiscovery>();
            Bind<ISuiteExplorer>().To<PythonPostProcessorScriptDiscovery>();
            Bind<IProjectBuilderFactory>().To<PythonScriptedProjectBuilderFactory>();
            Bind<IPostProcessorFactory>().To<PythonScriptedPostProcessorFactory>();
            Bind<IProjectBuildScriptRunner>().To<ProjectBuildScriptRunner>();
            Bind<IPostProcessorScriptRunner>().To<PostProcessorScriptRunner>();

            Bind<IPythonScriptedBuilderFactory>().ToFactory();

            var store = Kernel.Get<IBuilderStore>();
            var pythonScriptedBuilderFactory = Kernel.Get<IPythonScriptedBuilderFactory>();
            Rebind<IPythonScriptedBuilderFactory>()
                .ToConstant(new StoredPythonScriptedBuilderFactory(pythonScriptedBuilderFactory, store));
        }
    }
}
