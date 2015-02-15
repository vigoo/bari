using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies.Protocol;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Build.Dependencies.FingerprintSerialization
{
    [TestFixture]
    public class BinaryFingerprintSerializationTest
    {
        private IDependencyFingerprintProtocolRegistry protocolRegistry;

        [SetUp]
        public void Setup()
        {
            protocolRegistry = new DependencyFingerprintProtocolRegistry();
        }

        [Test]
        public void BoolSerialization()
        {
            Test(ctx => ctx.Write(false), ctx => Assert.IsFalse(ctx.ReadBool()));
            Test(ctx => ctx.Write(true), ctx => Assert.IsTrue(ctx.ReadBool()));
        }

        [Test]
        public void IntSerialization()
        {
            Test(ctx => ctx.Write(123), ctx => Assert.AreEqual(123, ctx.ReadInt()));
        }

        [Test]
        public void LongSerialization()
        {
            Test(ctx => ctx.Write(10000000000L), ctx => Assert.AreEqual(10000000000L, ctx.ReadLong()));
        }

        [Test]
        public void DoubleSerialization()
        {
            Test(ctx => ctx.Write(Math.PI), ctx => Assert.AreEqual(Math.PI, ctx.ReadDouble()));
        }

        [Test]
        public void StringSerialization()
        {
            Test(ctx => ctx.Write(""), ctx => Assert.AreEqual("", ctx.ReadString()));
            Test(ctx => ctx.Write("hello world"), ctx => Assert.AreEqual("hello world", ctx.ReadString()));
        }

        [Test]
        public void URISerialization()
        {
            Test(ctx => ctx.Write(new Uri("http://test.value")), ctx => Assert.AreEqual(new Uri("http://test.value"), ctx.ReadUri()));
        }

        [Test]
        public void DateTimeSerialization()
        {
            var now = DateTime.Now;
            Test(ctx => ctx.Write(now), ctx => Assert.AreEqual(now, ctx.ReadDateTime()));
        }

        public class Prot1 : IDependencyFingerprintProtocol
        {
            public string Data { get; set; }

            public IDependencyFingerprint CreateFingerprint()
            {
                throw new NotImplementedException();
            }

            public void Load(IProtocolDeserializerContext context)
            {
                Data = context.ReadString();
            }

            public void Save(IProtocolSerializerContext context)
            {
                context.Write(Data);
            }
        }

        public class Prot2 : IDependencyFingerprintProtocol
        {
            public int Data { get; set; }

            public IDependencyFingerprint CreateFingerprint()
            {
                throw new NotImplementedException();
            }

            public void Load(IProtocolDeserializerContext context)
            {
                Data = context.ReadInt();
            }

            public void Save(IProtocolSerializerContext context)
            {
                context.Write(Data);
            }
        }

        [Test]
        public void ProtocolSerialization()
        {
            protocolRegistry.Register<Prot1>();
            protocolRegistry.Register<Prot2>();

            var p1 = new Prot1 { Data = "Hello World" };
            var p2 = new Prot2 { Data = 123 };

            Test(ctx =>
                {
                    ctx.Write(p2);
                    ctx.Write((IDependencyFingerprintProtocol)null);
                    ctx.Write(p1);
                },
                ctx =>
                {
                    var a = ctx.ReadProtocol();
                    var x = ctx.ReadProtocol();
                    var b = ctx.ReadProtocol();

                    x.Should().BeNull();
                    a.Should().BeOfType<Prot2>();
                    b.Should().BeOfType<Prot1>();

                    ((Prot2)a).Data.Should().Be(p2.Data);
                    ((Prot1)b).Data.Should().Be(p1.Data);
                });
        }

        [Test]
        public void TimeSpanSerialization()
        {
            Test(ctx => ctx.Write(new TimeSpan(1, 2, 3)), ctx => Assert.AreEqual(new TimeSpan(1, 2, 3), ctx.ReadTimeSpan()));
        }

        [Test]
        public void GenericBoolSerialization()
        {
            TestPrimitive(false);
            TestPrimitive(true);
        }

        [Test]
        public void GenericIntSerialization()
        {
            TestPrimitive(123);
        }

        [Test]
        public void GenericLongSerialization()
        {
            TestPrimitive(10000000000L);
        }

        [Test]
        public void GenericDoubleSerialization()
        {
            TestPrimitive(Math.PI);
        }

        [Test]
        public void GenericStringSerialization()
        {
            TestPrimitive("");
            TestPrimitive("hello world");
        }

        [Test]
        public void GenericURISerialization()
        {
            TestPrimitive(new Uri("http://test.value"));
        }

        [Test]
        public void GenericDateTimeSerialization()
        {
            TestPrimitive(DateTime.Now);
        }

        [Test]
        public void GenericTimeSpanSerialization()
        {
            TestPrimitive(new TimeSpan(1, 2, 3));
        }

        [Test]
        public void GenericNullableBoolSerialization()
        {
            TestNullablePrimitive<bool>(false);
            TestNullablePrimitive<bool>(true);
            TestNullablePrimitive<bool>(null);
        }

        [Test]
        public void GenericNullableIntSerialization()
        {
            TestNullablePrimitive<int>(123);
            TestNullablePrimitive<int>(null);
        }

        [Test]
        public void GenericNullableLongSerialization()
        {
            TestNullablePrimitive<long>(10000000000L);
            TestNullablePrimitive<long>(null);
        }

        [Test]
        public void GenericNullableDoubleSerialization()
        {
            TestNullablePrimitive<double>(Math.PI);
            TestNullablePrimitive<double>(null);
        }

        [Test]
        public void GenericNullableDateTimeSerialization()
        {
            TestNullablePrimitive<DateTime>(DateTime.Now);
            TestNullablePrimitive<DateTime>(null);
        }

        [Test]
        public void GenericNullableTimeSpanSerialization()
        {
            TestNullablePrimitive<TimeSpan>(new TimeSpan(1, 2, 3));
            TestNullablePrimitive<TimeSpan>(null);
        }

        [Test]
        public void GenericArraySerialization()
        {
            TestPrimitive(new[] { 1, 2, 3, 4, 5, 6 }, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
            TestPrimitive(new[] { true, true, false }, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
            TestPrimitive(new int[] { }, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
            TestPrimitive(new bool[] { }, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
            TestPrimitive(new[] {new[] {"hello", "world"}, new[] {"x"}, new string[] {}},
                (a, b) =>
                {
                    Assert.AreEqual(a.Length, b.Length);
                    for (int i = 0; i < a.Length; i++)
                    {
                        var x = a[i];
                        var y = b[i];

                        Assert.AreEqual(x.Length, y.Length);
                        for (int j = 0; j < x.Length; j++)
                            Assert.AreEqual(x[j], y[j]);
                    }
                });
        }

        [Test]
        public void GenericDictSerialization()
        {
            var dict = new Dictionary<int, string>
            {
                {10, "hello"},
                {15, null},
                {20, "world"}
            };

            TestPrimitive(dict, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
            TestPrimitive(new Dictionary<string, int>(), (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
            TestPrimitive((IDictionary<int, string>)dict, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
            TestPrimitive((IDictionary<string, int>)(new Dictionary<string, int>()), (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
        }

        public enum Enum1 { V1, V2, V3 }
        public enum Enum2 { V4, V5, V6 }
        public enum Enum3 { V7, V8, V9, V10 }

        [Test]
        public void GenericEnumSerialization()
        {
            TestPrimitive(Enum1.V2);
            TestPrimitive(Enum2.V6);
            TestPrimitive(Enum3.V10);
        }

        [Test]
        public void GenericEnumArraySerialization()
        {
            TestPrimitive(new[] { Enum2.V4, Enum2.V5 }, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
            TestPrimitive(new Enum2[] { }, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
        }

        [Test]
        public void GenericNullableEnumSerialization()
        {
            TestNullablePrimitive<Enum3>(null);
            TestNullablePrimitive<Enum3>(Enum3.V9);
        }

        private void RegisterEnums()
        {
            protocolRegistry.RegisterEnum(i => (Enum1)i);
            protocolRegistry.RegisterEnum(i => (Enum2)i);
            protocolRegistry.RegisterEnum(i => (Enum3)i);
        }

        [Test]
        public void GenericRegisteredEnumSerialization()
        {
            RegisterEnums();

            TestPrimitive(Enum1.V2);
            TestPrimitive(Enum2.V6);
            TestPrimitive(Enum3.V10);
        }

        [Test]
        public void GenericRegisteredEnumArraySerialization()
        {
            RegisterEnums();

            TestPrimitive(new[] { Enum2.V4, Enum2.V5 }, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
            TestPrimitive(new Enum2[] { }, (a, b) => Assert.IsTrue(a.SequenceEqual(b)));
        }

        [Test]
        public void GenericRegisteredNullableEnumSerialization()
        {
            RegisterEnums();

            TestNullablePrimitive<Enum3>(null);
            TestNullablePrimitive<Enum3>(Enum3.V9);
        }

        [Test]
        public void GenericNullSerialization()
        {
            TestPrimitive<string>(null, (a, b) => { Assert.IsNull(a); Assert.IsNull(b); });
        }

        private void Test(Action<IProtocolSerializerContext> serialize, Action<IProtocolDeserializerContext> deserialize)
        {
            using (var stream = new MemoryStream())
            {
                var serializationContext = new BinaryProtocolSerializerContext(stream, protocolRegistry);
                serialize(serializationContext);

                stream.Position = 0;
                var deserializationContext = new BinaryProtocolDeserializerContext(stream, protocolRegistry);
                deserialize(deserializationContext);
            }
        }

        private void TestPrimitive<T>(T value, Action<T, T> customAssert = null)
        {
            Test(ctx => ctx.WritePrimitive(value, typeof(T)),
                ctx =>
                {
                    object v = ctx.ReadPrimitive();

                    if (customAssert == null)
                    {
                        v.Should().BeOfType<T>();
                        v.Should().Be(value);
                    }
                    else
                        customAssert(value, (T)v);
                });
        }

        private void TestNullablePrimitive<T>(T? value)
            where T: struct 
        {
            Test(ctx => ctx.WritePrimitive(value, typeof(T)),
                ctx =>
                {
                    var v = (T?)ctx.ReadPrimitive();
                    v.Should().Be(value);
                });
        }
    }
}
