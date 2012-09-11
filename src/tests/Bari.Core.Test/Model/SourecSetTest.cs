using Bari.Core.Model;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Model
{
    [TestFixture]
    public class SourecSetTest
    {
        [Test]
        public void SourceSetIsNotCaseSensitive()
        {
            var set = new SourceSet("test");
            set.Add("x/y/z.abc");
            set.Add("x/Y/z.abc");
            set.Add("x/y/Z.abc");
            set.Add("x/y/z.aBc");

            set.Files.Should().HaveCount(1);
            set.Files.Should().HaveElementAt(0, "x/y/z.abc");
        }
    }
}