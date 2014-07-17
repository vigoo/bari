using System;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.Test.Helper;
using Bari.Core.UI;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Loader
{

    [TestFixture]
    public class YamlReferenceAliasesLoaderTest
    {
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
        }

        [Test]
        public void Supports()
        {
            var loader = new YamlReferenceAliasesLoader(new Suite(new Mock<IFileSystemDirectory>().Object));
            loader.Supports("aliases").Should().BeTrue();
        }

        [Test]
        public void EmptyBlock()
        {
            const string yaml = @"---                 
suite: test  
aliases:
";
            var aliases = LoadAliases(yaml);
            aliases.Names.Should().BeEmpty();
        }

        [Test]
        public void SingleAlias()
        {
            const string yaml = @"---                   
suite: test
aliases:
    ref1: ref://test1
";
            var aliases = LoadAliases(yaml);
            aliases.Names.Should().HaveCount(1);
            aliases.Names.First().Should().Be("ref1");

            var ref1 = aliases.Get("ref1");
            ref1.Should().NotBeNull();

            ref1.References.Should().HaveCount(1);
            ref1.References.Should().Contain(new Reference(new Uri("ref://test1"), ReferenceType.Build));
        }

        [Test]
        public void SingleItem()
        {
            const string yaml = @"---                   
suite: test
aliases:
    ref1:
      - ref://test1
      - ref://test2
";
            var aliases = LoadAliases(yaml);
            aliases.Names.Should().HaveCount(1);
            aliases.Names.First().Should().Be("ref1");

            var ref1 = aliases.Get("ref1");
            ref1.Should().NotBeNull();

            ref1.References.Should().HaveCount(2);
            ref1.References.Should().Contain(new Reference(new Uri("ref://test1"), ReferenceType.Build));
            ref1.References.Should().Contain(new Reference(new Uri("ref://test2"), ReferenceType.Build));
        }

        [Test]
        public void MultipleItems()
        {
            const string yaml = @"---                   
suite: test
aliases:
    ref1:
        - ref://test1
        - ref://test2
    ref2:
        - ref://test3
        - ref://test4
        - ref://test5
";
            var aliases = LoadAliases(yaml);
            aliases.Names.Should().HaveCount(2);
            aliases.Names.Should().Contain("ref1");
            aliases.Names.Should().Contain("ref2");

            var ref1 = aliases.Get("ref1");
            ref1.Should().NotBeNull();

            ref1.References.Should().HaveCount(2);
            ref1.References.Should().Contain(new Reference(new Uri("ref://test1"), ReferenceType.Build));
            ref1.References.Should().Contain(new Reference(new Uri("ref://test2"), ReferenceType.Build));

            var ref2 = aliases.Get("ref2");
            ref2.Should().NotBeNull();

            ref2.References.Should().HaveCount(3);
            ref2.References.Should().Contain(new Reference(new Uri("ref://test3"), ReferenceType.Build));
            ref2.References.Should().Contain(new Reference(new Uri("ref://test4"), ReferenceType.Build));
            ref2.References.Should().Contain(new Reference(new Uri("ref://test5"), ReferenceType.Build));
        }

        private ReferenceAliases LoadAliases(string yaml)
        {
            var loader = kernel.Get<InMemoryYamlModelLoader>();
            loader.Should().NotBeNull();

            var suite = loader.Load(yaml);
            suite.HasParameters("aliases").Should().BeTrue();
            return suite.GetParameters<ReferenceAliases>("aliases");
        }
    }
}