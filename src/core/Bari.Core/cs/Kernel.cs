using System;
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
using Bari.Core.Commands.Pack;
using Bari.Core.Commands.Test;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Core.Model.Loader;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Parameters;
using Ninject.Syntax;
using Bari.Core.Build.Statistics;

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

        public static void RegisterCommand<TCommand, TCommandPrerequisites>(IKernel kernel, string name)
            where TCommand : ICommand
            where TCommandPrerequisites : ICommandPrerequisites
        {
            kernel.Bind<ICommand>().To<TCommand>().Named(name);
            kernel.Bind<ICommandPrerequisites>().To<TCommandPrerequisites>().Named(name);
        }

        /// <summary>
        /// Registers default bindings offered by the core bari module to the given ninject kernel
        /// </summary>
        /// <param name="kernel">The kernel to be used for registration</param>
        public static void RegisterCoreBindings(IKernel kernel)
        {
            Contract.Requires(kernel != null);

            kernel.Bind<IBindingRoot>().ToConstant(kernel);

            kernel.Bind<IPluginLoader>().To<DefaultPluginLoader>();

            kernel.Bind<ISuiteFactory>().To<DefaultSuiteFactory>().InSingletonScope();
            kernel.Bind<IModelLoader>().To<LocalYamlModelLoader>().InSingletonScope();
            kernel.Bind<IModelLoader>().To<InMemoryYamlModelLoader>().InSingletonScope();
            kernel.Bind<ISuiteLoader>().To<DefaultSuiteLoader>().InSingletonScope();

            kernel.Bind<IEnvironmentVariableContext>().To<DefaultEnvironmentVariableContext>().InSingletonScope();

            kernel.Bind<IYamlProjectParametersLoader>().To<YamlReferenceAliasesLoader>();
            kernel.Bind<IYamlProjectParametersLoader>().To<YamlTestsLoader>();

            // Command factory and enumerator
            RegisterCommandFactory(kernel);

            // Builder enumerator
            kernel.Bind<IBuilderEnumerator>().ToConstant(new BuilderEnumerator());

            // Built-in commands
            RegisterCommand<HelpCommand, HelpCommandPrerequisites>(kernel, "help");
            RegisterCommand<InfoCommand, DefaultCommandPrerequisites>(kernel, "info");
            RegisterCommand<CleanCommand, DefaultCommandPrerequisites>(kernel, "clean");
            RegisterCommand<BuildCommand, DefaultCommandPrerequisites>(kernel, "build");
            RegisterCommand<TestCommand, DefaultCommandPrerequisites>(kernel, "test");
            RegisterCommand<RebuildCommand, DefaultCommandPrerequisites>(kernel, "rebuild");
            RegisterCommand<PackCommand, DefaultCommandPrerequisites>(kernel, "pack");
            RegisterCommand<PublishCommand, DefaultCommandPrerequisites>(kernel, "publish");
            RegisterCommand<SelfUpdateCommand, SelfUpdateCommandPrerequisites>(kernel, "selfupdate");

            // Built-in suite explorers
            kernel.Bind<ISuiteExplorer>().To<ModuleProjectDiscovery>();

            // Default serializer
            kernel.Bind<IProtocolSerializer>().To<BinarySerializer>();

			// Builder statistics
			kernel.Bind<IBuilderStatistics>().To<DefaultBuilderStatistics>();
			kernel.Bind<IMonitoredBuilderFactory>().ToFactory();

            // Default build context
            kernel.Bind<IBuildContext>().To<BuildContext>();
            kernel.Bind<IBuildContextFactory>().ToFactory();

            // Builders 
            kernel.Bind<IReferenceBuilder>().To<ModuleReferenceBuilder>().Named("module");
            kernel.Bind<IReferenceBuilder>().To<SuiteReferenceBuilder>().Named("suite");
            kernel.Bind<IReferenceBuilder>().To<AliasReferenceBuilder>().Named("alias");
            kernel.Bind<IReferenceBuilder>().To<FileReferenceBuilder>().Named("file");

            // Builder factories
            kernel.Bind<ICachedBuilderFactory>().ToFactory();
            kernel.Bind<IReferenceBuilderFactory>().ToFactory(() => new ReferenceBuilderInstanceProvider());

            kernel.Bind<ISourceSetDependencyFactory>().ToFactory();
            kernel.Bind<ISourceSetFingerprintFactory>().ToFactory();

            kernel.Bind<IProjectBuilderFactory>().To<ContentProjectBuilderFactory>();

            // Packager factory
            kernel.Bind<IProductPackagerFactory>().ToFactory(() => new PackagerFactoryInstanceProvider());

            // Default command target parser
            kernel.Bind<ICommandTargetParser>().To<CommandTargetParser>();

            // Clean helpers
            kernel.Bind<ISoftCleanPredicates>().To<SoftCleanPredicates>().InSingletonScope();
        }

        public static void RegisterCommandFactory(IKernel kernel)
        {
            kernel.Bind<ICommandFactory>().ToFactory(() => new CommandInstanceProvider());
            kernel.Bind<ICommandEnumerator>().ToConstant(new CommandEnumerator(kernel));            
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

        class PackagerFactoryInstanceProvider : StandardInstanceProvider
        {
            protected override string GetName(System.Reflection.MethodInfo methodInfo, object[] arguments)
            {
                var type = (PackagerId)arguments[0];
                return type.AsString;
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

            public bool NeedsExplicitTargetGoal(string commandName)
            {
                var cmd = localRoot.GetAll<ICommand>().FirstOrDefault(c => c.Name == commandName);
                if (cmd != null)
                {
                    return cmd.NeedsExplicitTargetGoal;
                }    
                    
                return true;
            }
        }

        class BuilderEnumerator : IBuilderEnumerator
        {
            private bool IsPersistentReferenceBuilder(Type referenceBuilderType)
            {
                return
                    referenceBuilderType
                        .GetCustomAttributes(typeof(PersistentReferenceAttribute), true)
                        .OfType<PersistentReferenceAttribute>()
                        .Any();
            }

            public IEnumerable<Type> GetAllPersistentBuilders()
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof (IReferenceBuilder).IsAssignableFrom(p));
                return types.Where(IsPersistentReferenceBuilder);
            }
        }
    }
}