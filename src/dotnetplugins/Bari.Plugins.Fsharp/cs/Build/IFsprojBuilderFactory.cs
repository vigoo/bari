using Bari.Core.Model;

namespace Bari.Plugins.Fsharp.Build
{
    public interface IFsprojBuilderFactory
    {
        FsprojBuilder CreateFsprojBuilder(Project project);
    }
}