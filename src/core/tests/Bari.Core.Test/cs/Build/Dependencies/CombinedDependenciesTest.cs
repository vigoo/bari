using System;
using System.Collections.Generic;
using System.IO;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Build.Dependencies
{
    [TestFixture]
    public class CombinedDependenciesTest
    {
        private IKernel kernel;
        private TempDirectory tmp;
        private LocalFileSystemDirectory rootDir;
        private SourceSet sourceSet1;
        private SourceSet sourceSet2;
        private ISourceSetFingerprintFactory fingerprintFactory;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();

            tmp = new TempDirectory();
            rootDir = new LocalFileSystemDirectory(tmp);
            using (var writer = rootDir.CreateTextFile("file1"))
                writer.WriteLine("Contents of file 1");
            using (var writer = rootDir.CreateTextFile("file2"))
                writer.WriteLine("Contents of file 2");
            using (var writer = rootDir.CreateTextFile("file3"))
                writer.WriteLine("Contents of file 3");

            sourceSet1 = new SourceSet("test1");
            sourceSet1.Add(new SuiteRelativePath("file1"));
            sourceSet1.Add(new SuiteRelativePath("file2"));

            sourceSet2 = new SourceSet("test2");
            sourceSet2.Add(new SuiteRelativePath("file1"));
            sourceSet2.Add(new SuiteRelativePath("file3"));

            kernel.Bind<IFileSystemDirectory>().ToConstant(rootDir).WhenTargetHas<SuiteRootAttribute>();

            var factoryMock = new Mock<ISourceSetFingerprintFactory>();
            factoryMock.Setup(
                f =>
                f.CreateSourceSetFingerprint(It.IsAny<IEnumerable<SuiteRelativePath>>(), It.IsAny<Func<string, bool>>(), It.IsAny<bool>()))
                       .Returns<IEnumerable<SuiteRelativePath>, Func<string, bool>, bool>(
                            (files, exclusions, fullDependency) => new SourceSetFingerprint(rootDir, files, exclusions, fullDependency));
            fingerprintFactory = factoryMock.Object;
        }

        [TearDown]
        public void TearDown()
        {
            tmp.Dispose();
            kernel.Dispose();
        }

        [Test]
        public void CreatesSameFingerprintForSameState()
        {
            var combined1 = CreateDependencyObject();
            var combined2 = CreateDependencyObject();

            var fp1 = combined1.Fingerprint;
            var fp2 = combined2.Fingerprint;

            fp1.Should().Be(fp2);
            fp2.Should().Be(fp1);
        }

        private MultipleDependencies CreateDependencyObject()
        {
            return new MultipleDependencies(new IDependencies[]
                {
                    new SourceSetDependencies(fingerprintFactory, sourceSet1),
                    new NoDependencies(),
                    new SourceSetDependencies(fingerprintFactory, sourceSet2)
                });
        }

        [Test]
        public void ConvertToProtocolAndBack()
        {
            var dep = CreateDependencyObject();
            var fp1 = dep.Fingerprint;

            var proto = fp1.Protocol;
            var fp2 = proto.CreateFingerprint();

            fp1.Should().Be(fp2);
        }

        [Test]
        public void SerializeAndReadBack()
        {
            var registry = new DependencyFingerprintProtocolRegistry();
            registry.Register<CombinedFingerprintProtocol>();
            registry.Register<SourceSetFingerprintProtocol>();

            var ser = new BinarySerializer(registry);
            var dep = CreateDependencyObject();
            var fp1 = dep.Fingerprint;

            byte[] data;
            using (var ms = new MemoryStream())
            {
                fp1.Save(ser, ms);
                data = ms.ToArray();
            }

            CombinedFingerprint fp2;
            using (var ms = new MemoryStream(data))
            {
                fp2 = new CombinedFingerprint(ser, ms);
            }

            fp1.Should().Be(fp2);
        }
    }
}