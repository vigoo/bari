using System;
using System.Collections.Generic;
using System.IO;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.Test.Helper;
using Bari.Core.UI;
using FluentAssertions;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Test.Loader
{
    [TestFixture]
    public class YamlProjectParamtersLoaderBaseTest
    {
        class TestBlock : IProjectParameters
        {           
            public string Prop1 { get; set; }
        }

        class TestLoader : YamlProjectParametersLoaderBase<TestBlock>
        {
            public TestLoader(IUserOutput output) : base(output)
            {
            }

            protected override string BlockName
            {
                get { return "test"; }
            }

            protected override TestBlock CreateNewParameters(Suite suite)
            {
                return new TestBlock();
            }

            protected override Dictionary<string, Action> GetActions(TestBlock target, YamlNode value, YamlParser parser)
            {
                return new Dictionary<string, Action>
                {
                    {"prop1", () => target.Prop1 = ParseString(value)}
                };
            }
        }

        private TestUserOutput output;
        private TestLoader loader;
        private Suite suite;

        [SetUp]
        public void SetUp()
        {
            output = new TestUserOutput();
            loader = new TestLoader(output);
            suite = new Suite(new TestFileSystemDirectory("root"));
        }

        [Test]
        public void MappingNodeLoaded()
        {
            const string yaml = @"---
test:
  prop1: test-value
";
            var node = Load(yaml);

            var result = (TestBlock)loader.Load(suite, "test", node, new YamlParser());
            result.Prop1.Should().Be("test-value");

            output.Warnings.Should().BeEmpty();
        }

        [Test]
        public void SequenceEmitsWarning()
        {
            const string yaml = @"---
test:
  - prop1: test-value
";
            var node = Load(yaml);

            var result = (TestBlock)loader.Load(suite, "test", node, new YamlParser());
            result.Prop1.Should().BeNull();

            output.Warnings.Should().NotBeEmpty();
        }

        private YamlNode Load(string yaml)
        {
            var stream = new YamlStream();
            stream.Load(new StringReader(yaml));
            return ((YamlMappingNode) stream.Documents[0].RootNode).Children[new YamlScalarNode("test")];
        }
    }
}