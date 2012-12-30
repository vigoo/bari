using System;

namespace Bari.Core.Generic
{
    /// <summary>
    /// Attribute indicating that a parameter of type <see cref="IFileSystemDirectory"/>
    /// should be bound to the suite's cache directory 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class CacheRootAttribute : Attribute
    {
    }
}