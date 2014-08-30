using System;
using System.Linq;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.Test.Helper;
using Bari.Core.UI;
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
        private Mock<IParameters> parameters;
        private readonly TestUserOutput testOutput = new TestUserOutput();

        [SetUp]
        public void Setup()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("root")).WhenTargetHas
                <SuiteRootAttribute>();
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("target")).WhenTargetHas<TargetRootAttribute>();
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("cache")).WhenTargetHas<CacheRootAttribute>();
            kernel.Bind<IUserOutput>().ToConstant(testOutput);
            
            parameters = new Mock<IParameters>();
            parameters.SetupGet(p => p.Goal).Returns("debug");
            kernel.Bind<IParameters>().ToConstant(parameters.Object);
            TestSetup.EnsureFactoryExtensionLoaded(kernel);
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
            suite.Copyright.Should().BeNull();
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
        public void SuiteCopyrightRead()
        {
            const string yaml = @"---                   
suite: Test suite
copyright: test";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Name.Should().Be("Test suite");
            suite.Copyright.Should().Be("test");
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
        public void ModuleCopyrightRead()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
    - name: Module1
      copyright: test2
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Modules.Should().HaveCount(1);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module1");
            suite.GetModule("Module1").Copyright.Should().Be("test2");
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
        - name: Project31
          type: static-library
        - name: Project32
          version: 3.0
          copyright: test3
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
            suite.GetModule("Module3").GetProject("Project31").Type.Should().Be(ProjectType.StaticLibrary);
            suite.GetModule("Module3").GetProject("Project32").Type.Should().Be(ProjectType.Executable);

            suite.GetModule("Module1").GetProject("Project11").Version.Should().BeNull();
            suite.GetModule("Module1").GetProject("Project11").Copyright.Should().BeNull();
            suite.GetModule("Module3").GetProject("Project31").Version.Should().BeNull();
            suite.GetModule("Module3").GetProject("Project31").Copyright.Should().BeNull();
            suite.GetModule("Module3").GetProject("Project32").Version.Should().Be("3.0");
            suite.GetModule("Module3").GetProject("Project32").Copyright.Should().Be("test3");
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
            - { uri: 'test://ref2', type: Runtime }
            - { uri: 'test://ref3', type: Build }
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
            prj.References.Should().OnlyContain(r => r == new Reference(new Uri("test://ref1"), ReferenceType.Build) ||
                                                     r == new Reference(new Uri("test://ref2"), ReferenceType.Runtime) ||
                                                     r == new Reference(new Uri("test://ref3"), ReferenceType.Build));
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
        public void PostprocessorsLoaded1()
        {
            const string yaml = @"---                   
suite: Test suite

modules:    
    - name: Module
      projects:
        - name: Project
          postprocessors:
             - pptype1
             - pptype2
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            suite.GetModule("Module").GetProject("Project").PostProcessors.Should().HaveCount(2);
            suite.GetModule("Module").GetProject("Project").PostProcessors.Should().OnlyContain(
                p => (p.Name == "pptype1" && p.PostProcessorId == new PostProcessorId("pptype1")) ||
                     (p.Name == "pptype2" && p.PostProcessorId == new PostProcessorId("pptype2")));
        }

        [Test]
        public void PostprocessorsLoaded2()
        {
            const string yaml = @"---                   
suite: Test suite

modules:    
    - name: Module
      projects:
        - name: Project
          postprocessors:
             - name: pp1
               type: pptype1
             - name: pp2
               type: pptype2
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            suite.GetModule("Module").GetProject("Project").PostProcessors.Should().HaveCount(2);
            suite.GetModule("Module").GetProject("Project").PostProcessors.Should().OnlyContain(
                p => (p.Name == "pp1" && p.PostProcessorId == new PostProcessorId("pptype1")) ||
                     (p.Name == "pp2" && p.PostProcessorId == new PostProcessorId("pptype2")));
        }

        [Test]
        public void PostprocessorsLoadedWithParameters()
        {
            const string yaml = @"---                   
suite: Test suite

modules:    
    - name: Module
      projects:
        - name: Project
          postprocessors:
             - name: pp1
               type: pptype1
               param: v1
             - name: pp2
               type: pptype2
               param: v2
";

            var paramLoader = new Mock<IYamlProjectParametersLoader>();
            kernel.Bind<IYamlProjectParametersLoader>().ToConstant(paramLoader.Object);

            var param1 = new Mock<IProjectParameters>();
            var param2 = new Mock<IProjectParameters>();

            paramLoader.Setup(l => l.Supports("pptype1")).Returns(true);
            paramLoader.Setup(l => l.Supports("pptype2")).Returns(true);

            paramLoader.Setup(l => l.Load("pptype1", It.IsAny<YamlNode>(), It.IsAny<YamlParser>()))
                .Returns(param1.Object);
            paramLoader.Setup(l => l.Load("pptype2", It.IsAny<YamlNode>(), It.IsAny<YamlParser>()))
                .Returns(param2.Object);

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.GetModule("Module").GetProject("Project").PostProcessors.Should().HaveCount(2);
            suite.GetModule("Module").GetProject("Project").PostProcessors.Should().OnlyContain(
                p => (p.Name == "pp1" && p.PostProcessorId == new PostProcessorId("pptype1")) ||
                     (p.Name == "pp2" && p.PostProcessorId == new PostProcessorId("pptype2")));

            paramLoader.Verify(l => l.Supports("pptype1"), Times.AtLeastOnce);
            paramLoader.Verify(l => l.Supports("pptype2"), Times.AtLeastOnce);

            suite.GetModule("Module").GetProject("Project").GetPostProcessor("pp1").Parameters.Should().Be(param1.Object);
            suite.GetModule("Module").GetProject("Project").GetPostProcessor("pp2").Parameters.Should().Be(param2.Object);
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
            var paramBlock = new Mock<IProjectParameters>();
            paramLoader.Setup(l => l.Supports("test1")).Returns(true);
            paramLoader.Setup(l => l.Load("test1", It.IsAny<YamlNode>(), It.IsAny<YamlParser>())).Returns(paramBlock.Object);

            kernel.Bind<IYamlProjectParametersLoader>().ToConstant(paramLoader.Object);

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            paramLoader.Verify(l => l.Load("test1", It.Is<YamlNode>(node => (node is YamlScalarNode) && ((YamlScalarNode) node).Value == "something"), It.IsAny<YamlParser>()), Times.Once());
            paramLoader.Verify(l => l.Load("test2", It.IsAny<YamlNode>(), It.IsAny<YamlParser>()), Times.Never());

            suite.GetParameters<IProjectParameters>("test1").Should().Be(paramBlock.Object);
        }

        [Test]
        public void ProductsLoaded()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
    - Module1
    - Module2
    - Module3

products:
    - name: one_three
      modules:
        - Module1
        - Module3
    - name: mod2
      modules:
        - Module2
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Products.Should().HaveCount(2);
            suite.HasProduct("one_three").Should().BeTrue();
            suite.HasProduct("mod2").Should().BeTrue();
            suite.Products.Should().Contain(p => p.Name == "one_three");
            suite.Products.Should().Contain(p => p.Name == "mod2");

            suite.GetProduct("one_three").Modules.Should().BeEquivalentTo(
                new[] {suite.GetModule("Module1"), suite.GetModule("Module3")});
            suite.GetProduct("mod2").Modules.Should().BeEquivalentTo(
                new[] { suite.GetModule("Module2") });
        }

        [Test]
        public void ProductsReferringToNonExistingModules()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
    - Module1
    - Module2
    - Module3

