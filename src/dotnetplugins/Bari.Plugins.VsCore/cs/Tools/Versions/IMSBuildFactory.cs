using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VsCore.Tools.Versions
{
    public interface IMSBuildFactory
    {
        IMSBuild CreateMSBuild(MSBuildVersion version);
    }
}