using System.Diagnostics.Contracts;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Commands;
using Bari.Core.Commands.Clean;
using Bari.Core.Commands.Helper;
using Bari.Core.Commands.Test;
using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Core.Model.Loader;
using Ninject;
using Ninject.Syntax;

namespace Bari.Core
{
    /// <summary>
    /// Hosts the root Ninject kernel
    /// </summary>
    public static class Kernel
    {
        private static readonly IKernel root = new StandardKernel();

        /// <summary>
        /// Returns the root Ninject kernel, to be used for creating top level objects
        /// </summary>
        public static IKernel Root
        {
            get
            {
                Contract.Ensures(Contract.Result<IKernel>() != null);
                
                return root;
            }
        }

        /// <summary>
        /// Registers default bindings offered by the core bari module
        /// </summary>
        public static void RegisterCoreBindings()
        {
            RegisterCoreBindings(root);
        }

        /// <summary>
        /// Registers default bindings offered by the core bari module to the given ninject kernel
        /// </summary>
        /// <param name="kernel">The kernel to be used for registration</param>
        public static void RegisterCoreBindings(IKernel kernel)
        {
            Contract.Requires(kernel != null);

            kernel.Bind<IBindingRoot>().ToConstant(kernel);

            kernel.Bind<IModelLoader>().To<LocalYamlModelLoader>().InSingletonScope();
            kernel.Bind<IModelLoader>().To<InMemoryYamlModelLoader>().InSingletonScope();
            kernel.Bind<ISuiteLoader>().To<DefaultSuiteLoader>().InSingletonScope();

            // Built-in commands
            kernel.Bind<ICommand>().To<HelpCommand>().Named("help");
            kernel.Bind<ICommand>().To<InfoCommand>().Named("info");
            kernel.Bind<ICommand>().To<CleanCommand>().Named("clean");
            kernel.Bind<ICommand>().To<BuildCommand>().Named("build");
            kernel.Bind<ICommand>().To<TestCommand>().Named("test");

            // Built-in suite explorers
            kernel.Bind<ISuiteExplorer>().To<ModuleProjectDiscovery>();

            // Default serializer
            kernel.Bind<IProtocolSerializer>().To<BinarySerializer>();

            // Default build context
            kernel.Bind<IBuildContext>().To<BuildContext>();

            // Builders 
            kernel.Bind<IReferenceBuilder>().To<ModuleReferenceBuilder>().Named("module");
            kernel.Bind<IReferenceBuilder>().To<SuiteReferenceBuilder>().Named("suite");

            // Default command target parser
            kernel.Bind<ICommandTargetParser>().To<CommandTargetParser>();
        }
    }
}