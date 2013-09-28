using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test
{
    [SetUpFixture]
    public class TestSetup
    {
        [SetUp]
        public void Setup()
        {
            Kernel.RegisterCoreBindings();
            EnsureFactoryExtensionLoaded(Kernel.Root);
        }

        public static void EnsureFactoryExtensionLoaded(IKernel kernel)
        {
            if (!kernel.HasModule(typeof (Ninject.Extensions.Factory.FuncModule).FullName))
                kernel.Load(new[] {typeof (Ninject.Extensions.Factory.FuncModule).Assembly});
        }
    }
}