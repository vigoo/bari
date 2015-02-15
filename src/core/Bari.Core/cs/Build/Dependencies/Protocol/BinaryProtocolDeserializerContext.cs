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
            return DateTime.FromFileTime(reader.ReadInt64());
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

        private Tuple<ProtocolPrimitiveValue, Type> ReadType()
        {
            var primitiveType = (ProtocolPrimitiveValue)ReadInt();

            switch (primitiveType)
            {
                case ProtocolPrimitiveValue.ProtocolBool:
                    return Tuple.Create(primitiveType, typeof (bool));
                case ProtocolPrimitiveValue.ProtocolInt:
                    return Tuple.Create(primitiveType, typeof (int));
                case ProtocolPrimitiveValue.ProtocolLong:
                    return Tuple.Create(primitiveType, typeof (long));
                case ProtocolPrimitiveValue.ProtocolDouble:
                    return Tuple.Create(primitiveType, typeof (double));
                case ProtocolPrimitiveValue.ProtocolString:
                    return Tuple.Create(primitiveType, typeof (string));
                case ProtocolPrimitiveValue.ProtocolUri:
                    return Tuple.Create(primitiveType, typeof (Uri));
                case ProtocolPrimitiveValue.ProtocolDateTime:
                    return Tuple.Create(primitiveType, typeof (DateTime));
                case ProtocolPrimitiveValue.ProtocolTimeSpan:
                    return Tuple.Create(primitiveType, typeof (TimeSpan));
                case ProtocolPrimitiveValue.ProtocolEnum:
                    var typeName = ReadString();
                    return Tuple.Create(primitiveType, Type.GetType(typeName));
                case ProtocolPrimitiveValue.ProtocolRegisteredEnum:
                    var typeId = ReadInt();
                    return Tuple.Create(primitiveType, registry.GetEnumType(typeId));
                case ProtocolPrimitiveValue.ProtocolNullable:
                    var inner = ReadType();
                    return Tuple.Create(primitiveType, typeof (Nullable<>).MakeGenericType(inner.Item2));
                case ProtocolPrimitiveValue.ProtocolArray:
                    var elemType = ReadType();
                    return Tuple.Create(primitiveType, elemType.Item2.MakeArrayType());
                case ProtocolPrimitiveValue.ProtocolDict:
                    var keyType = ReadType();
                    var valType = ReadType();
                    return Tuple.Create(primitiveType,
                        typeof (Dictionary<,>).MakeGenericType(keyType.Item2, valType.Item2));
                case ProtocolPrimitiveValue.ProtocolNull:
                    return Tuple.Create(primitiveType, typeof (object));
                default:
                    throw new ArgumentOutOfRangeException("primitiveType");
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
                    var fullName = ReadString();
                    var value = ReadInt();
                    return Enum.ToObject(Type.GetType(fullName), value);
                }                    
                case ProtocolPrimitiveValue.ProtocolRegisteredEnum:
                {
                    var typeId = ReadInt();
                    var value = ReadInt();
                    return registry.CreateEnum(typeId, value);
                }
                case ProtocolPrimitiveValue.ProtocolArray:
                {
                    var elemType = ReadType();
                    var count = ReadInt();
                    if (count > 0)
                    {
                        var items = new List<object>();
                        for (int i = 0; i < count; i++)
                            items.Add(ReadPrimitive());

                        var result = Array.CreateInstance(elemType.Item2, count);
                        items.ToArray().CopyTo(result, 0);
                        return result;
                    }
                    else
                    {
                        return Array.CreateInstance(elemType.Item2, 0);
                    }
                }
                case ProtocolPrimitiveValue.ProtocolDict:
                {
                    var keyType = ReadType();
                    var valueType = ReadType();
                    var dictType = typeof (Dictionary<,>).MakeGenericType(keyType.Item2, valueType.Item2);
                    var count = ReadInt();

                    var dict = (IDictionary)Activator.CreateInstance(dictType);

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

