using System;

namespace Bari.Core.Build
{
    /// <summary>
    /// Indicates that if the builder fails, it is valid to fall back to a previous cached result
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FallbackToCacheAttribute: Attribute
    {         
    }
}