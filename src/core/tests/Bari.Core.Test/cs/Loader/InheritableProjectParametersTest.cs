using Bari.Core.Model;
using Bari.Core.Model.Parameters;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;
using System;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Loader
{
    [TestFixture]
    public class InheritableProjectParametersTest
    {
        private class Defs : ProjectParametersPropertyDefs<Props>
        {
            public Defs()
            {
                Define<string>("simpleStringProperty");
                Define<string[]>("simpleStringArray");
                Define<string[]>("mergedInheritedStringArray", mergeWithInherited: true);
            }

            public override Props CreateDefault(Suite suite, Props parent)
            {
                return new Props(parent);
            }
        }

        private class Props: InheritableProjectParameters<Props, Defs>
        {
            public Props(Props parent): base(parent)
            {                
            }

            public string SimpleStringProperty 
            {                
                get { return Get<string>("simpleStringProperty"); }
                set { Set<string>("simpleStringProperty", value); }
            }

            public string[] SimpleStringArray 
            {                
                get { return Get<string[]>("simpleStringArray"); }
                set { Set<string[]>("simpleStringArray", value); }
            }

            public string[] MergedInheritedStringArray 
            {                
                get { return Get<string[]>("mergedInheritedStringArray"); }
                set { Set<string[]>("mergedInheritedStringArray", value); }
            }
        }

        [Test]
        public void GetSimpleStringValue()
        {
            var props = new Props(null);
            props.SimpleStringProperty = "hello world";
            props.SimpleStringProperty.Should().Be("hello world");
        }

        [Test]
        public void GetSimpleStringArrayValue()
        {
            var props = new Props(null);
            props.SimpleStringArray = new[] { "hello", "world" };
            props.SimpleStringArray.Should().HaveCount(2);
            props.SimpleStringArray[0].Should().Be("hello");
            props.SimpleStringArray[1].Should().Be("world");
        }

        [Test]
        public void GetMergedStringArrayValue()
        {
            var props = new Props(null);
            props.MergedInheritedStringArray = new[] { "hello", "world" };
            props.MergedInheritedStringArray.Should().HaveCount(2);
            props.MergedInheritedStringArray[0].Should().Be("hello");
            props.MergedInheritedStringArray[1].Should().Be("world");
        }

        [Test]
        public void GetSimpleStringValueFromParent()
        {
            var parentProps = new Props(null);
            var props = new Props(parentProps);
            parentProps.SimpleStringProperty = "hello world";
            props.SimpleStringProperty.Should().Be("hello world");
        }

        [Test]
        public void GetSimpleStringArrayValueFromParent()
        {
            var parentProps = new Props(null);
            var props = new Props(parentProps);
            parentProps.SimpleStringArray = new[] { "hello", "world" };
            props.SimpleStringArray.Should().HaveCount(2);
            props.SimpleStringArray[0].Should().Be("hello");
            props.SimpleStringArray[1].Should().Be("world");
        }

        [Test]
        public void GetMergedStringArrayValueFromParent()
        {
            var parentProps = new Props(null);
            var props = new Props(parentProps);
            parentProps.MergedInheritedStringArray = new[] { "hello", "world" };
            props.MergedInheritedStringArray.Should().HaveCount(2);
            props.MergedInheritedStringArray[0].Should().Be("hello");
            props.MergedInheritedStringArray[1].Should().Be("world");
        }

        [Test]
        public void GetSimpleStringArrayValueWithParent()
        {
            var parentProps = new Props(null);
            var props = new Props(parentProps);
            parentProps.SimpleStringArray = new[] { "!!!" };
            props.SimpleStringArray = new[] { "hello", "world" };
            props.SimpleStringArray.Should().HaveCount(2);
            props.SimpleStringArray[0].Should().Be("hello");
            props.SimpleStringArray[1].Should().Be("world");
        }

        [Test]
        public void GetMergedStringArrayValueWithParent()
        {
            var parentProps = new Props(null);
            var props = new Props(parentProps);
            parentProps.MergedInheritedStringArray = new[] { "!!!" };
            props.MergedInheritedStringArray = new[] { "hello", "world" };
            props.MergedInheritedStringArray.Should().HaveCount(3);
            props.MergedInheritedStringArray.Should().Contain("hello");
            props.MergedInheritedStringArray.Should().Contain("world");
            props.MergedInheritedStringArray.Should().Contain("!!!");
        }
    }
}