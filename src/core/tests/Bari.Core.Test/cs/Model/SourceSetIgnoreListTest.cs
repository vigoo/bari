using Bari.Core.Generic;
using Bari.Core.Model;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Model
{
    [TestFixture]
    public class SourceSetIgnoreListTest
    {
        [Test]
        public void SingleIgnoreExpression()
        {
            var ignoreList = new SourceSetIgnoreList();
            ignoreList.Add(@".+\.tmp$");

            ignoreList.IsIgnored(new SuiteRelativePath(@"a\b\c.dll.tmp")).Should().BeTrue();
            ignoreList.IsIgnored(new SuiteRelativePath(@"x.tmp")).Should().BeTrue();
            ignoreList.IsIgnored(new SuiteRelativePath(@"a\tmp\c.dll")).Should().BeFalse();
            ignoreList.IsIgnored(new SuiteRelativePath(@"tmp")).Should().BeFalse();
        }

        [Test]
        public void MultipleIgnoreExpressions()
        {
            var ignoreList = new SourceSetIgnoreList();
            ignoreList.Add(@".+\.tmp$");
            ignoreList.Add(@".+\.log$");

            ignoreList.IsIgnored(new SuiteRelativePath(@"a\b\c.dll.tmp")).Should().BeTrue();
            ignoreList.IsIgnored(new SuiteRelativePath(@"x.log")).Should().BeTrue();
            ignoreList.IsIgnored(new SuiteRelativePath(@"tmp")).Should().BeFalse();
        }
    }
}