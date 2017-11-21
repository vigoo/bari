using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Build
{
    public interface IVcxprojBuilderFactory
    {
        VcxprojBuilder CreateVcxprojBuilder(Project project);
    }
}