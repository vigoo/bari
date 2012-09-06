using Bari.Core.Model;
using Bari.Core.Model.Loader;
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
            root.Bind<IModelLoader>().To<LocalYamlModelLoader>().InSingletonScope();
            root.Bind<IModelLoader>().To<InMemoryYamlModelLoader>().InSingletonScope();
            root.Bind<ISuiteLoader>().To<DefaultSuiteLoader>().InSingletonScope();
        }
    }
}