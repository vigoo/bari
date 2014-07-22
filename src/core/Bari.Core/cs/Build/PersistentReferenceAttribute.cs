using System;

namespace Bari.Core.Build
{
    /// <summary>
    /// Attribute to be used for <see cref="IReferenceBuilder"/> implementations. If applied, the output of the
    /// reference builder will be kept in cache when the clean/rebuild command is used with the <c>--keep-references</c>
    /// option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PersistentReferenceAttribute: Attribute
    {         
    }
}