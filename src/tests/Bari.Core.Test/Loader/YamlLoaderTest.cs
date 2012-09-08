using Bari.Core.Model.Loader;
using FluentAssertions;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Loader
{
    [TestFixture]
    public class YamlLoaderTest
    {
        [Test]
        public void SuiteNameRead()
        {
            const string yaml = @"---                   
suite: Test suite";

            var loader = Kernel.Root.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Name.Should().Be("Test suite");
        }

        [Test]
        public void EmptyModulesNodeHasNoEffect()
        {
            const string yaml = @"---                   
suite: Test suite

modules:
";

            var loader = Kernel.Root.Get<InMemoryYamlModelLoader>();
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

            var loader = Kernel.Root.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);

            suite.Should().NotBeNull();
            suite.Modules.Should().HaveCount(3);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module1" ||
                                                    m.Name == "Module2" ||
                                                    m.Name == "Module3");
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
        - Project32
";

            var loader = Kernel.Root.Get<InMemoryYamlModelLoader>();
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
        }
    }
}