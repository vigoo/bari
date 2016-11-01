using Bari.Core.Model;

namespace Bari.Core.Build
{
    public interface IBuildContextFactory
    {
        IBuildContext CreateBuildContext(Suite suite);
    }
}