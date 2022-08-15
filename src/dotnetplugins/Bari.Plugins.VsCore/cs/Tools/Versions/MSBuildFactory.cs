using System;
using Bari.Core.UI;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VsCore.Tools.Versions
{
    public class MSBuildFactory: IMSBuildFactory
    {
        private readonly IParameters parameters;

        public MSBuildFactory(IParameters parameters)
        {
            this.parameters = parameters;
        }

        public IMSBuild CreateMSBuild(MSBuildVersion version)
        {
            switch (version)
            {
                case MSBuildVersion.Net40x86: return new MSBuild40x86(parameters);
                case MSBuildVersion.Net40x64: return new MSBuild40x64(parameters);
                case MSBuildVersion.VS2013: return new MSBuildVS2013(parameters);
                case MSBuildVersion.VS2015: return new MSBuildVS2015(parameters);
                case MSBuildVersion.VS2017: return new MSBuildVS2017(parameters);
                case MSBuildVersion.VS2019: return new MSBuildVS2019(parameters);
                case MSBuildVersion.VS2022: return new MSBuildVS2022(parameters);
                case MSBuildVersion.Default: return new MSBuildInPath(parameters);
                default:
                    throw new ArgumentOutOfRangeException("version");
            }
        }
    }
}