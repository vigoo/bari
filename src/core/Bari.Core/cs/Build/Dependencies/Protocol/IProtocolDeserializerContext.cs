using System;

namespace Bari.Core.Build.Dependencies.Protocol
{
    public interface IProtocolDeserializerContext
    {
        bool ReadBool();
        int ReadInt();
        long ReadLong();
        string ReadString();
        Uri ReadUri();
        double ReadDouble();
        DateTime ReadDateTime();
        TimeSpan ReadTimeSpan();

        IDependencyFingerprintProtocol ReadProtocol();
        object ReadPrimitive();
    }
}

