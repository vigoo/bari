using System;
using System.IO;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Test.Helper;
using Bari.Plugins.FSRepository.Build.Dependencies;
using Bari.Plugins.FSRepository.Model;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ninject;
using Ninject.Extensions.Factory;

namespace Bari.Plugins.FSRepository.Test.Build.Dependencies
{
    [TestFixture]
    public class FSRepositoryReferenceDependenciesTest
    {
        private StandardKernel kernel;
        private Mock<IFileSystemRepositoryAccess> repository;
        private TestFileSystemDirectory depRoot;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
            kernel.Bind<IFSRepositoryFingerprintFactory>().ToFactory();

            repository = new Mock<IFileSystemRepositoryAccess>();

            depRoot = new TestFileSystemDirectory("dep")
                {
                    Files = new[] {"x"}
                };
            repository.Setup(r => r.GetDirectory(Path.Combine("test", "x"))).Returns(depRoot);

        }

        [Test]
        public void CreatesSameFingerprintForSameState()
        {
            var dep1 = new FSRepositoryReferenceDependencies(kernel.Get<IFSRepositoryFingerprintFactory>(), repository.Object, Path.Combine("test", "x"));
            var dep2 = new FSRepositoryReferenceDependencies(kernel.Get<IFSRepositoryFingerprintFactory>(), repository.Object, Path.Combine("test", "x"));
            var fp1 = dep1.Fingerprint;
            var fp2 = dep2.Fingerprint;

            fp1.Should().Be(fp2);
            fp2.Should().Be(fp1);
        }

        [Test]
        public void ChangingTheSourceChangesTheFingerprint()
        {
            var dep1 = new FSRepositoryReferenceDependencies(kernel.Get<IFSRepositoryFingerprintFactory>(), repository.Object, Path.Combine("test", "x"));
            var fp1 = dep1.Fingerprint;

            depRoot.SetFileSize("x", 200);

            var dep2 = new FSRepositoryReferenceDependencies(kernel.Get<IFSRepositoryFingerprintFactory>(), repository.Object, Path.Combine("test", "x"));
            var fp2 = dep2.Fingerprint;

            depRoot.SetFileSize("x", 11); // default
            depRoot.SetDate("x", DateTime.Now);

            var dep3 = new FSRepositoryReferenceDependencies(kernel.Get<IFSRepositoryFingerprintFactory>(), repository.Object, Path.Combine("test", "x"));
            var fp3 = dep3.Fingerprint;

            fp1.Should().NotBe(fp2);
            fp1.Should().NotBe(fp3);
            fp2.Should().NotBe(fp3);
        }

        [Test]
        public void ConvertToProtocolAndBack()
        {
            var dep = new FSRepositoryReferenceDependencies(kernel.Get<IFSRepositoryFingerprintFactory>(), repository.Object, Path.Combine("test", "x"));
            var fp1 = dep.Fingerprint;

            var proto = fp1.Protocol;
            var fp2 = proto.CreateFingerprint();

            fp1.Should().Be(fp2);
        }

        [Test]
        public void SerializeAndReadBack()
        {
            var ser = new BinarySerializer();
            var dep = new FSRepositoryReferenceDependencies(kernel.Get<IFSRepositoryFingerprintFactory>(), repository.Object, Path.Combine("test", "x"));
            var fp1 = dep.Fingerprint;

            byte[] data;
            using (var ms = new MemoryStream())
            {
                fp1.Save(ser, ms);
                data = ms.ToArray();
            }

            FSRepositoryFingerprint fp2;
            using (var ms = new MemoryStream(data))
            {
                fp2 = new FSRepositoryFingerprint(ser, ms);
            }

            fp1.Should().Be(fp2);
        }
    }
}