using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Core.Test.Helper;
using FluentAssertions;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Discovery
{
    [TestFixture]
    public class ModuleProjectDiscoveryTest
    {
        [Test]
        public void EmptyModulesDiscovered()
        {
            var fs = new TestFileSystemDirectory("root",
                                                 new TestFileSystemDirectory("src",
                                                                             new TestFileSystemDirectory("Module1"),
                                                                             new TestFileSystemDirectory("Module2"),
                                                                             new TestFileSystemDirectory("Module3")),
                                                 new TestFileSystemDirectory("output"));

            var suite =  Kernel.Root.Get<Suite>();

            suite.Modules.Should().BeEmpty();

            var discovery = new ModuleProjectDiscovery(fs);
            discovery.ExtendWithDiscoveries(suite);

            suite.Modules.Should().HaveCount(3);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module1" ||
                                                    m.Name == "Module2" ||
                                                    m.Name == "Module3");
        }

        [Test]
        public void NoSourceDirectoryDoesNotCauseError()
        {
            var fs = new TestFileSystemDirectory("root",
                                     new TestFileSystemDirectory("abc"),
                                     new TestFileSystemDirectory("output"));

            var suite = Kernel.Root.Get<Suite>();

            suite.Modules.Should().BeEmpty();

            var discovery = new ModuleProjectDiscovery(fs);
            discovery.ExtendWithDiscoveries(suite);

            suite.Modules.Should().BeEmpty();
        }

        [Test]
        public void ProjectsDiscovered()
        {
            var fs = new TestFileSystemDirectory("root",
                                     new TestFileSystemDirectory("src",
                                                                 new TestFileSystemDirectory("Module1",
                                                                     new TestFileSystemDirectory("Project11")),
                                                                 new TestFileSystemDirectory("Module2"),
                                                                 new TestFileSystemDirectory("Module3",
                                                                     new TestFileSystemDirectory("Project31"),
                                                                     new TestFileSystemDirectory("Project32"))),
                                     new TestFileSystemDirectory("output"));

            var suite = Kernel.Root.Get<Suite>();

            suite.Modules.Should().BeEmpty();

            var discovery = new ModuleProjectDiscovery(fs);
            discovery.ExtendWithDiscoveries(suite);

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

        [Test]
        public void ExistingModulesMergedWithDiscoveredOnes()
        {
            var fs = new TestFileSystemDirectory("root",
                                                 new TestFileSystemDirectory("src",
                                                                             new TestFileSystemDirectory("Module1",
                                                                                                         new TestFileSystemDirectory
                                                                                                             ("Project11"))));

            var suite = Kernel.Root.Get<Suite>();

            var module1 = suite.GetModule("Module1");
            var projectA = module1.GetProject("ProjectA");

            module1.Projects.Should().HaveCount(1);
            module1.Projects.Should().HaveElementAt(0, projectA);

            var discovery = new ModuleProjectDiscovery(fs);
            discovery.ExtendWithDiscoveries(suite);

            suite.Modules.Should().HaveCount(1);
            suite.Modules.Should().HaveElementAt(0, module1);
            module1.Projects.Should().HaveCount(2);
            module1.Projects.Should().Contain(projectA);
            module1.Projects.Should().Contain(p => p.Name == "Project11");
        }
    }
}