using System.Linq;
using Bari.Core;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.Test;
using Bari.Core.Test.Helper;
using Bari.Core.UI;
using Bari.Plugins.FSRepository.Model;
using Bari.Plugins.FSRepository.Model.Loader;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ninject;

namespace Bari.Plugins.FSRepository.Test.Model.Loader
{
    [TestFixture]
    public class YamlRepositoryPatternCollectionLoaderTest
    {
        private IFileSystemRepositoryAccess fsAccess;
        private StandardKernel kernel;
        private Mock<IParameters> parameters;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
            Kernel.RegisterCoreBindings(kernel);
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("root")).WhenTargetHas
                <SuiteRootAttribute>();
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("target")).WhenTargetHas<TargetRootAttribute>();
            kernel.Bind<IUserOutput>().To<TestUserOutput>();

            parameters = new Mock<IParameters>();
            parameters.SetupGet(p => p.Goal).Returns("debug");
            kernel.Bind<IParameters>().ToConstant(parameters.Object);
            TestSetup.EnsureFactoryExtensionLoaded(kernel);
            
            kernel.Rebind<ISuiteFactory>().To<DefaultSuiteFactory>().InTransientScope();

            fsAccess = new Mock<IFileSystemRepositoryAccess>().Object;
            kernel.Bind<IFileSystemRepositoryAccess>().ToConstant(fsAccess);
            kernel.Bind<IYamlProjectParametersLoader>().To<YamlRepositoryPatternCollectionLoader>();
        }

        [Test]
        public void Supports()
        {
            var loader = new YamlRepositoryPatternCollectionLoader(new Suite(new Mock<IFileSystemDirectory>().Object), fsAccess);
            loader.Supports("fs-repositories").Should().BeTrue();
        }

        [Test]
        public void EmptyBlock()
        {            
            const string yaml = @"---                 
suite: test  
fs-repositories:
";
            var repos = LoadCollection(yaml);
            repos.Patterns.Should().BeEmpty();
        }

        [Test]
        public void SingleItem()
        {
            const string yaml = @"---                   
suite: test
fs-repositories:
    - my-first-pattern
";
            var repos = LoadCollection(yaml);
            repos.Patterns.Should().HaveCount(1);
            repos.Patterns.First().Pattern.Should().Be("my-first-pattern");
        }

        [Test]
        public void MultipleItems()
        {
            const string yaml = @"---                   
suite: test
fs-repositories:
    - my-first-pattern
    - my-second-pattern
    - my-third-pattern
";
            var repos = LoadCollection(yaml);
            repos.Patterns.Should().HaveCount(3);
            repos.Patterns.ElementAt(0).Pattern.Should().Be("my-first-pattern");
            repos.Patterns.ElementAt(1).Pattern.Should().Be("my-second-pattern");
            repos.Patterns.ElementAt(2).Pattern.Should().Be("my-third-pattern");
        }

        [Test]
        public void ConditionalItems()
        {
            const string yaml = @"---                   
suite: test
goals:
  - name: none
  - name: x86
  - name: x64

fs-repositories:
    - when x86:
       - my-32bit-repo
    - when x64:
       - my-64bit-repo
    - my-common-repo
";
            parameters.SetupGet(p => p.Goal).Returns("none");
            var repos = LoadCollection(yaml);
            repos.Patterns.Should().HaveCount(1);
            repos.Patterns.ElementAt(0).Pattern.Should().Be("my-common-repo");

            parameters.SetupGet(p => p.Goal).Returns("x86");
            var reposX86 = LoadCollection(yaml);
            reposX86.Patterns.Should().HaveCount(2);
            reposX86.Patterns.ElementAt(0).Pattern.Should().Be("my-32bit-repo");
            reposX86.Patterns.ElementAt(1).Pattern.Should().Be("my-common-repo");

            parameters.SetupGet(p => p.Goal).Returns("x64");
            var reposX64 = LoadCollection(yaml);
            reposX64.Patterns.Should().HaveCount(2);
            reposX64.Patterns.ElementAt(0).Pattern.Should().Be("my-64bit-repo");
            reposX64.Patterns.ElementAt(1).Pattern.Should().Be("my-common-repo");
        }

        private RepositoryPatternCollection LoadCollection(string yaml)
        {
            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);
            suite.HasParameters("fs-repositories").Should().BeTrue();
            return suite.GetParameters<RepositoryPatternCollection>("fs-repositories");
        }        
    }
}