using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Bari.Core.Build.Dependencies.Protocol
{
    public class BinaryProtocolDeserializerContext : IProtocolDeserializerContext
    {
        private readonly IDependencyFingerprintProtocolRegistry registry;
        private readonly BinaryReader reader;

        public BinaryProtocolDeserializerContext(Stream input, IDependencyFingerprintProtocolRegistry registry)
        {
            this.registry = registry;
            reader = new BinaryReader(input);
        }

        public bool ReadBool()
        {
            return reader.ReadBoolean();
        }

        public int ReadInt()
        {
            return reader.ReadInt32();
        }

        public long ReadLong()
        {
            return reader.ReadInt64();
        }

        public Uri ReadUri()
        {
            return new Uri(ReadString());
        }

        public double ReadDouble()
        {
            return reader.ReadDouble();
        }

        public string ReadString()
        {
            return reader.ReadString();
        }

        public DateTime ReadDateTime()
        {
            return DateTime.FromFileTimeUtc(reader.ReadInt64());
        }

        public TimeSpan ReadTimeSpan()
        {
            return new TimeSpan(ReadLong());
        }

        public IDependencyFingerprintProtocol ReadProtocol()
        {
            bool hasValue = ReadBool();
            if (hasValue)
            {
                int typeId = ReadInt();
                var protocol = registry.Create(typeId);
                protocol.Load(this);
                return protocol;
            }
            else
            {
                return null;
            }
        }

        public object ReadPrimitive()
        {
            var primitiveType = (ProtocolPrimitiveValue)ReadInt();
            switch (primitiveType)
            {
                case ProtocolPrimitiveValue.ProtocolNull:
                    return null;
                case ProtocolPrimitiveValue.ProtocolBool:
                    return ReadBool();
                case ProtocolPrimitiveValue.ProtocolInt:
                    return ReadInt();
                case ProtocolPrimitiveValue.ProtocolLong:
                    return ReadLong();
                case ProtocolPrimitiveValue.ProtocolDouble:
                    return ReadDouble();
                case ProtocolPrimitiveValue.ProtocolString:
                    return ReadString();
                case ProtocolPrimitiveValue.ProtocolUri:
                    return ReadUri();
                case ProtocolPrimitiveValue.ProtocolDateTime:
                    return ReadDateTime();
                case ProtocolPrimitiveValue.ProtocolTimeSpan:
                    return ReadTimeSpan();
                case ProtocolPrimitiveValue.ProtocolNullable:
                    return ReadPrimitive();
                case ProtocolPrimitiveValue.ProtocolEnum:
                {
                    var typeName = ReadString();
                    var value = ReadInt();
                    return Enum.ToObject(Type.GetType(typeName), value);
                }
                case ProtocolPrimitiveValue.ProtocolArray:
                {
                    var count = ReadInt();
                    if (count > 0)
                    {
                        var items = new List<object>();
                        for (int i = 0; i < count; i++)
                            items.Add(ReadPrimitive());

                        var result = Array.CreateInstance(items[0].GetType(), count);
                        items.CopyTo((object[]) result);
                        return result;
                    }
                    else
                    {
                        return new object[0];
                    }
                }
                case ProtocolPrimitiveValue.ProtocolDict:
                {
                    var fullName = ReadString();
                    var count = ReadInt();

                    var dict = (IDictionary)Activator.CreateInstance(Type.GetType(fullName));

                    for (int i = 0; i < count; i++)
                    {
                        var key = ReadPrimitive();
                        var value = ReadPrimitive();
                        dict.Add(key, value);
                    }

                    return dict;
                }
                default:
                    throw new InvalidOperationException("Illegal primitive type: " + primitiveType);
            }
        }
    }
}

