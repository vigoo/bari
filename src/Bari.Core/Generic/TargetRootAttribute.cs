using System;

namespace Bari.Core.Generic
{
    /// <summary>
    /// Attribute indicating that a paramter of type <see cref="IFileSystemDirectory"/>
    /// should be bound to the suite's build target root directory 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class TargetRootAttribute: Attribute
    {         
    }
}