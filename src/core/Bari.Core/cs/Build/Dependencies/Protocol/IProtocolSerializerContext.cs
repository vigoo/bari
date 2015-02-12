using System;

namespace Bari.Core.Build.Dependencies.Protocol
{
    public interface IProtocolSerializerContext
    {
        void Write(int value);
        void Write(long value);
        void Write(bool value);
        void Write(double value);
        void Write(string value);
        void Write(Uri value);
        void Write(DateTime value);
        void Write(TimeSpan value);
        void Write(IDependencyFingerprintProtocol protocol);
        void WritePrimitive(object value);
    }
}

