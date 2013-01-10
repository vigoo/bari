using NUnit.Framework;

namespace Bari.Core.Test
{
    [SetUpFixture]
    public class TestSetup
    {
        [SetUp]
        public void Setup()
        {
            Kernel.RegisterCoreBindings();
            Kernel.Root.Load(new[] {typeof (Ninject.Extensions.Factory.FuncModule).Assembly});
        }

    }
}