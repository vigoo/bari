using Bari.Core.Generic;
using Bari.Core.Model;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Model
{
    [TestFixture]
    public class SourceSetTest
    {
        [Test]
        public void SourceSetIsNotCaseSensitive()
        {
            var set = new SourceSet("test");
            set.Add(new SuiteRelativePath("x/y/z.abc"));
            set.Add(new SuiteRelativePath("x/Y/z.abc"));
            set.Add(new SuiteRelativePath("x/y/Z.abc"));
            set.Add(new SuiteRelativePath("x/y/z.aBc"));

            set.Files.Should().HaveCount(1);
            set.Files.Should().HaveElementAt(0, new SuiteRelativePath("x/y/z.abc"));
        }
    }
}