using System;
using System.Collections.Generic;

namespace Bari.Core.Model.Parameters
{
    public class InheritableProjectParameters<TPropertyDefs>: IProjectParameters
        where TPropertyDefs: ProjectParametersPropertyDefs, new()
    {
        private readonly IDictionary<string, IPropertyValue> propertyValues = new Dictionary<string, IPropertyValue>();
        private readonly InheritableProjectParameters<TPropertyDefs> parent;
        private readonly TPropertyDefs defs = new TPropertyDefs();

        public InheritableProjectParameters(InheritableProjectParameters<TPropertyDefs> parent = null)
        {
            this.parent = parent;

            foreach (var propertyDef in defs.Properties)
            {
                var value = CreateUnspecifiedValue(propertyDef.Type);
                propertyValues.Add(propertyDef.Name, value);
            }
        }

        private static IPropertyValue CreateUnspecifiedValue(Type type)
        {
            return (IPropertyValue)Activator.CreateInstance(typeof (PropertyValue<>).MakeGenericType(type));
        }

        private static IPropertyValue CreateValue<T>(T value)
        {
            return (IPropertyValue) Activator.CreateInstance(typeof (PropertyValue<>).MakeGenericType(typeof (T)), value);
        }

        protected bool IsSpecified(string name)
        {            
            var value = GetPropertyValue(name);
            return value.IsSpecified || (parent != null && parent.IsSpecified(name));
        }

        protected T Get<T>(string name)
        {
            defs.TypeCheck(name, typeof(T));

            var value = GetPropertyValue(name);
            if (!value.IsSpecified && parent != null)
                value = parent.GetPropertyValue(name);

            if (!value.IsSpecified)
                throw new InvalidOperationException("Property value is not specified: " + name);
            
            return (T) value.Value;
        }

        protected void Set<T>(string name, T value)
        {
            defs.TypeCheck(name, typeof(T));
            propertyValues[name] = CreateValue(value);
        }

        protected void Clear(string name)
        {
            propertyValues[name] = CreateUnspecifiedValue(defs.TypeOf(name));
        }

        private IPropertyValue GetPropertyValue(string name)
        {
            if (!defs.IsDefined(name))
                throw new ArgumentOutOfRangeException("name", "Property is not defined");

            return propertyValues[name];            
        }
    }
}