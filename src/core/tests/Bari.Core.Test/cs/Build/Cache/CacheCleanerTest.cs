using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Commands.Clean;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using FluentAssertions;
using Moq;
using Ninject;
using NUnit.Framework;

namespace Bari.Core.Test.Build.Cache
{
    class NonPersistentReference : IReferenceBuilder
    {
        public IDependencies Dependencies
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerable<IBuilder> Prerequisites
        {
            get { return new IBuilder[0]; }
        }

        public string Uid
        {
            get { throw new System.NotImplementedException(); }
        }

        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            throw new System.NotImplementedException();
        }

        public bool CanRun()
        {
            throw new NotImplementedException();
        }

        public Type BuilderType
        {
            get { return typeof(NonPersistentReference); }
        }

        public Reference Reference
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public bool IsEffective
        {
            get { throw new System.NotImplementedException(); }
        }
    }

    [PersistentReference]
    class PersistentReference : IReferenceBuilder
    {
        public IDependencies Dependencies
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Uid
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerable<IBuilder> Prerequisites
        {
            get { return new IBuilder[0]; }
        }

        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            throw new System.NotImplementedException();
        }

        public bool CanRun()
        {
            throw new NotImplementedException();
        }

        public Type BuilderType
        {
            get { return typeof (PersistentReference); }
        }

        public Reference Reference
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public bool IsEffective
        {
            get { throw new System.NotImplementedException(); }
        }
    }

    [TestFixture]
    public class CacheCleanerTest
    {
        [Test]
        public void DeletesCacheDirectory()
        {
            var be = new Mock<IBuilderEnumerator>();
            be.Setup(b => b.GetAllPersistentBuilders()).Returns(new Type[0]);

            var parameters = new Mock<ICleanParameters>();
            var predicates = new SoftCleanPredicates();
            var cdir = new TestFileSystemDirectory("cache");
            var cleaner = new CacheCleaner(new Lazy<IFileSystemDirectory>(() => cdir), be.Object, () => predicates);

            cdir.IsDeleted.Should().BeFalse();
            cleaner.Clean(parameters.Object);
            cdir.IsDeleted.Should().BeTrue();
        }

        [Test]
        public void BuilderEnumeratorFindsPersistentReferenceBuilders()
        {
            var builderEnumerator = Kernel.Root.Get<IBuilderEnumerator>();
            var persistentBuilders = builderEnumerator.GetAllPersistentBuilders().ToList();

            persistentBuilders.Should().Contain(typeof (PersistentReference));
            persistentBuilders.Should().NotContain(typeof (NonPersistentReference));
        }

        [Test]
        public void KeepsPersistentReferences()
        {
            var cdir = new TestFileSystemDirectory("cache",
                new[]
                {
                    new TestFileSystemDirectory("Bari.Core.Test.Build.Cache.NonPersistentReference_1"), 
                    new TestFileSystemDirectory("Bari.Core.Test.Build.Cache.NonPersistentReference_2"), 
                    new TestFileSystemDirectory("Bari.Core.Test.Build.Cache.PersistentReference_3"), 
                    new TestFileSystemDirectory("Bari.Core.Test.Build.Cache.PersistentReference_4")
                });

            var be = new Mock<IBuilderEnumerator>();
            be.Setup(b => b.GetAllPersistentBuilders()).Returns(new[] {typeof(PersistentReference)});

            var predicates = new SoftCleanPredicates();
            var cleaner = new CacheCleaner(new Lazy<IFileSystemDirectory>(() => cdir), be.Object, () => predicates);

            cdir.IsDeleted.Should().BeFalse();
            ((TestFileSystemDirectory) cdir.GetChildDirectory("Bari.Core.Test.Build.Cache.NonPersistentReference_1"))
                .IsDeleted.Should().BeFalse();
            ((TestFileSystemDirectory)cdir.GetChildDirectory("Bari.Core.Test.Build.Cache.NonPersistentReference_2"))
                .IsDeleted.Should().BeFalse();
            ((TestFileSystemDirectory)cdir.GetChildDirectory("Bari.Core.Test.Build.Cache.PersistentReference_3"))
                .IsDeleted.Should().BeFalse();
            ((TestFileSystemDirectory)cdir.GetChildDirectory("Bari.Core.Test.Build.Cache.PersistentReference_4"))
                .IsDeleted.Should().BeFalse();

            var parameters = new Mock<ICleanParameters>();
            parameters.SetupGet(p => p.KeepReferences).Returns(true);

            cleaner.Clean(parameters.Object);

            cdir.IsDeleted.Should().BeFalse();
            ((TestFileSystemDirectory)cdir.GetChildDirectory("Bari.Core.Test.Build.Cache.NonPersistentReference_1"))
                .IsDeleted.Should().BeTrue();
            ((TestFileSystemDirectory)cdir.GetChildDirectory("Bari.Core.Test.Build.Cache.NonPersistentReference_2"))
                .IsDeleted.Should().BeTrue();
            ((TestFileSystemDirectory)cdir.GetChildDirectory("Bari.Core.Test.Build.Cache.PersistentReference_3"))
                .IsDeleted.Should().BeFalse();
            ((TestFileSystemDirectory)cdir.GetChildDirectory("Bari.Core.Test.Build.Cache.PersistentReference_4"))
                .IsDeleted.Should().BeFalse();
        }
    }
}