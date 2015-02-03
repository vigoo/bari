using System;
using Bari.Core.Generic;
using Bari.Plugins.FSRepository.Model;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Plugins.FSRepository.Test.Model
{
    [TestFixture]
    public class UriParserTest
    {
        [Test]
        public void ShortForm()
        {
            var uri = new Uri("fsrepo://dependency/version/something.dll");
            var context = new UriBasedPatternResolutionContext(new DefaultEnvironmentVariableContext(), uri);

            context.RepositoryName.Should().BeNull();
            context.DependencyName.Should().Be("dependency");
            context.Version.Should().Be("version");
            context.FileName.Should().Be("something");
            context.Extension.Should().Be("dll");
        }

        [Test]
        public void ShortFormNoVersion()
        {
            var uri = new Uri("fsrepo://dependency/something.dll");
            var context = new UriBasedPatternResolutionContext(new DefaultEnvironmentVariableContext(), uri);

            context.RepositoryName.Should().BeNull();
            context.DependencyName.Should().Be("dependency");
            context.Version.Should().BeNull();
            context.FileName.Should().Be("something");
            context.Extension.Should().Be("dll");
        }

        [Test]
        public void LongForm()
        {
            var uri = new Uri("fsrepo://reponame/dependency/version/something.dll");
            var context = new UriBasedPatternResolutionContext(new DefaultEnvironmentVariableContext(), uri);

            context.RepositoryName.Should().Be("reponame");
            context.DependencyName.Should().Be("dependency");
            context.Version.Should().Be("version");
            context.FileName.Should().Be("something");
            context.Extension.Should().Be("dll");
        }
    }
}