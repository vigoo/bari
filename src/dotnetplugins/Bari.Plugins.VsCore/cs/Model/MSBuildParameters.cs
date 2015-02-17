using Bari.Core.Model;

namespace Bari.Plugins.VsCore.Model
{
    public class MSBuildParameters: IProjectParameters
    {
        public MSBuildVersion Version { get; set; }

        public MSBuildParameters()
        {
            Version = MSBuildVersion.Net40x86;
        }
    }
}