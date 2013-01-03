using System;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.Test.Helper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ninject;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Test.Loader
{
    [TestFixture]
    public class YamlLoaderTest
    {
        private IKernel kernel;

        [SetUp]
        public void Setup()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("root")).WhenTargetHas
                <SuiteRootAttribute>();
        }

        [Test]
        public void SuiteNameRead()
        {
            const string yaml = @"---                   
suite: Test suite";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Name.Should().Be("Test suite");
            suite.Version.Should().BeNull();
        }

        [Test]
        public void SuiteVersionRead()
        {
            const string yaml = @"---                   
suite: Test suite
version: 1.0";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Name.Should().Be("Test suite");
            suite.Version.Should().Be("1.0");
        }

        [Test]
        public void EmptyModulesNodeHasNoEffect()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Modules.Should().BeEmpty();
        }

        [Test]
        public void MultipleModulesWithoutProjectsAreRead()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
    - Module1
    - Module2
    - Module3
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Modules.Should().HaveCount(3);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module1" ||
                                                    m.Name == "Module2" ||
                                                    m.Name == "Module3");
        }

        [Test]
        public void ModuleVersionRead()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
    - name: Module1
      version: 2.0
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Modules.Should().HaveCount(1);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module1");
            suite.GetModule("Module1").Version.Should().Be("2.0");
        }

        [Test]
        public void MultipleModulesWithProjectsAreRead()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
    - name: Module1
      projects: 
        - Project11
    - name: Module2
    - name: Module3
      projects:
        - Project31
        - name: Project32
          version: 3.0
          type: executable
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Modules.Should().HaveCount(3);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module1" ||
                                                    m.Name == "Module2" ||
                                                    m.Name == "Module3");
            suite.GetModule("Module1").Projects.Should().HaveCount(1);
            suite.GetModule("Module1").Projects.Should().Contain(p => p.Name == "Project11");
            suite.GetModule("Module2").Projects.Should().HaveCount(0);
            suite.GetModule("Module3").Projects.Should().HaveCount(2);
            suite.GetModule("Module3").Projects.Should().Contain(p => p.Name == "Project31");
            suite.GetModule("Module3").Projects.Should().Contain(p => p.Name == "Project32");

            suite.GetModule("Module1").GetProject("Project11").Type.Should().Be(ProjectType.Library);
            suite.GetModule("Module3").GetProject("Project31").Type.Should().Be(ProjectType.Library);
            suite.GetModule("Module3").GetProject("Project32").Type.Should().Be(ProjectType.Executable);

            suite.GetModule("Module1").GetProject("Project11").Version.Should().BeNull();
            suite.GetModule("Module3").GetProject("Project31").Version.Should().BeNull();
            suite.GetModule("Module3").GetProject("Project32").Version.Should().Be("3.0");
        }

        [Test]
        public void MultipleModulesWithProjectsAndTestProjectsAreRead()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
    - name: Module1
      projects: 
        - Project11
      tests:
        - Project11.Test
    - name: Module2
      tests:
        - SpecialTest
    - name: Module3
      projects:
        - Project31
        - name: Project32
          type: executable
      tests:
        - name: Project31.Test
        - name: Project32.Test
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Modules.Should().HaveCount(3);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module1" ||
                                                    m.Name == "Module2" ||
                                                    m.Name == "Module3");
            suite.GetModule("Module1").Projects.Should().HaveCount(1);
            suite.GetModule("Module1").Projects.Should().Contain(p => p.Name == "Project11");
            suite.GetModule("Module1").TestProjects.Should().HaveCount(1);
            suite.GetModule("Module1").TestProjects.Should().Contain(p => p.Name == "Project11.Test");

            suite.GetModule("Module2").Projects.Should().HaveCount(0);
            suite.GetModule("Module2").TestProjects.Should().HaveCount(1);
            suite.GetModule("Module2").TestProjects.Should().Contain(p => p.Name == "SpecialTest");

            suite.GetModule("Module3").Projects.Should().HaveCount(2);
            suite.GetModule("Module3").Projects.Should().Contain(p => p.Name == "Project31");
            suite.GetModule("Module3").Projects.Should().Contain(p => p.Name == "Project32");
            suite.GetModule("Module3").TestProjects.Should().HaveCount(2);
            suite.GetModule("Module3").TestProjects.Should().Contain(p => p.Name == "Project31.Test");
            suite.GetModule("Module3").TestProjects.Should().Contain(p => p.Name == "Project32.Test");
        }

        [Test]
        public void ProjectReferencesRead()
        {
            const string yaml = @"---                   
suite: Test suite

modules:    
    - name: Module
      projects:
        - name: Project
          references:
            - test://ref1
            - test://ref2
            - test://ref3
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Modules.Should().HaveCount(1);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module");
            suite.GetModule("Module").Projects.Should().HaveCount(1);
            suite.GetModule("Module").Projects.Should().Contain(p => p.Name == "Project");
            var prj = suite.GetModule("Module").GetProject("Project");

            prj.Should().NotBeNull();
            prj.References.Should().NotBeEmpty();
            prj.References.Should().HaveCount(3);
            prj.References.Should().OnlyContain(r => r.Uri == new Uri("test://ref1") ||
                                                     r.Uri == new Uri("test://ref2") ||
                                                     r.Uri == new Uri("test://ref3"));
        }

        [Test]
        public void TestProjectReferencesRead()
        {
            const string yaml = @"---                   
suite: Test suite

modules:    
    - name: Module
      projects:
        - name: Project
      tests:
        - name: Test1
          references:
            - test://ref1
            - test://ref2
            - test://ref3
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Modules.Should().HaveCount(1);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module");
            suite.GetModule("Module").TestProjects.Should().HaveCount(1);
            suite.GetModule("Module").TestProjects.Should().Contain(p => p.Name == "Test1");
            var prj = suite.GetModule("Module").GetTestProject("Test1");

            prj.Should().NotBeNull();
            prj.References.Should().NotBeEmpty();
            prj.References.Should().HaveCount(3);
            prj.References.Should().OnlyContain(r => r.Uri == new Uri("test://ref1") ||
                                                     r.Uri == new Uri("test://ref2") ||
                                                     r.Uri == new Uri("test://ref3"));
        }

        [Test]
        public void AllLoadersAskedForUnknownSuiteSections()
        {
            const string yaml = @"---                   
suite: Test suite
test1: something
test2:
    - one
    - two
    - three
";

            var paramLoader = new Mock<IYamlProjectParametersLoader>();
            kernel.Bind<IYamlProjectParametersLoader>().ToConstant(paramLoader.Object);
            
            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            paramLoader.Verify(l => l.Supports("test1"), Times.AtLeastOnce());
            paramLoader.Verify(l => l.Supports("test2"), Times.AtLeastOnce());
        }

        [Test]
        public void AllLoadersAskedForUnknownModuleSections()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
   - name: mod1
     test1: something
   - name: mod2
     test2:
       - one
       - two
       - three
