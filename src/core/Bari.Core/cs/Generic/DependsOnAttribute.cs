using System;

namespace Bari.Core.Generic
{
    /// <summary>
    /// To be used on module classes, to ensure that another module is loaded before the
    /// annotated one.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DependsOnAttribute: Attribute
    {
        private readonly Type dependentModuleType;

        public Type DependentModuleType
        {
            get { return dependentModuleType; }
        }

        public DependsOnAttribute(Type dependentModuleType)
        {
            this.dependentModuleType = dependentModuleType;
        }
    }
}