using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Core.Test.Helper;
using FluentAssertions;
using NUnit.Framework;
using System.IO;

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

            var suite = new Suite(fs);

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

            var suite = new Suite(fs);

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

            var suite = new Suite(fs);

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
        public void TestProjectsDiscovered()
        {
            var fs = CreateFsWithSourcesAndTests();

            var suite = new Suite(fs);
            suite.Modules.Should().BeEmpty();

            var discovery = new ModuleProjectDiscovery(fs);
            discovery.ExtendWithDiscoveries(suite);

            suite.Modules.Should().HaveCount(3);
            suite.Modules.Should().OnlyContain(m => m.Name == "Module1" ||
                                                    m.Name == "Module2" ||
                                                    m.Name == "Module3");

            var mod3 = suite.GetModule("Module3");
            mod3.Projects.Should().HaveCount(2);
            mod3.Projects.Should().Contain(p => p.Name == "Project31");
            mod3.Projects.Should().Contain(p => p.Name == "Project32");
            mod3.TestProjects.Should().HaveCount(2);
            mod3.TestProjects.Should().Contain(p => p.Name == "Project31.Test");
            mod3.TestProjects.Should().Contain(p => p.Name == "Project32.Test");
        }

        [Test]
        public void ExistingModulesMergedWithDiscoveredOnes()
        {
            var fs = new TestFileSystemDirectory("root",
                                                 new TestFileSystemDirectory("src",
                                                                             new TestFileSystemDirectory("Module1",
                                                                                                         new TestFileSystemDirectory
                                                                                                             ("Project11"))));

            var suite = new Suite(fs);

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

        [Test]
        public void SourceSetsDiscovered()
        {
            var fs = CreateFsWithSources();

            var suite = new Suite(fs);

            suite.Modules.Should().BeEmpty();

            var discovery = new ModuleProjectDiscovery(fs);
            discovery.ExtendWithDiscoveries(suite);

            var project = suite.GetModule("Module1").GetProject("Project11");
            project.SourceSets.Should().HaveCount(2);
            project.SourceSets.Should().Contain(set => set.Type == "cs");
            project.SourceSets.Should().Contain(set => set.Type == "fs");

            var csSet = project.GetSourceSet("cs");
            var fsSet = project.GetSourceSet("fs");

            csSet.Files.Should().HaveCount(3);
            csSet.Files.Should().Contain(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "cs", "source1.cs")));
            csSet.Files.Should().Contain(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "cs", "source2.cs")));
            csSet.Files.Should().Contain(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "cs", "subdir", "source3.cs")));

            fsSet.Files.Should().HaveCount(1);
            fsSet.Files.Should().HaveElementAt(0, new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "fs", "a.fs")));
        }

        private static TestFileSystemDirectory CreateFsWithSources()
        {
            var fs = new TestFileSystemDirectory(
                "root",
                new TestFileSystemDirectory(
                    "src",
                    new TestFileSystemDirectory(
                        "Module1",
                        new TestFileSystemDirectory
                            ("Project11",
                             new TestFileSystemDirectory
                                 ("cs",
                                  new TestFileSystemDirectory
                                      ("subdir")
                                      {
                                          Files =new[] { "source3.cs" }
                                      })
                                 {
                                     Files = new[] { "source1.cs","source2.cs" }
                                 },
                             new TestFileSystemDirectory
                                 ("fs")
                                 {
                                     Files = new[] { "a.fs" }
                                 })),
                    new TestFileSystemDirectory(
                        "Module2"),
                    new TestFileSystemDirectory(
                        "Module3",
                        new TestFileSystemDirectory
                            ("Project31"),
                        new TestFileSystemDirectory
                            ("Project32"))),
                new TestFileSystemDirectory("output"));
            return fs;
        }

        private static TestFileSystemDirectory CreateFsWithSourcesAndTests()
        {
            var fs = new TestFileSystemDirectory(
                "root",
                new TestFileSystemDirectory(
                    "src",
                    new TestFileSystemDirectory(
                        "Module1",
                        new TestFileSystemDirectory
                            ("Project11",
                             new TestFileSystemDirectory
                                 ("cs",
                                  new TestFileSystemDirectory
                                      ("subdir")
                                  {
                                      Files = new[] { "source3.cs" }
                                  })
                             {
                                 Files = new[] { "source1.cs", "source2.cs" }
                             },
                             new TestFileSystemDirectory
                                 ("fs")
                             {
                                 Files = new[] { "a.fs" }
                             })),
                    new TestFileSystemDirectory(
                        "Module2"),
                    new TestFileSystemDirectory(
                        "Module3",
                        new TestFileSystemDirectory
                            ("Project31"),
                        new TestFileSystemDirectory
                            ("Project32"),
                        new TestFileSystemDirectory(
                            ("tests"),
                            new TestFileSystemDirectory("Project31.Test",
                                new TestFileSystemDirectory("cs")
                                    {
                                        Files = new[] { "test1.cs"}
                                    }),
                            new TestFileSystemDirectory("Project32.Test",
                                new TestFileSystemDirectory("cs")
                                    {
                                        Files = new[] { "test2.cs, test3.cs" }
                                    })))),
                new TestFileSystemDirectory("target"));
            return fs;
        }

        [Test]
        public void ExistingSourcesMergedWithDiscoveredOnes()
        {
            var fs = CreateFsWithSources();

            var suite = new Suite(fs);

            var fsSet = suite.GetModule("Module1").GetProject("Project11").GetSourceSet("fs");
            var vbSet = suite.GetModule("Module1").GetProject("Project11").GetSourceSet("vb");

            fsSet.Add(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "fs", "b.fs")));
            vbSet.Add(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "vb", "x.vb")));

            var discovery = new ModuleProjectDiscovery(fs);
            discovery.ExtendWithDiscoveries(suite);

            var project = suite.GetModule("Module1").GetProject("Project11");
            project.SourceSets.Should().HaveCount(3);
            project.SourceSets.Should().Contain(set => set.Type == "cs");
            project.SourceSets.Should().Contain(set => set.Type == "fs");
            project.SourceSets.Should().Contain(set => set.Type == "vb");

            var csSet = project.GetSourceSet("cs");
            fsSet = project.GetSourceSet("fs");
            vbSet = project.GetSourceSet("vb");

            csSet.Files.Should().HaveCount(3);
            csSet.Files.Should().Contain(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "cs", "source1.cs")));
            csSet.Files.Should().Contain(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "cs", "source2.cs")));
            csSet.Files.Should().Contain(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "cs", "subdir", "source3.cs")));

            fsSet.Files.Should().HaveCount(2);
            fsSet.Files.Should().Contain(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "fs", "a.fs")));
            fsSet.Files.Should().Contain(new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "fs", "b.fs")));

            vbSet.Files.Should().HaveCount(1);
            vbSet.Files.Should().HaveElementAt(0, new SuiteRelativePath(Path.Combine("src", "Module1", "Project11", "vb", "x.vb")));
        }
    }
}