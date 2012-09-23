using System.IO;
using Bari.Core.Generic;
using Bari.Core.Test.Helper;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Generic
{
    [TestFixture]
    public class LocalFileSystemDirectoryTest
    {       
        [Test]
        public void EmptyDirectoryLooksEmpty()
        {
            using (var tmp = new TempDirectory())
            {
                var dir = new LocalFileSystemDirectory(tmp);

                dir.ChildDirectories.Should().BeEmpty();
                dir.Files.Should().BeEmpty();
            }
        }

        [Test]
        public void FilesAreEnumerated()
        {
            using (var tmp = new TempDirectory())
            {
                using (File.Create(Path.Combine(tmp, "test1.txt"))) {}                
                using (File.Create(Path.Combine(tmp, "test2.bin"))) {}

                var dir = new LocalFileSystemDirectory(tmp);
                dir.ChildDirectories.Should().BeEmpty();
                dir.Files.Should().HaveCount(2);
                dir.Files.Should().Contain("test1.txt");
                dir.Files.Should().Contain("test2.bin");
            }
        }

        [Test]
        public void FilesAddedLaterAreEnumerated()
        {
            using (var tmp = new TempDirectory())
            {
                using (File.Create(Path.Combine(tmp, "test1.txt"))) {}
                var dir = new LocalFileSystemDirectory(tmp);
                using (File.Create(Path.Combine(tmp, "test2.bin"))) {}

                dir.ChildDirectories.Should().BeEmpty();
                dir.Files.Should().HaveCount(2);
                dir.Files.Should().Contain("test1.txt");
                dir.Files.Should().Contain("test2.bin");
            }
        }

        [Test]
        public void DirectoriesAreEnumerated()
        {
            using (var tmp = new TempDirectory())
            {
                Directory.CreateDirectory(Path.Combine(tmp, "dir1"));
                Directory.CreateDirectory(Path.Combine(tmp, "dir2"));

                var dir = new LocalFileSystemDirectory(tmp);
                dir.ChildDirectories.Should().HaveCount(2);
                dir.ChildDirectories.Should().Contain("dir1");
                dir.ChildDirectories.Should().Contain("dir2");
                dir.Files.Should().BeEmpty();
            }
        }

        [Test]
        public void DirectoriesAddedLaterAreEnumerated()
        {
            using (var tmp = new TempDirectory())
            {
                Directory.CreateDirectory(Path.Combine(tmp, "dir1"));
                var dir = new LocalFileSystemDirectory(tmp);
                Directory.CreateDirectory(Path.Combine(tmp, "dir2"));
                
                dir.ChildDirectories.Should().HaveCount(2);
                dir.ChildDirectories.Should().Contain("dir1");
                dir.ChildDirectories.Should().Contain("dir2");
                dir.Files.Should().BeEmpty();
            }
        }

        [Test]
        public void GetChildDirectoryWorks()
        {
            using (var tmp = new TempDirectory())
            {
                Directory.CreateDirectory(Path.Combine(tmp, "dir1"));
                var dir = new LocalFileSystemDirectory(tmp);
                var subdir = dir.GetChildDirectory("dir1");

                subdir.Should().NotBeNull();                
            }
        }

        [Test]
        public void GetChildDirectoryForNonExistingDirectoryReturnsNull()
        {
            using (var tmp = new TempDirectory())
            {
                var dir = new LocalFileSystemDirectory(tmp);
                var subdir = dir.GetChildDirectory("dir1");

                subdir.Should().BeNull();
            }
        }

        [Test]
        public void GetRelativePathWorks()
        {
            using (var tmp = new TempDirectory())
            {
                Directory.CreateDirectory(Path.Combine(tmp, "dir1"));                
                Directory.CreateDirectory(Path.Combine(tmp, "dir1", "dir2"));

                var dir = new LocalFileSystemDirectory(tmp);
                var dir1 = dir.GetChildDirectory("dir1");
                var dir2 = dir1.GetChildDirectory("dir2");

                dir1.Should().NotBeNull();
                dir2.Should().NotBeNull();

                dir.GetRelativePath(dir1).Should().Be("dir1");
                dir.GetRelativePath(dir2).Should().Be("dir1\\dir2");
            }
        }

        [Test]
        public void CreateDirectoryWorks()
        {
            using (var tmp = new TempDirectory())
            {
                var dir = new LocalFileSystemDirectory(tmp);
                dir.CreateDirectory("testdir");

                dir.ChildDirectories.Should().Contain("testdir");
                Directory.Exists(Path.Combine(tmp, "testdir")).Should().BeTrue();
            }
        }

        [Test]
        public void CreateDirectoryForExistingDirectoryIsNotAnError()
        {
            using (var tmp = new TempDirectory())
            {
                Directory.CreateDirectory(Path.Combine(tmp, "testdir"));

                var dir = new LocalFileSystemDirectory(tmp);
                dir.ChildDirectories.Should().Contain("testdir");

                dir.CreateDirectory("testdir");

                dir.ChildDirectories.Should().Contain("testdir");
                Directory.Exists(Path.Combine(tmp, "testdir")).Should().BeTrue();
            }
        }

        [Test]
        public void CreateTextFileWorks()
        {
            using (var tmp = new TempDirectory())
            {
                var dir = new LocalFileSystemDirectory(tmp);
                using (var writer = dir.CreateTextFile("test.txt"))
                    writer.WriteLine("Hello world");

                dir.Files.Should().Contain("test.txt");
                File.Exists(Path.Combine(tmp, "test.txt")).Should().BeTrue();
            }        
        }

        [Test]
        public void GetLastModifiedDateReturnsLastModifiedDateInUTC()
        {
            using (var tmp = new TempDirectory())
            {
                var dir = new LocalFileSystemDirectory(tmp);
                using (var writer = dir.CreateTextFile("test.txt"))
                    writer.WriteLine("Hello world");

                var lastWriteTime = File.GetLastWriteTimeUtc(Path.Combine(tmp, "test.txt"));
                var lastModifiedDate = dir.GetLastModifiedDate("test.txt");

                lastModifiedDate.Should().Be(lastWriteTime);
            }        
        }
    }
}