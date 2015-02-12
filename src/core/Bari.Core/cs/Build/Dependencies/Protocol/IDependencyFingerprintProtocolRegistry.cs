namespace Bari.Core.Build.Dependencies.Protocol
{
    public interface IDependencyFingerprintProtocolRegistry
    {
        void Register<T>()             
            where T: IDependencyFingerprintProtocol;

        int GetId(IDependencyFingerprintProtocol protocol);

        IDependencyFingerprintProtocol Create(int typeId);
    }
}

