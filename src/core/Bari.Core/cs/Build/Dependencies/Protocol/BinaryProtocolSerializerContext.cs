using System;
using System.Collections;
using System.IO;

namespace Bari.Core.Build.Dependencies.Protocol
{
    public class BinaryProtocolSerializerContext : IProtocolSerializerContext
    {
        private readonly IDependencyFingerprintProtocolRegistry registry;
        private readonly BinaryWriter writer;

        public BinaryProtocolSerializerContext(Stream output, IDependencyFingerprintProtocolRegistry registry)
        {
            this.registry = registry;
            writer = new BinaryWriter(output);
        }

        public void Write(int value)
        {
            writer.Write(value);
        }

        public void Write(long value)
        {
            writer.Write(value);
        }

        public void Write(bool value)
        {
            writer.Write(value);
        }

        public void Write(double value)
        {
            writer.Write(value);
        }

        public void Write(string value)
        {
            writer.Write(value);
        }

        public void Write(Uri value)
        {
            writer.Write(value.ToString());
        }

        public void Write(DateTime value)
        {
            writer.Write(value.ToFileTime());
        }

        public void Write(TimeSpan value)
        {
            Write(value.Ticks);
        }

        public void Write(IDependencyFingerprintProtocol protocol)
        {
            if (protocol != null)
            {
                writer.Write(true);
                writer.Write(registry.GetId(protocol));
                protocol.Save(this);
            }
            else
            {
                writer.Write(false);
            }
        }
        public void WritePrimitive(object value, Type valueType)
        {
            if (value == null)
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolNull);
            }
            else if (valueType == typeof(int))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolInt);
                Write((int)value);
            }
            else if (valueType == typeof(bool))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolBool);
                Write((bool)value);
            }
            else if (valueType == typeof(string))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolString);
                Write((string)value);
            }
            else if (valueType == typeof(long))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolLong);
                Write((long)value);
            }
            else if (valueType == typeof(double))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolDouble);
                Write((double)value);
            }
            else if (valueType == typeof(DateTime))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolDateTime);
                Write((DateTime)value);
            }
            else if (valueType == typeof(TimeSpan))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolTimeSpan);
                Write((TimeSpan)value);
            }
            else if (valueType == typeof (Uri))
            {
                writer.Write((int) ProtocolPrimitiveValue.ProtocolUri);
                Write((Uri) value);
            }
            else if (typeof(IDictionary).IsAssignableFrom(valueType))
            {
                writer.Write((int) ProtocolPrimitiveValue.ProtocolDict);
                var dict = (IDictionary) value;
                var keyType = valueType.GetGenericArguments()[0];
                var valType = valueType.GetGenericArguments()[1];
                WriteType(keyType);
                WriteType(valType);
                writer.Write(dict.Count);
                foreach (DictionaryEntry pair in dict)
                {
                    WritePrimitive(pair.Key, keyType);
                    WritePrimitive(pair.Value, valType);
                }
            }
            else
            {
                var nt = Nullable.GetUnderlyingType(valueType);
                if (nt != null)
                {
                    writer.Write((int)ProtocolPrimitiveValue.ProtocolNullable);
                    WritePrimitive(value, nt);
                }
                else if (valueType.IsArray)
                {
                    var arr = (Array) value;
                    writer.Write((int)ProtocolPrimitiveValue.ProtocolArray);
                    WriteType(valueType.GetElementType());                    

                    writer.Write(arr.Length);
                    foreach (object item in arr)
                        WritePrimitive(item, valueType.GetElementType());
                }
                else if (valueType.IsEnum)
                {
                    WriteEnum(value, valueType);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported primitive type: " + valueType.FullName);
                }
            }
        }

        private void WriteType(Type type)
        {
            if (type == typeof(int))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolInt);
            }
            else if (type == typeof(bool))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolBool);
            }
            else if (type == typeof(string))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolString);
            }
            else if (type == typeof(long))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolLong);
            }
            else if (type == typeof(double))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolDouble);
            }
            else if (type == typeof(DateTime))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolDateTime);
            }
            else if (type == typeof(TimeSpan))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolTimeSpan);
            }
            else if (type == typeof(Uri))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolUri);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolDict);
                WriteType(type.GetGenericArguments()[0]);
                WriteType(type.GetGenericArguments()[1]);
            }
            else
            {
                var nt = Nullable.GetUnderlyingType(type);
                if (nt != null)
                {
                    writer.Write((int)ProtocolPrimitiveValue.ProtocolNullable);
                    WriteType(nt);
                }
                else if (type.IsArray)
                {
                    writer.Write((int)ProtocolPrimitiveValue.ProtocolArray);
                    WriteType(type.GetElementType());
                }
                else if (type.IsEnum)
                {
                    var enumTypeId = registry.GetEnumId(type);
                    if (enumTypeId.HasValue)
                    {
                        writer.Write((int)ProtocolPrimitiveValue.ProtocolRegisteredEnum);
                        writer.Write(enumTypeId.Value);
                    }
                    else
                    {
                        writer.Write((int)ProtocolPrimitiveValue.ProtocolEnum);
                        writer.Write(type.AssemblyQualifiedName);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Unsupported primitive type: " + type.FullName);
                }
            }
        }

        private void WriteEnum(object value, Type valueType)
        {
            var enumTypeId = registry.GetEnumId(valueType);
            if (enumTypeId.HasValue)
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolRegisteredEnum);
                writer.Write(enumTypeId.Value);
                writer.Write((int)value);
            }
            else
            {
                writer.Write((int) ProtocolPrimitiveValue.ProtocolEnum);
                writer.Write(valueType.AssemblyQualifiedName);
                writer.Write((int) value);
            }
        }
    }
}

