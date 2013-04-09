using Bari.Core.Generic;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Interface for creating new <see cref="MSBuildRunner"/> instances
    /// </summary>
    public interface IMSBuildRunnerFactory
    {
        MSBuildRunner CreateMSBuildRunner(SlnBuilder slnBuilder, TargetRelativePath slnPath);
    }
}