using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using FluentAssertions;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Test.Loader
{
    [TestFixture]
    public class YamlParserTest
    {
        private YamlParser parser;

        [SetUp]
        public void Setup()
        {
            parser = new YamlParser();
        }

        [Test]
        public void EnumerateNodesOfSimpleMapping()
        {
            var doc = Load(@"---
first:  1
second: 2
third:  3
");

            var nodes = parser.EnumerateNodesOf((YamlMappingNode) doc.RootNode);

            nodes.Should().NotBeNull().And.HaveCount(3);
            nodes.Should().Contain(new[]
                {
                    NodePair(Scalar("first"), Scalar("1")),
                    NodePair(Scalar("second"), Scalar("2")),
                    NodePair(Scalar("third"), Scalar("3")),
                });
        }

        [Test]
        public void EnumerateNodesOfSimpleMappingWithCondition()
        {
            var doc = Load(@"---
first: 1
when X:
  second: 2
  third: 3
fourth: 4
");

            var nodesWithoutX = parser.EnumerateNodesOf((YamlMappingNode) doc.RootNode);
            nodesWithoutX.Should().NotBeNull().And.HaveCount(2);
            nodesWithoutX.Should().Contain(new[]
                {
                    NodePair(Scalar("first"), Scalar("1")),
                    NodePair(Scalar("fourth"), Scalar("4")),
                });

            parser.SetActiveGoal(new Goal("X"));
            var nodesWithX = parser.EnumerateNodesOf((YamlMappingNode) doc.RootNode);

            nodesWithX.Should().NotBeNull().And.HaveCount(4);
            nodesWithX.Should().Contain(new[]
                {
                    NodePair(Scalar("first"), Scalar("1")),
                    NodePair(Scalar("second"), Scalar("2")),
                    NodePair(Scalar("third"), Scalar("3")),
                    NodePair(Scalar("fourth"), Scalar("4")),
                });
        }

        [Test]
        public void EnumerateNodesOfSimpleMappingWithNestedConditions()
        {
            var doc = Load(@"---
first: 1
when X:
  when Y:
    second: 2  
  third: 3
fourth: 4
");

            parser.SetActiveGoal(new Goal("X"));
            var nodesWithX = parser.EnumerateNodesOf((YamlMappingNode)doc.RootNode);

            nodesWithX.Should().NotBeNull().And.HaveCount(3);
            nodesWithX.Should().Contain(new[]
                {
                    NodePair(Scalar("first"), Scalar("1")),
                    NodePair(Scalar("third"), Scalar("3")),
                    NodePair(Scalar("fourth"), Scalar("4")),
                });

            parser.SetActiveGoal(new Goal("Y"));
            var nodesWithY = parser.EnumerateNodesOf((YamlMappingNode)doc.RootNode);
            nodesWithY.Should().NotBeNull().And.HaveCount(2);
            nodesWithY.Should().Contain(new[]
                {
                    NodePair(Scalar("first"), Scalar("1")),
                    NodePair(Scalar("fourth"), Scalar("4")),
                });
        }

        [Test]
        public void EnumerateNodesOfSimpleMappingWithNestedConditionsHierarchicGoals()
        {
            var doc = Load(@"---
first: 1
when X:
  when Y:
    second: 2  
  third: 3
fourth: 4
");

            parser.SetActiveGoal(new Goal("Y", new[] { new Goal("X") }));
            var nodesWithY = parser.EnumerateNodesOf((YamlMappingNode)doc.RootNode);
            nodesWithY.Should().NotBeNull().And.HaveCount(4);
            nodesWithY.Should().Contain(new[]
                {
                    NodePair(Scalar("first"), Scalar("1")),
                    NodePair(Scalar("second"), Scalar("2")),
                    NodePair(Scalar("third"), Scalar("3")),
                    NodePair(Scalar("fourth"), Scalar("4")),
                });
        }

        [Test]
        public void GetOptionalScalarValueUnconditional()
        {
            var doc = Load(@"---
first:  1
second: 2
third:  3
");
            var v = parser.GetOptionalScalarValue(doc.RootNode, "second", "fallback");
            v.Should().Be("2");
        }


        [Test]
        public void GetOptionalNonExistingScalarValueUnconditional()
        {
            var doc = Load(@"---
first:  1
second: 2
third:  3
");
            var v = parser.GetOptionalScalarValue(doc.RootNode, "fourth", "fallback");
            v.Should().Be("fallback");
        }

        [Test]
        public void GetOptionalScalarValueConditional()
        {
            var doc = Load(@"---
first:  1
when X:
  second: 2
third:  3
");
            var v1 = parser.GetOptionalScalarValue(doc.RootNode, "second", "fallback");
            v1.Should().Be("fallback");

            parser.SetActiveGoal(new Goal("X"));

            var v2 = parser.GetOptionalScalarValue(doc.RootNode, "second", "fallback");
            v2.Should().Be("2");
        }

        [Test]
        public void GetOptionalScalarValueNestedConditional()
        {
            var doc = Load(@"---
first:  1
when X:
  when Y:
    second: 2
third:  3
");
            parser.SetActiveGoal(new Goal("Y", new[] { new Goal("X") }));

            var v2 = parser.GetOptionalScalarValue(doc.RootNode, "second", "fallback");
            v2.Should().Be("2");
        }

        [Test]
        public void GetScalarValueUnconditional()
        {
            var doc = Load(@"---
first:  1
second: 2
third:  3
");
            var v = parser.GetScalarValue(doc.RootNode, "second");
            v.Should().Be("2");
        }

        [Test]
        [ExpectedException(typeof (InvalidSpecificationException))]
        public void GetNonExistingScalarValue()
        {
            var doc = Load(@"---
first:  1
second: 2
third:  3
");
            parser.GetScalarValue(doc.RootNode, "fifth");
        }

        [Test]
        [ExpectedException(typeof(InvalidSpecificationException))]
        public void GetScalarValueConditionalNoMatch()
        {
            var doc = Load(@"---
first:  1
when X:
  second: 2
third:  3
");
            parser.GetScalarValue(doc.RootNode, "second");
        }

        [Test]
        public void GetScalarValueConditionalMatch()
        {
            var doc = Load(@"---
first:  1
when X:
  second: 2
third:  3
");
            parser.SetActiveGoal(new Goal("X"));

            var v2 = parser.GetScalarValue(doc.RootNode, "second");
            v2.Should().Be("2");
        }

        [Test]
        public void GetScalarValueNestedConditional()
        {
            var doc = Load(@"---
first:  1
when X:
  when Y:
    second: 2
third:  3
");
            parser.SetActiveGoal(new Goal("Y", new[] { new Goal("X") }));

            var v2 = parser.GetScalarValue(doc.RootNode, "second");
            v2.Should().Be("2");
        }

        [Test]
        public void EnumerateNamedNodesWithNestedConditions()
        {
            var doc = Load(@"---
items:
  - first
  - when X:
     - when Y:
        - name: second
          other: xyz
     - third
  - name: fourth
");

            var nodesWithoutGoal = parser.EnumerateNamedNodesOf(doc.RootNode, "items");

            nodesWithoutGoal.Should()
                            .NotBeNull()
                            .And.HaveCount(2)
                            .And.Contain(pair => pair.Key == "first")
                            .And.Contain(pair => pair.Key == "fourth");

            parser.SetActiveGoal(new Goal("X"));
            var nodesWithX = parser.EnumerateNamedNodesOf(doc.RootNode, "items");

            nodesWithX.Should()
                      .NotBeNull()
                      .And.HaveCount(3)
                      .And.Contain(pair => pair.Key == "first")
                      .And.Contain(pair => pair.Key == "third")
                      .And.Contain(pair => pair.Key == "fourth");

            parser.SetActiveGoal(new Goal("Y"));
            var nodesWithY = parser.EnumerateNamedNodesOf(doc.RootNode, "items");

            nodesWithY.Should()
                .NotBeNull()
                .And.HaveCount(2)
                .And.Contain(pair => pair.Key == "first")
                .And.Contain(pair => pair.Key == "fourth");

            parser.SetActiveGoal(new Goal("Y", new[] { new Goal("X") }));
            var nodesWithXY = parser.EnumerateNamedNodesOf(doc.RootNode, "items");

            nodesWithXY.Should()
                       .NotBeNull()
                       .And.HaveCount(4)
                       .And.Contain(pair => pair.Key == "first")
                       .And.Contain(pair => pair.Key == "second")
                       .And.Contain(pair => pair.Key == "third")
                       .And.Contain(pair => pair.Key == "fourth");
        }

        [Test]
        public void EnumerateNodesOfSimpleMappingWithNegatedCondition()
        {
            var doc = Load(@"---
first: 1
when not X:
  second: 2
  third: 3
fourth: 4
");

            var nodesWithoutX = parser.EnumerateNodesOf((YamlMappingNode) doc.RootNode);
            nodesWithoutX.Should().NotBeNull().And.HaveCount(4);
            nodesWithoutX.Should().Contain(new[]
                {
                    NodePair(Scalar("first"), Scalar("1")),
                    NodePair(Scalar("second"), Scalar("2")),
                    NodePair(Scalar("third"), Scalar("3")),
                    NodePair(Scalar("fourth"), Scalar("4")),
                });                

            parser.SetActiveGoal(new Goal("X"));
            var nodesWithX = parser.EnumerateNodesOf((YamlMappingNode) doc.RootNode);

            nodesWithX.Should().NotBeNull().And.HaveCount(2);
            nodesWithX.Should().Contain(new[]
                {
                    NodePair(Scalar("first"), Scalar("1")),
                    NodePair(Scalar("fourth"), Scalar("4")),
                });
        }

        private KeyValuePair<YamlNode, YamlNode> NodePair(YamlNode k, YamlNode v)
        {
            return new KeyValuePair<YamlNode, YamlNode>(k, v);
        }

        private YamlScalarNode Scalar(string s)
        {
            return new YamlScalarNode(s);
        }

        private YamlDocument Load(string source)
        {
            using (var reader = new StringReader(source))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);

                if (yaml.Documents.Count == 1 &&
                    yaml.Documents[0] != null &&
                    yaml.Documents[0].RootNode != null)
                    return yaml.Documents[0];
                else
                    throw new InvalidSpecificationException(
                        string.Format("The yaml source contains multiple yaml documents!"));
            }
        }
    }
}