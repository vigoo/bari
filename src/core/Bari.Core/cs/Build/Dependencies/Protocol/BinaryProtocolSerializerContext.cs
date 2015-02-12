using System;
using System.Collections;
using System.Collections.Generic;
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
            writer.Write(value.ToFileTimeUtc());
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

        public void WritePrimitive(object value)
        {
            WritePrimitive(value, value != null ? value.GetType() : null);
        }

        private void WritePrimitive(object value, Type valueType)
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
                writer.Write(valueType.FullName);
                writer.Write(dict.Count);
                foreach (DictionaryEntry pair in dict)
                {
                    WritePrimitive(pair.Key);
                    WritePrimitive(pair.Value);
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
                    writer.Write(arr.Length);
                    foreach (object item in arr)
                        WritePrimitive(item, valueType.GetElementType());
                }
                else if (valueType.IsEnum)
                {
                    writer.Write((int)ProtocolPrimitiveValue.ProtocolEnum);
                    writer.Write(valueType.FullName);
                    writer.Write((int)value);                    
                }
                else
                {
                    throw new InvalidOperationException("Unsupported primitive type: " + valueType.FullName);
                }
            }
        }
    }
}

