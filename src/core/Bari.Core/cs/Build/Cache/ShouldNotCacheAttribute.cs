using System;

namespace Bari.Core.Build.Cache
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShouldNotCacheAttribute: Attribute
    {         
    }
}