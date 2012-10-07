using System.IO;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Test.Helper;
using FluentAssertions;
using NUnit.Framework;
using Ninject;

namespace Bari.Core.Test.Build.Dependencies
{
    [TestFixture]
    public class SourceSetDependenciesTest
    {
        private IKernel kernel;
        private TempDirectory tmp;
        private IFileSystemDirectory rootDir;
        private SourceSet sourceSet;

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

            sourceSet = new SourceSet("test");
            sourceSet.Add(new SuiteRelativePath("file1"));
            sourceSet.Add(new SuiteRelativePath("file2"));

            kernel.Bind<IFileSystemDirectory>().ToConstant(rootDir).WhenTargetHas<SuiteRootAttribute>();
        }

        [TearDown]
        public void TearDown()
        {
            tmp.Dispose();
        }

        [Test]
        public void CreatesSameFingerprintForSameState()
        {
            var dep = new SourceSetDependencies(kernel, sourceSet);
            var fp1 = dep.CreateFingerprint();
            var fp2 = dep.CreateFingerprint();

            fp1.Should().Be(fp2);
            fp2.Should().Be(fp1);
        }

        [Test]
        public void RemovingFileChangesFingerprint()
        {
            var dep = new SourceSetDependencies(kernel, sourceSet);
            var fp1 = dep.CreateFingerprint();

            File.Delete(Path.Combine(tmp, "file1"));
            sourceSet.Remove(new SuiteRelativePath("file1"));

            var fp2 = dep.CreateFingerprint();

            fp1.Should().NotBe(fp2);
        }

        [Test]
        public void AddingFileChangesFingerprint()
        {
            var dep = new SourceSetDependencies(kernel, sourceSet);
            var fp1 = dep.CreateFingerprint();

            using (var writer = rootDir.CreateTextFile("file3"))
                writer.WriteLine("Contents of file 3");
            sourceSet.Add(new SuiteRelativePath("file3"));

            var fp2 = dep.CreateFingerprint();

            fp1.Should().NotBe(fp2);
        }

        [Test]
        public void ModifyingFileChangesFingerprint()
        {
            var dep = new SourceSetDependencies(kernel, sourceSet);
            var fp1 = dep.CreateFingerprint();

            using (var writer = File.CreateText(Path.Combine(tmp, "file2")))
            {
                writer.WriteLine("Modified contents");
                writer.Flush();
            }

            var fp2 = dep.CreateFingerprint();

            fp1.Should().NotBe(fp2);
        }

        [Test]
        public void AddingFileToSubdirectoryChangesFingerprint()
        {
            var dep = new SourceSetDependencies(kernel, sourceSet);
            var fp1 = dep.CreateFingerprint();

            Directory.CreateDirectory(Path.Combine(tmp, "subdir"));
            using (var writer = rootDir.GetChildDirectory("subdir").CreateTextFile("file3"))
                writer.WriteLine("Contents of file 3");
            sourceSet.Add(new SuiteRelativePath("subdir\\file3"));

            var fp2 = dep.CreateFingerprint();

            fp1.Should().NotBe(fp2);
        }

        [Test]
        public void ModifyingFileInSubdirectoryChangesFingerprint()
        {
            var dep = new SourceSetDependencies(kernel, sourceSet);
            
            Directory.CreateDirectory(Path.Combine(tmp, "subdir"));
            using (var writer = rootDir.GetChildDirectory("subdir").CreateTextFile("file3"))
                writer.WriteLine("Contents of file 3");
            sourceSet.Add(new SuiteRelativePath("subdir\\file3"));

            var fp1 = dep.CreateFingerprint();

            using (var writer = rootDir.GetChildDirectory("subdir").CreateTextFile("file3"))
                writer.WriteLine("Modified contents of file 3");

            var fp2 = dep.CreateFingerprint();

            fp1.Should().NotBe(fp2);
        }

        [Test]
        public void ConvertToProtocolAndBack()
        {
            var dep = new SourceSetDependencies(kernel, sourceSet);
            var fp1 = dep.CreateFingerprint();

            var proto = fp1.Protocol;
            var fp2 = proto.CreateFingerprint();

            fp1.Should().Be(fp2);
        }

        [Test]
        public void SerializeAndReadBack()
        {
            var dep = new SourceSetDependencies(kernel, sourceSet);
            var fp1 = dep.CreateFingerprint();

            byte[] data;
            using (var ms = new MemoryStream())
            {
                fp1.Save(ms);
                data = ms.ToArray();
            }

            SourceSetFingerprint fp2;
            using (var ms = new MemoryStream(data))
            {
                fp2 = new SourceSetFingerprint(ms);
            }            

            fp1.Should().Be(fp2);
        }
    }
}