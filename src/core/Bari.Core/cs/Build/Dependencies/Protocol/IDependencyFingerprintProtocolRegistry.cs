using System;

namespace Bari.Core.Build.Dependencies.Protocol
{
    public interface IDependencyFingerprintProtocolRegistry
    {
        void Register<T>()             
            where T: IDependencyFingerprintProtocol;

        int GetId(IDependencyFingerprintProtocol protocol);

        IDependencyFingerprintProtocol Create(int typeId);
        
        void RegisterEnum<T>(Func<int, T> decode)
            where T: struct;

        int? GetEnumId(object value);

        object CreateEnum(int typeId, int value);
        Type GetEnumType(int typeId);
    }
}

