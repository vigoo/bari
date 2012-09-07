using Bari.Core.Commands;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using Ninject;

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
            get { return root; }
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
            kernel.Bind<IModelLoader>().To<LocalYamlModelLoader>().InSingletonScope();
            kernel.Bind<IModelLoader>().To<InMemoryYamlModelLoader>().InSingletonScope();
            kernel.Bind<ISuiteLoader>().To<DefaultSuiteLoader>().InSingletonScope();

            // Built-in commands
            kernel.Bind<ICommand>().To<HelpCommand>().Named("help");
        }

        /// <summary>
        /// Registers UI interfaces to the console implementation
        /// </summary>
        public static void RegisterConsoleUI()
        {
            root.Bind<IUserOutput>().To<ConsoleUserInterface>().InSingletonScope();
        }
    }
}