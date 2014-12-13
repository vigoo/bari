using System;
using System.IO;
using System.Text;
using Bari.Core.Build;
using Bari.Core.Build.Cache;
using Bari.Core.Generic;
using Bari.Core.Test.Helper;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bari.Core.Test.Build.Cache
{
    [TestFixture]
    public class MemoryBuildCacheTest
    {
        private MemoryBuildCache cache;
        private BuildKey T;
        private IFileSystemDirectory root;
        private Mock<IDependencyFingerprint> fingerprint;
        private Mock<IDependencyFingerprint> otherFingerprint;

        [SetUp]
        public void SetUp()
        {
            cache = new MemoryBuildCache();
            T = new BuildKey(typeof(IBuilder), "test");
            root = new TestFileSystemDirectory("root");           

            fingerprint = new Mock<IDependencyFingerprint>();
            otherFingerprint = new Mock<IDependencyFingerprint>();

            fingerprint.Setup(f => f.Equals(fingerprint.Object)).Returns(true);
            fingerprint.Setup(f => f.Equals(otherFingerprint.Object)).Returns(false);

            otherFingerprint.Setup(f => f.Equals(fingerprint.Object)).Returns(false);
            otherFingerprint.Setup(f => f.Equals(otherFingerprint.Object)).Returns(true);
        }

        [TearDown]
        public void TearDown()
        {
            cache.Dispose();
        }

        [Test]
        public void CanLockAndUnlock()
        {                        
            cache.LockForBuilder(T);
            cache.UnlockForBuilder(T);
        }

        [Test]
        public void CanLockAndUnlockNested()
        {
            cache.LockForBuilder(T);
            cache.LockForBuilder(T);
            cache.UnlockForBuilder(T);
            cache.UnlockForBuilder(T);
        }

        [Test]
        public void InitiallyEmpty()
        {
            cache.Contains(T, fingerprint.Object).Should().BeFalse();
        }

        [Test]
        public void ContainsItemAfterStore()
        {
            cache.Store(T, fingerprint.Object, new TargetRelativePath[0], root);
            cache.Contains(T, fingerprint.Object).Should().BeTrue();
            cache.Contains(T, otherFingerprint.Object).Should().BeFalse();
        }

        [Test]
        public void ContainsItemAfterStoreWithLock()
        {
            cache.LockForBuilder(T);
            cache.Store(T, fingerprint.Object, new TargetRelativePath[0], root);
            cache.Contains(T, fingerprint.Object).Should().BeTrue();
            cache.Contains(T, otherFingerprint.Object).Should().BeFalse();
            cache.UnlockForBuilder(T);
        }

        [Test]
        public void StoreReplacePreviousItem()
        {
            cache.Store(T, fingerprint.Object, new TargetRelativePath[0], root);
            cache.Store(T, otherFingerprint.Object, new TargetRelativePath[0], root);
            cache.Contains(T, fingerprint.Object).Should().BeFalse();
            cache.Contains(T, otherFingerprint.Object).Should().BeTrue();
        }

        [Test]
        public void StoreReplacePreviousItemWithLock()
        {
            cache.LockForBuilder(T);
            cache.Store(T, fingerprint.Object, new TargetRelativePath[0], root);
            cache.Store(T, otherFingerprint.Object, new TargetRelativePath[0], root);
            cache.Contains(T, fingerprint.Object).Should().BeFalse();
            cache.Contains(T, otherFingerprint.Object).Should().BeTrue();
            cache.UnlockForBuilder(T);
        }

        [Test]
        public void RestoreWorks()
        {
            cache.Store(T, fingerprint.Object,
                        new[]
                            {
                                new TargetRelativePath(String.Empty, "a"),
                                new TargetRelativePath(String.Empty, "b"),
                                new TargetRelativePath(String.Empty, "c")
                            }, root);

            var target = new Mock<IFileSystemDirectory>();

            var a = new MemoryStream();
            var b = new MemoryStream();
            var c = new MemoryStream();

            target.Setup(t => t.CreateBinaryFile("a")).Returns(a);
            target.Setup(t => t.CreateBinaryFile("b")).Returns(b);
            target.Setup(t => t.CreateBinaryFile("c")).Returns(c);
            
            var paths = cache.Restore(T, target.Object, false);

            paths.Should().HaveCount(3).And.Contain(
                new[]
                    {
                        new TargetRelativePath(String.Empty, "a"),
                        new TargetRelativePath(String.Empty, "b"),
                        new TargetRelativePath(String.Empty, "c")
                    });

            target.Verify(t => t.CreateBinaryFile("a"), Times.Once());
            target.Verify(t => t.CreateBinaryFile("b"), Times.Once());
            target.Verify(t => t.CreateBinaryFile("c"), Times.Once());

            Encoding.UTF8.GetString(a.ToArray()).Should().Be("a");
            Encoding.UTF8.GetString(b.ToArray()).Should().Be("b");
            Encoding.UTF8.GetString(c.ToArray()).Should().Be("c");
        }

        [Test]
        public void RestoreWorksWithLocks()
        {
            cache.LockForBuilder(T);
            RestoreWorks();
            cache.UnlockForBuilder(T);
        }

        [Test]
        public void RestoreNonCachedItemDoesNotDoAnything()
        {
            cache.Restore(T, root, false).Should().BeEmpty();
        }
    }
}