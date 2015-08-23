using Bari.Core.Model;
using Bari.Core.Model.Parameters;

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