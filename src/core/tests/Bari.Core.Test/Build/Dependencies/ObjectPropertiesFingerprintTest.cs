using System;
using System.IO;
using Bari.Core.Build.Dependencies;
using Bari.Core.Build.Dependencies.Protocol;
using FluentAssertions;
using NUnit.Framework;

namespace Bari.Core.Test.Build.Dependencies
{
    [TestFixture]
    public class ObjectPropertiesFingerprintTest
    {
        [Test]
        public void EmptyPropertySetIsValid()
        {
            var fp1 = new ObjectPropertiesFingerprint(new object(), new string[0]);
            var fp2 = new ObjectPropertiesFingerprint(new object(), new string[0]);

            fp1.Should().Be(fp2);
            fp2.Should().Be(fp1);
        }

        [Test]
        public void FingerprintDoesNotChangeIfPropertiesRemain()
        {
            var obj = new
                {
                    A = "hello",
                    B = 10,
                    C = 12.2,
                    D = new TimeSpan(2, 10, 25)
                };

            var fp1 = new ObjectPropertiesFingerprint(obj, new[] { "A", "B", "C" });
            var fp2 = new ObjectPropertiesFingerprint(obj, new[] { "A", "B", "C" });

            var obj2 = new
            {
                A = "hello",
                B = 10,
                C = 12.2,
                D = new TimeSpan(3, 10, 25)
            };

            var fp3 = new ObjectPropertiesFingerprint(obj2, new[] { "A", "B", "C" });

            fp1.Should().Be(fp2);
            fp2.Should().Be(fp3);
            fp3.Should().Be(fp1);
            fp3.Should().Be(fp2);
        }

        [Test]
        public void ChangingPropertyValueChangesFingerprint()
        {
            var obj = new
            {
                A = "hello",
                B = 10,
                C = 12.2,
                D = new TimeSpan(2, 10, 25)
            };

            var fp1 = new ObjectPropertiesFingerprint(obj, new[] { "A", "B", "D" });

            var obj2 = new
            {
                A = "hello",
                B = 10,
                C = 12.2,
                D = new TimeSpan(3, 10, 25)
            };

            var fp2 = new ObjectPropertiesFingerprint(obj2, new[] { "A", "B", "D" });

            fp1.Should().NotBe(fp2);
        }

        [Test]
        public void ConvertToProtocolAndBack()
        {
            var obj = new
            {
                A = "hello",
                B = 10,
                C = 12.2,
                D = new TimeSpan(2, 10, 25)
            };
            var fp1 = new ObjectPropertiesFingerprint(obj, new[] {"A", "B", "C", "D"});

            var proto = fp1.Protocol;
            var fp2 = proto.CreateFingerprint();

            fp1.Should().Be(fp2);
        }

        [Test]
        public void SerializeAndReadBack()
        {
            var ser = new BinarySerializer();
            var obj = new
            {
                A = "hello",
                B = 10,
                C = 12.2,
                D = new TimeSpan(2, 10, 25)
            };
            var fp1 = new ObjectPropertiesFingerprint(obj, new[] { "A", "B", "C", "D" });

            byte[] data;
            using (var ms = new MemoryStream())
            {
                fp1.Save(ser, ms);
                data = ms.ToArray();
            }

            ObjectPropertiesFingerprint fp2;
            using (var ms = new MemoryStream(data))
            {
                fp2 = new ObjectPropertiesFingerprint(ser, ms);
            }

            fp1.Should().Be(fp2);
        }
    }
}