";

            var paramLoader = new Mock<IYamlProjectParametersLoader>();
            kernel.Bind<IYamlProjectParametersLoader>().ToConstant(paramLoader.Object);

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            paramLoader.Verify(l => l.Supports("test1"), Times.AtLeastOnce());
            paramLoader.Verify(l => l.Supports("test2"), Times.AtLeastOnce());
        }

        [Test]
        public void AllLoadersAskedForUnknownProjectSections()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
   - name: mod1
     projects: 
        - name: proj1
          test1: anything
     tests:
        - name: testproj
          test2:
             - one
             - two
             - three
";

            var paramLoader = new Mock<IYamlProjectParametersLoader>();
            kernel.Bind<IYamlProjectParametersLoader>().ToConstant(paramLoader.Object);

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            paramLoader.Verify(l => l.Supports("test1"), Times.AtLeastOnce());
            paramLoader.Verify(l => l.Supports("test2"), Times.AtLeastOnce());
        }

        [Test]
        public void LoaderCalledForUnknownSection()
        {
            const string yaml = @"---                   
suite: Test suite
test1: something
test2:
    - one
    - two
    - three
";

            var paramLoader = new Mock<IYamlProjectParametersLoader>();
            paramLoader.Setup(l => l.Supports("test1")).Returns(true);

            kernel.Bind<IYamlProjectParametersLoader>().ToConstant(paramLoader.Object);

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            paramLoader.Verify(l => l.Load("test1", It.Is<YamlNode>(node => (node is YamlScalarNode) && ((YamlScalarNode) node).Value == "something")), Times.Once());
            paramLoader.Verify(l => l.Load("test2", It.IsAny<YamlNode>()), Times.Never());
        }
    }
}