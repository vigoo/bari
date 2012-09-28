using System;
using System.IO;
using System.Reflection;
using Bari.Console.UI;
using Bari.Core;
using Bari.Core.Generic;
using Bari.Core.Process;
using Bari.Core.UI;
using Ninject;

namespace Bari.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            Kernel.RegisterCoreBindings();
            var root = Kernel.Root;

            string fixPluginPattern = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                                   "Bari.Plugins.*.dll");
            root.Load(fixPluginPattern);
            
            root.Bind<IUserOutput>().To<ConsoleUserInterface>().InSingletonScope();
            root.Bind<IParameters>().ToConstant(new ConsoleParameters(args)).InSingletonScope();

            var suiteRoot = new LocalFileSystemDirectory(Environment.CurrentDirectory);
            root.Bind<IFileSystemDirectory>()
                .ToConstant(suiteRoot)
                .WhenTargetHas<SuiteRootAttribute>();
            root.Bind<IFileSystemDirectory>()
                .ToConstant(suiteRoot.GetChildDirectory("target", createIfMissing: true))
                .WhenTargetHas<TargetRootAttribute>();

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
    }
}
