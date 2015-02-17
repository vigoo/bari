using Bari.Core.Generic;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VsCore.Build
{
    /// <summary>
    /// Interface for creating new <see cref="MSBuildRunner"/> instances
    /// </summary>
    public interface IMSBuildRunnerFactory
    {
        MSBuildRunner CreateMSBuildRunner(SlnBuilder slnBuilder, TargetRelativePath slnPath, MSBuildVersion version);
    }
}