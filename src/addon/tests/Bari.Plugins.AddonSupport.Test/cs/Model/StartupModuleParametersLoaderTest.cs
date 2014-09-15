using Bari.Core;
using Bari.Core.Generic;
using Bari.Core.Model.Loader;
using Bari.Core.Test;
using Bari.Core.Test.Helper;
using Bari.Core.UI;
using Bari.Plugins.AddonSupport.Model;
using Bari.Plugins.AddonSupport.Model.Loader;
using FluentAssertions;
using Moq;
using Ninject;
using NUnit.Framework;

namespace Bari.Plugins.AddonSupport.Test.Model
{
    [TestFixture]
    public class StartupModuleParametersLoaderTest
    {
        private IKernel kernel;
        private Mock<IParameters> parameters;
        private readonly TestUserOutput testOutput = new TestUserOutput();

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            kernel.Bind<IYamlProjectParametersLoader>().To<StartupModuleParametersLoader>();

            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("root")).WhenTargetHas
                <SuiteRootAttribute>();
            kernel.Bind<IFileSystemDirectory>()
                .ToConstant(new TestFileSystemDirectory("target"))
                .WhenTargetHas<TargetRootAttribute>();
            kernel.Bind<IFileSystemDirectory>()
                .ToConstant(new TestFileSystemDirectory("cache"))
                .WhenTargetHas<CacheRootAttribute>();
            kernel.Bind<IUserOutput>().ToConstant(testOutput);

            parameters = new Mock<IParameters>();
            parameters.SetupGet(p => p.Goal).Returns("debug");
            kernel.Bind<IParameters>().ToConstant(parameters.Object);
            TestSetup.EnsureFactoryExtensionLoaded(kernel);
        }

        [Test]
        public void ModuleNameAccepted()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
    - name: Module1
      projects: 
        - name: Project11
          type: executable
    - name: Module2  
      projects: 
        - name: Project21
        - name: Project22
          type: executable
    - name: Module3

products:
    - name: testproduct
      startup: Module2
      modules:
        - Module1
        - Module2
        - Module3
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            var p = suite.GetProduct("testproduct").GetParameters<StartupModuleParameters>("startup");
            p.Project.Should().NotBeNull();
            p.Project.Name.Should().Be("Project22");
        }

        [Test]
        public void ProjectNameAccepted()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
    - name: Module1
      projects: 
        - name: Project11
          type: executable
    - name: Module2  
      projects: 
        - name: Project21
        - name: Project22
          type: executable
    - name: Module3

products:
    - name: testproduct
      startup: Project22
      modules:
        - Module1
        - Module2
        - Module3
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            var p = suite.GetProduct("testproduct").GetParameters<StartupModuleParameters>("startup");
            p.Project.Should().NotBeNull();
            p.Project.Name.Should().Be("Project22");
        }
    }
}