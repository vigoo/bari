using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Moq;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Discovery
{
    [TestFixture]
    class ExplorerRunnerTest
    {
        [Test]
        public void CanRunWithZeroExplorers()
        {
            var kernel = new StandardKernel();
            var explorer = kernel.Get<ExplorerRunner>();
            var suite = kernel.Get<Suite>();

            explorer.RunAll(suite);
        }

        [Test]
        public void AllRegisteredExplorersAreExecuted()
        {
            var kernel = new StandardKernel();            
            var suite = kernel.Get<Suite>();

            var exp1 = new Mock<ISuiteExplorer>();
            var exp2 = new Mock<ISuiteExplorer>();            

            kernel.Bind<ISuiteExplorer>().ToConstant(exp1.Object);
            kernel.Bind<ISuiteExplorer>().ToConstant(exp2.Object);

            var explorer = kernel.Get<ExplorerRunner>();
            explorer.RunAll(suite);

            exp1.Verify(x => x.ExtendWithDiscoveries(suite), Times.Once());
            exp2.Verify(x => x.ExtendWithDiscoveries(suite), Times.Once());
        }
    }
}
