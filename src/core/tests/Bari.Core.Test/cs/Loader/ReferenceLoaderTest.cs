using System;
using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using FluentAssertions;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Test.Loader
{
    [TestFixture]
    public class ReferenceLoaderTest
    {
        [Test]
        public void SimpleReferenceUri()
        {
            var loader = new ReferenceLoader();
            var r = loader.LoadReference(new YamlScalarNode("ref://test/x.dll"));

            r.Should().NotBeNull();
            r.Should().HaveCount(1);
            r[0].Uri.Should().Be(new Uri("ref://test/x.dll"));
            r[0].Type.Should().Be(ReferenceType.Build);
        }

        [Test]
        public void ReferenceByUriMappingForm()
        {
            var loader = new ReferenceLoader();
            var r = loader.LoadReference(
                new YamlMappingNode(
                    new[]
                        {
                            new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("uri"), new YamlScalarNode("ref://test/x.dll"))
                        }));

            r.Should().NotBeNull();
            r.Should().HaveCount(1);
            r[0].Uri.Should().Be(new Uri("ref://test/x.dll"));
            r[0].Type.Should().Be(ReferenceType.Build);
        }

        [Test]
        public void BuildReference()
        {
            var loader = new ReferenceLoader();
            var r = loader.LoadReference(
                new YamlMappingNode(
                    new[]
                        {
                            new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("uri"), new YamlScalarNode("ref://test/x.dll")),
                            new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("type"), new YamlScalarNode("Build"))
                        }));

            r.Should().NotBeNull();
            r.Should().HaveCount(1);
            r[0].Uri.Should().Be(new Uri("ref://test/x.dll"));
            r[0].Type.Should().Be(ReferenceType.Build);
        }

        [Test]
        public void RuntimeReference()
        {
            var loader = new ReferenceLoader();
            var r = loader.LoadReference(
                new YamlMappingNode(
                    new[]
                        {
                            new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("uri"), new YamlScalarNode("ref://test/x.dll")),
                            new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("type"), new YamlScalarNode("Runtime"))
                        }));

            r.Should().NotBeNull();
            r.Should().HaveCount(1);
            r[0].Uri.Should().Be(new Uri("ref://test/x.dll"));
            r[0].Type.Should().Be(ReferenceType.Runtime);
        }

        [Test]
        public void AliasIsSpecialCase()
        {
            var loader = new ReferenceLoader();
            var r = loader.LoadReference(new YamlScalarNode("alias://test"));

            r.Should().NotBeNull();
            r.Should().HaveCount(2);
            r[0].Uri.Should().Be(new Uri("alias://test"));
            r[0].Type.Should().Be(ReferenceType.Build);
            r[1].Uri.Should().Be(new Uri("alias://test"));
            r[1].Type.Should().Be(ReferenceType.Runtime);
        }
    }
}