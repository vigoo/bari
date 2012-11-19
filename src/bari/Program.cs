using System;
using System.IO;
using System.Reflection;
using Bari.Console.UI;
using Bari.Core;
using Bari.Core.Build.Cache;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Commands.Clean;
using Bari.Core.Generic;
using Bari.Core.Process;
using Bari.Core.UI;
using Ninject;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Bari.Console
{
    static class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (Program));

        static void Main(string[] args)
        {
            var consoleParams = new ConsoleParameters(args);
            if (consoleParams.VerboseOutput)
                EnableConsoleDebugLog();

            var root = Kernel.Root;

            // Binding UI interfaces
            root.Bind<IUserOutput>().To<ConsoleUserInterface>().InSingletonScope();
            root.Bind<IParameters>().ToConstant(consoleParams).InSingletonScope();

            // Binding special directories
            var suiteRoot = new LocalFileSystemDirectory(Environment.CurrentDirectory);
            root.Bind<IFileSystemDirectory>()
                .ToConstant(suiteRoot)
                .WhenTargetHas<SuiteRootAttribute>();
            root.Bind<IFileSystemDirectory>()
                .ToConstant(suiteRoot.GetChildDirectory("target", createIfMissing: true))
                .WhenTargetHas<TargetRootAttribute>();

            // Binding core services
            Kernel.RegisterCoreBindings();

            // Binding default cache
            var cacheDir = suiteRoot.GetChildDirectory("cache", createIfMissing: true);
            var buildCache = new FileBuildCache(
                cacheDir,
                root.Get<IProtocolSerializer>());
            root.Bind<IBuildCache>().ToConstant(buildCache);
            root.Bind<ICleanExtension>().ToConstant(new CacheCleaner(cacheDir));

            // Loading fix plugins
            string fixPluginPattern = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                                   "Bari.Plugins.*.dll");
            root.Load(fixPluginPattern);           

            var process = root.Get<MainProcess>();
            try
            {
                process.Run();
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(ex.ToString());
                System.Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private static void EnableConsoleDebugLog()
        {
            var consoleAppender = new log4net.Appender.ColoredConsoleAppender
                {
                    Layout = new SimpleLayout(),
                    Threshold = Level.All
                };
            consoleAppender.ActivateOptions();

            var repo = (Hierarchy)log4net.LogManager.GetRepository();
            var root = repo.Root;
            root.AddAppender(consoleAppender);

            repo.Configured = true;

            log.Info("Verbose logging to console enabled.");
        }
    }
}
