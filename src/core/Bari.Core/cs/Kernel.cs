using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Cache;
using Bari.Core.Build.Dependencies;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Commands;
using Bari.Core.Commands.Clean;
using Bari.Core.Commands.Helper;
using Bari.Core.Commands.Test;
using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Core.Model.Loader;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Parameters;
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

            kernel.Bind<ISuiteFactory>().To<DefaultSuiteFactory>().InSingletonScope();
            kernel.Bind<IModelLoader>().To<LocalYamlModelLoader>().InSingletonScope();
            kernel.Bind<IModelLoader>().To<InMemoryYamlModelLoader>().InSingletonScope();
            kernel.Bind<ISuiteLoader>().To<DefaultSuiteLoader>().InSingletonScope();

            // Command factory and enumerator
            kernel.Bind<ICommandFactory>().ToFactory(() => new CommandInstanceProvider());
            kernel.Bind<ICommandEnumerator>().ToConstant(new CommandEnumerator(kernel));

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
            kernel.Bind<IBuildContextFactory>().ToFactory();

            // Builders 
            kernel.Bind<IReferenceBuilder>().To<ModuleReferenceBuilder>().Named("module");
            kernel.Bind<IReferenceBuilder>().To<SuiteReferenceBuilder>().Named("suite");

            // Builder factories
            kernel.Bind<ICachedBuilderFactory>().ToFactory();
            kernel.Bind<IReferenceBuilderFactory>().ToFactory(() => new ReferenceBuilderInstanceProvider());

            kernel.Bind<ISourceSetDependencyFactory>().ToFactory();
            kernel.Bind<ISourceSetFingerprintFactory>().ToFactory();

            // Default command target parser
            kernel.Bind<ICommandTargetParser>().To<CommandTargetParser>();
        }

        class CommandInstanceProvider : StandardInstanceProvider
        {
            protected override string GetName(System.Reflection.MethodInfo methodInfo, object[] arguments)
            {
                return (string)arguments[0];
            }

            protected override ConstructorArgument[] GetConstructorArguments(System.Reflection.MethodInfo methodInfo, object[] arguments)
            {
                return base.GetConstructorArguments(methodInfo, arguments).Skip(1).ToArray();
            }

            public override object GetInstance(Ninject.Extensions.Factory.Factory.IInstanceResolver instanceResolver, System.Reflection.MethodInfo methodInfo, object[] arguments)
            {
                try
                {
                    return base.GetInstance(instanceResolver, methodInfo, arguments);
                }
                catch (ActivationException)
                {
                    return null;
                }
            }
        }

        class ReferenceBuilderInstanceProvider : StandardInstanceProvider
        {
            protected override string GetName(System.Reflection.MethodInfo methodInfo, object[] arguments)
            {
                var reference = (Reference) arguments[0];
                return reference.Uri.Scheme;
            }

            protected override ConstructorArgument[] GetConstructorArguments(System.Reflection.MethodInfo methodInfo, object[] arguments)
            {
                return base.GetConstructorArguments(methodInfo, arguments).Skip(1).ToArray();
            }

            public override object GetInstance(Ninject.Extensions.Factory.Factory.IInstanceResolver instanceResolver, System.Reflection.MethodInfo methodInfo, object[] arguments)
            {
                try
                {
                    var builder = (IReferenceBuilder)base.GetInstance(instanceResolver, methodInfo, arguments);
                    builder.Reference = (Reference)arguments[0];
                    return builder;
                }
                catch (ActivationException)
                {
                    return null;
                }
            }
        }

        class CommandEnumerator : ICommandEnumerator
        {
            private readonly IResolutionRoot localRoot;

            public CommandEnumerator(IResolutionRoot localRoot)
            {
                this.localRoot = localRoot;
            }

            public IEnumerable<string> AvailableCommands
            {
                get { return localRoot.GetAll<ICommand>().Select(cmd => cmd.Name).OrderBy(n => n); }
            }
        }
    }
}