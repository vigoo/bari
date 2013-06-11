using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Build
{
    public interface IVcxprojBuilderFactory
    {
        VcxprojBuilder CreateFsprojBuilder(Project project);
    }
}