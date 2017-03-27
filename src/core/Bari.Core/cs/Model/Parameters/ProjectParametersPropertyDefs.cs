using System;
using System.Collections.Generic;

namespace Bari.Core.Model.Parameters
{
    public abstract class ProjectParametersPropertyDefs<TParams> : IInheritableProjectParametersDef
        where TParams: IInheritableProjectParameters
    {
        public struct PropertyDef
        {
            private readonly string name;
            private readonly Type type;
            private readonly bool mergeWithInherited; 

            public string Name
            {
                get { return name; }
            }

            public Type Type
            {
                get { return type; }
            }

            public bool MergeWithInherited
            {
                get { return mergeWithInherited; }
            }

            public PropertyDef(string name, Type type, bool mergeWithInherited = false)
            {
                this.type = type;
                this.name = name;
                this.mergeWithInherited = mergeWithInherited;
            }
        }

        private readonly IDictionary<string, PropertyDef> propertyDefinitions = new Dictionary<string, PropertyDef>();

        protected void Define<T>(string name, bool mergeWithInherited = false)
        {
            Define(name, typeof (T), mergeWithInherited);
        }

        protected void Define(string name, Type type, bool mergeWithInherited = false)
        {
            propertyDefinitions.Add(name, new PropertyDef(name, type, mergeWithInherited));
        }

        public IEnumerable<PropertyDef> Properties { get { return propertyDefinitions.Values; } }

        public bool IsDefined(string name)
        {
            return propertyDefinitions.ContainsKey(name);
        }

        public bool TypeCheck(string name, Type t)
        {
            return TypeOf(name) == t;
        }

        public Type TypeOf(string name)
        {
            return propertyDefinitions[name].Type;
        }

        public bool MergeWithInherited(string name)
        {
            return propertyDefinitions[name].MergeWithInherited;
        }

        public abstract TParams CreateDefault(Suite suite, TParams parent);
        
        IInheritableProjectParameters IInheritableProjectParametersDef.CreateDefault(Suite suite, IInheritableProjectParameters parent)
        {
            return CreateDefault(suite, (TParams) parent);
        }
    }
}