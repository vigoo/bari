using System;
using System.Collections.Generic;

namespace Bari.Core.Model.Parameters
{
    public abstract class ProjectParametersPropertyDefs
    {
        public struct PropertyDef
        {
            private readonly string name;
            private readonly Type type;

            public string Name
            {
                get { return name; }
            }

            public Type Type
            {
                get { return type; }
            }

            public PropertyDef(string name, Type type)
            {
                this.type = type;
                this.name = name;
            }
        }

        private readonly IDictionary<string, PropertyDef> propertyDefinitions = new Dictionary<string, PropertyDef>();

        protected void Define<T>(string name)
        {
            Define(name, typeof (T));
        }

        protected void Define(string name, Type type)
        {
            propertyDefinitions.Add(name, new PropertyDef(name, type));
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
    }
}