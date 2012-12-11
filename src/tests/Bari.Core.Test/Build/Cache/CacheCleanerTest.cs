using Bari.Core.Commands.Clean;
using Bari.Core.Test.Helper;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Build.Cache
{
    [TestFixture]
    public class CacheCleanerTest
    {
        [Test]
        public void DeletesCacheDirectory()
        {
            var cdir = new TestFileSystemDirectory("cache");
            var cleaner = new CacheCleaner(cdir);

            cdir.IsDeleted.Should().BeFalse();
            cleaner.Clean();
            cdir.IsDeleted.Should().BeTrue();
        }
    }
}