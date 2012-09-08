using System;
using Bari.Console.UI;
using Bari.Core;
using Bari.Core.Process;
using Bari.Core.UI;
using Ninject;

namespace Bari.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Kernel.RegisterCoreBindings();
            var root = Kernel.Root;
            
            root.Bind<IUserOutput>().To<ConsoleUserInterface>().InSingletonScope();
            root.Bind<IParameters>().ToConstant(new ConsoleParameters(args)).InSingletonScope();

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
