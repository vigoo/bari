using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bari.Core.Build.Dependencies.Protocol
{
    public class DependencyFingerprintProtocolRegistry: IDependencyFingerprintProtocolRegistry
    {
        private class ModuleTypeRegistry
        {
            private short lastId = 0;
            private readonly IDictionary<short, Type> typesPerId = new Dictionary<short, Type>();
            private readonly IDictionary<Type, short> idsPerType = new Dictionary<Type, short>();

            public void Add(Type t)
            {
                short id = lastId++;
                typesPerId.Add(id, t);
                idsPerType.Add(t, id);
            }

            public Type this [short id]
            {
                get
                {
                    return typesPerId[id];
                }
            }

            public short this [Type t]
            {
                get
                {
                    return idsPerType[t];
                }
            }
        }

        private readonly IDictionary<Assembly, short> assemblyIds = new Dictionary<Assembly, short>();
        private readonly IDictionary<short, ModuleTypeRegistry> moduleRegistries = new Dictionary<short, ModuleTypeRegistry>();
        private short lastAssemblyId = 0;

        public void Register<T>() where T : IDependencyFingerprintProtocol
        {
            var t = typeof(T);
            var assembly = t.Assembly;
            var registry = GetOrCreateRegistry(assembly);
            registry.Add(t);
        }

        public int GetId(IDependencyFingerprintProtocol protocol)
        {
            var t = protocol.GetType();
            var assembly = t.Assembly;

            var assemblyId = assemblyIds[assembly];
            var moduleTypeId = moduleRegistries[assemblyId][t];

            return ((int)(assemblyId) << 16) | ((int)moduleTypeId);
        }

        public IDependencyFingerprintProtocol Create(int typeId)
        {
            short assemblyId = (short)((typeId & 0xFFFF0000) >> 16);
            short moduleTypeId = (short)(typeId & 0x0000FFFF);

            Type t = moduleRegistries[assemblyId][moduleTypeId];
            return (IDependencyFingerprintProtocol)Activator.CreateInstance(t);
        }

        private ModuleTypeRegistry GetOrCreateRegistry(Assembly assembly)
        {
            short assemblyId;
            if (!assemblyIds.TryGetValue(assembly, out assemblyId))
            {
                assemblyId = lastAssemblyId++;
                assemblyIds.Add(assembly, assemblyId);
            }

            ModuleTypeRegistry registry;
            if (!moduleRegistries.TryGetValue(assemblyId, out registry))
            {
                registry = new ModuleTypeRegistry();
                moduleRegistries.Add(assemblyId, registry);
            }

            return registry;
        }
    }
}

