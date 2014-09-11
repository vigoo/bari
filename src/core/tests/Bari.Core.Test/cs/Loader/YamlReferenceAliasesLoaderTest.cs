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
            kernel.Bind<IFileSystemDirectory>().ToConstant(new TestFileSystemDirectory("cache")).WhenTargetHas<CacheRootAttribute>();
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
            var loader = new YamlReferenceAliasesLoader();
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

        [Test]
        public void ConditionalAliases()
        {
            const string yaml = @"---
suite: test
goals: 
   - name: test-x86
     incorporates:
        - x86
   - name: test-x64
     incorporates:
        - x64
aliases:
    ref1:
        - when x86:
            - ref://x86/1
            - ref://x86/2
        - when x64:
            - ref://x64/1
            - ref://x64/2
";

            parameters.SetupGet(p => p.Goal).Returns("test-x86");
            var aliasesX86 = LoadAliases(yaml);
            aliasesX86.Names.Should().HaveCount(1);
            aliasesX86.Names.First().Should().Be("ref1");

            var refX86 = aliasesX86.Get("ref1");
            refX86.Should().NotBeNull();

            refX86.References.Should().HaveCount(2);
            refX86.References.Should().Contain(new Reference(new Uri("ref://x86/1"), ReferenceType.Build));
            refX86.References.Should().Contain(new Reference(new Uri("ref://x86/2"), ReferenceType.Build));

            parameters.SetupGet(p => p.Goal).Returns("test-x64");
            var aliasesX64= LoadAliases(yaml);
            aliasesX64.Names.Should().HaveCount(1);
            aliasesX64.Names.First().Should().Be("ref1");

            var refX64 = aliasesX64.Get("ref1");
            refX64.Should().NotBeNull();

            refX64.References.Should().HaveCount(2);
            refX64.References.Should().Contain(new Reference(new Uri("ref://x64/1"), ReferenceType.Build));
            refX64.References.Should().Contain(new Reference(new Uri("ref://x64/2"), ReferenceType.Build));
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