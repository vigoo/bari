using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bari.Core.Build.Dependencies.Protocol
{
    public class DependencyFingerprintProtocolRegistry: IDependencyFingerprintProtocolRegistry
    {
        private class ModuleTypeRegistry
        {
            private short lastId;
            private short lastEnumId;
            private readonly IDictionary<short, Type> typesPerId = new Dictionary<short, Type>();
            private readonly IDictionary<Type, short> idsPerType = new Dictionary<Type, short>();
            private readonly IDictionary<short, Tuple<Func<object, int>, Func<int, object>, Type>> enumsPerId = new Dictionary<short, Tuple<Func<object, int>, Func<int, object>, Type>>();
            private readonly IDictionary<Type, short> idsPerEnum = new Dictionary<Type, short>();

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

            public void AddEnum<T>(Type t, Func<T, int> encode, Func<int, T> decode)
            {
                short id = lastEnumId++;
                enumsPerId.Add(id, Tuple.Create<Func<object, int>, Func<int, object>, Type>(
                    obj => encode((T)obj), 
                    val => decode(val),
                    t));
                idsPerEnum.Add(t, id);
            }

            public short GetEnumId(Type enumType)
            {
                return idsPerEnum[enumType];
            }

            public Func<int, object> GetEnumDecoder(short enumTypeId)
            {
                return enumsPerId[enumTypeId].Item2;
            }

            public Type GetEnumType(short enumTypeId)
            {
                return enumsPerId[enumTypeId].Item3;
            }

            public bool HasEnum(Type type)
            {
                return idsPerEnum.ContainsKey(type);
            }
        }

        private readonly IDictionary<Assembly, short> assemblyIds = new Dictionary<Assembly, short>();
        private readonly IDictionary<short, ModuleTypeRegistry> moduleRegistries = new Dictionary<short, ModuleTypeRegistry>();
        private short lastAssemblyId;

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

        public void RegisterEnum<T>(Func<T, int> encode, Func<int, T> decode) where T : struct
        {
            var t = typeof(T);
            var assembly = t.Assembly;
            var registry = GetOrCreateRegistry(assembly);
            registry.AddEnum(t, encode, decode);
        }

        public int? GetEnumId(object value)
        {
            var t = value.GetType();
            var assembly = t.Assembly;

            short assemblyId;
            if (assemblyIds.TryGetValue(assembly, out assemblyId))
            {
                var moduleRegistry = moduleRegistries[assemblyId];

                if (moduleRegistry.HasEnum(t))
                {
                    var enumTypeId = moduleRegistry.GetEnumId(t);
                    return ((int) (assemblyId) << 16) | ((int) enumTypeId);
                }
            }
             
            return null;
        }

        public object CreateEnum(int typeId, int value)
        {
            short assemblyId = (short)((typeId & 0xFFFF0000) >> 16);
            short enumTypeId = (short)(typeId & 0x0000FFFF);

            var decoder = moduleRegistries[assemblyId].GetEnumDecoder(enumTypeId);
            return decoder(value);
        }

        public Type GetEnumType(int typeId)
        {
            short assemblyId = (short)((typeId & 0xFFFF0000) >> 16);
            short enumTypeId = (short)(typeId & 0x0000FFFF);

            return moduleRegistries[assemblyId].GetEnumType(enumTypeId);
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

