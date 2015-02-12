using System;
using System.IO;

namespace Bari.Core.Build.Dependencies.Protocol
{
    public class BinaryProtocolSerializerContext: IProtocolSerializerContext
    {
        private readonly IDependencyFingerprintProtocolRegistry registry;
        private readonly BinaryWriter writer;

        public BinaryProtocolSerializerContext (Stream output, IDependencyFingerprintProtocolRegistry registry)
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
            } else
            {
                writer.Write(false);
            }
        }

        public void WritePrimitive(object value)
        {
            WritePrimitive(value, value.GetType());
        }

        private void WritePrimitive(object value, Type valueType)
        {
            if (valueType == typeof(int))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolInt);
                Write((int)value);
            } else if (valueType == typeof(bool))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolBool);
                Write((bool)value);
            } else if (valueType == typeof(string))
            {            
                writer.Write((int)ProtocolPrimitiveValue.ProtocolString);
                Write((string)value);
            } else if (valueType == typeof(long))
            {              
                writer.Write((int)ProtocolPrimitiveValue.ProtocolLong);
                Write((long)value);
            } else if (valueType == typeof(double))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolDouble);
                Write((double)value);
            } else if (valueType == typeof(DateTime))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolDateTime);
                Write((DateTime)value);
            } else if (valueType == typeof(TimeSpan))
            {
                writer.Write((int)ProtocolPrimitiveValue.ProtocolTimeSpan);
                Write((TimeSpan)value);
            } else
            {
                var nt = Nullable.GetUnderlyingType(valueType);
                if (nt != null)
                {
                    writer.Write((int)ProtocolPrimitiveValue.ProtocolNullable);
                    if (value == null)
                    {
                        Write(false);
                    } else
                    {
                        Write(true);
                        WritePrimitive(value, nt);
                    }
                } else if (valueType.IsEnum)
                {
                    writer.Write((int)ProtocolPrimitiveValue.ProtocolEnum);
                    writer.Write(valueType.FullName);
                    writer.Write((int)value);
                } else
                {
                    throw new InvalidOperationException("Unsupported primitive type: " + valueType.FullName);
                }
            }
        }
    }
}