products:
    - name: invalid
      modules:
        - Module1
        - Module4
";

            testOutput.Reset();

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Products.Should().HaveCount(1);
            suite.HasProduct("invalid").Should().BeTrue();
            suite.Products.Should().Contain(p => p.Name == "invalid");

            suite.GetProduct("invalid").Modules.Should().BeEquivalentTo(
                new[] { suite.GetModule("Module1") });

            testOutput.Warnings.Should().HaveCount(1);
        }

        [Test]
        public void TestDefaultGoals()
        {
            const string yaml = @"---                   
suite: Test suite";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            var suite = loader.Load(yaml);

            suite.Goals.Should().BeEquivalentTo(new[]
                {
                    Suite.DebugGoal, Suite.ReleaseGoal
                });
        }

        [Test]
        public void DefiningSimpleGoals()
        {
            const string yaml = @"---                   
suite: Test suite

goals:
  - a
  - b
  - c
";

            parameters.SetupGet(p => p.Goal).Returns("b");
            var loader = kernel.Get<InMemoryYamlModelLoader>();
            var suite = loader.Load(yaml);

            suite.Goals.Should().BeEquivalentTo(new[]
                {
                    new Goal("a"), new Goal("b"), new Goal("c")
                });
            suite.ActiveGoal.Should().Be(new Goal("b"));
        }

        [Test]
        [ExpectedException(typeof (InvalidGoalException))]
        public void ExceptionThrownIfTargetGoalIsNotInTheGoalList()
        {
            const string yaml = @"---                   
suite: Test suite

goals:
  - a
  - b
  - c
";
            parameters.SetupGet(p => p.Goal).Returns("X");
            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Load(yaml);
        }

        [Test]
        public void GoalsDependingOnDefaultGoals()
        {
            const string yaml = @"---                   
suite: Test suite

goals:
  - name: test-goal
    incorporates:
      - debug
";
            parameters.SetupGet(p => p.Goal).Returns("test-goal");
            var loader = kernel.Get<InMemoryYamlModelLoader>();
            var suite = loader.Load(yaml);

            suite.Goals.Should().HaveCount(1);
            suite.Goals.First().Name.Should().Be("test-goal");
            suite.Goals.First().IncorporatedGoals.Should().BeEquivalentTo(
                new[] {Suite.DebugGoal});
        }

        [Test]
        public void ModulePostprocessorsLoaded1()
        {
            const string yaml = @"---                   
suite: Test suite

modules:    
    - name: Module
      projects:
        - name: Project          
      postprocessors:
        - pptype1
        - pptype2
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            suite.GetModule("Module").PostProcessors.Should().HaveCount(2);
            suite.GetModule("Module").PostProcessors.Should().OnlyContain(
                p => (p.Name == "pptype1" && p.PostProcessorId == new PostProcessorId("pptype1")) ||
                     (p.Name == "pptype2" && p.PostProcessorId == new PostProcessorId("pptype2")));
        }

        [Test]
        public void ModulePostprocessorsLoaded2()
        {
            const string yaml = @"---                   
suite: Test suite

modules:    
    - name: Module
      projects:
        - name: Project
      postprocessors:
        - name: pp1
          type: pptype1
        - name: pp2
          type: pptype2
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            suite.GetModule("Module").PostProcessors.Should().HaveCount(2);
            suite.GetModule("Module").PostProcessors.Should().OnlyContain(
                p => (p.Name == "pp1" && p.PostProcessorId == new PostProcessorId("pptype1")) ||
                     (p.Name == "pp2" && p.PostProcessorId == new PostProcessorId("pptype2")));
        }

        [Test]
        public void ModulePostprocessorsLoadedWithParameters()
        {
            const string yaml = @"---                   
suite: Test suite

modules:    
    - name: Module
      projects:
        - name: Project
      postprocessors:
        - name: pp1
          type: pptype1
          param: v1
        - name: pp2
          type: pptype2
          param: v2
";

            var paramLoader = new Mock<IYamlProjectParametersLoader>();
            kernel.Bind<IYamlProjectParametersLoader>().ToConstant(paramLoader.Object);

            var param1 = new Mock<IProjectParameters>();
            var param2 = new Mock<IProjectParameters>();

            paramLoader.Setup(l => l.Supports("pptype1")).Returns(true);
            paramLoader.Setup(l => l.Supports("pptype2")).Returns(true);

            paramLoader.Setup(l => l.Load("pptype1", It.IsAny<YamlNode>(), It.IsAny<YamlParser>()))
                .Returns(param1.Object);
            paramLoader.Setup(l => l.Load("pptype2", It.IsAny<YamlNode>(), It.IsAny<YamlParser>()))
                .Returns(param2.Object);

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.GetModule("Module").PostProcessors.Should().HaveCount(2);
            suite.GetModule("Module").PostProcessors.Should().OnlyContain(
                p => (p.Name == "pp1" && p.PostProcessorId == new PostProcessorId("pptype1")) ||
                     (p.Name == "pp2" && p.PostProcessorId == new PostProcessorId("pptype2")));

            paramLoader.Verify(l => l.Supports("pptype1"), Times.AtLeastOnce);
            paramLoader.Verify(l => l.Supports("pptype2"), Times.AtLeastOnce);

            suite.GetModule("Module").GetPostProcessor("pp1").Parameters.Should().Be(param1.Object);
            suite.GetModule("Module").GetPostProcessor("pp2").Parameters.Should().Be(param2.Object);
        }

        [Test]
        public void ProductPostprocessorsLoaded1()
        {
            const string yaml = @"---                   
suite: Test suite

products:    
    - name: Module
      postprocessors:
        - pptype1
        - pptype2
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            suite.GetProduct("Module").PostProcessors.Should().HaveCount(2);
            suite.GetProduct("Module").PostProcessors.Should().OnlyContain(
                p => (p.Name == "pptype1" && p.PostProcessorId == new PostProcessorId("pptype1")) ||
                     (p.Name == "pptype2" && p.PostProcessorId == new PostProcessorId("pptype2")));
        }

        [Test]
        public void ProductPostprocessorsLoaded2()
        {
            const string yaml = @"---                   
suite: Test suite

products:    
    - name: Module
      postprocessors:
        - name: pp1
          type: pptype1
        - name: pp2
          type: pptype2
";

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();

            suite.GetProduct("Module").PostProcessors.Should().HaveCount(2);
            suite.GetProduct("Module").PostProcessors.Should().OnlyContain(
                p => (p.Name == "pp1" && p.PostProcessorId == new PostProcessorId("pptype1")) ||
                     (p.Name == "pp2" && p.PostProcessorId == new PostProcessorId("pptype2")));
        }

        [Test]
        public void ProductPostprocessorsLoadedWithParameters()
        {
            const string yaml = @"---                   
suite: Test suite

products:    
    - name: Module
      postprocessors:
        - name: pp1
          type: pptype1
          param: v1
        - name: pp2
          type: pptype2
          param: v2
";

            var paramLoader = new Mock<IYamlProjectParametersLoader>();
            kernel.Bind<IYamlProjectParametersLoader>().ToConstant(paramLoader.Object);

            var param1 = new Mock<IProjectParameters>();
            var param2 = new Mock<IProjectParameters>();

            paramLoader.Setup(l => l.Supports("pptype1")).Returns(true);
            paramLoader.Setup(l => l.Supports("pptype2")).Returns(true);

            paramLoader.Setup(l => l.Load("pptype1", It.IsAny<YamlNode>(), It.IsAny<YamlParser>()))
                .Returns(param1.Object);
            paramLoader.Setup(l => l.Load("pptype2", It.IsAny<YamlNode>(), It.IsAny<YamlParser>()))
                .Returns(param2.Object);

            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.GetProduct("Module").PostProcessors.Should().HaveCount(2);
            suite.GetProduct("Module").PostProcessors.Should().OnlyContain(
                p => (p.Name == "pp1" && p.PostProcessorId == new PostProcessorId("pptype1")) ||
                     (p.Name == "pp2" && p.PostProcessorId == new PostProcessorId("pptype2")));

            paramLoader.Verify(l => l.Supports("pptype1"), Times.AtLeastOnce);
            paramLoader.Verify(l => l.Supports("pptype2"), Times.AtLeastOnce);

            suite.GetProduct("Module").GetPostProcessor("pp1").Parameters.Should().Be(param1.Object);
            suite.GetProduct("Module").GetPostProcessor("pp2").Parameters.Should().Be(param2.Object);
        }
    }
}