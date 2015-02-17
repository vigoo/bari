using Bari.Core.UI;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VsCore.Tools.Versions
{
    public class XBuildFactory: IMSBuildFactory
    {
        private readonly IParameters parameters;

        public XBuildFactory(IParameters parameters)
        {
            this.parameters = parameters;
        }

        public IMSBuild CreateMSBuild(MSBuildVersion version)
        {
            return new XBuild(parameters);
        }
    }
}