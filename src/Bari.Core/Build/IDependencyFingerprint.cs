using System;

namespace Bari.Core.Build
{
    /// <summary>
    /// Represents a generic fingerprint of a dependency set, which can be compared to other fingerprints
    /// to determine cache validity.
    /// </summary>
    public interface IDependencyFingerprint: IEquatable<IDependencyFingerprint>
    {
         
    }
}