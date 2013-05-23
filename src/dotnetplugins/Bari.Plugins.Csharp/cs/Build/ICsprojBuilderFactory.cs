using Bari.Core.Model;

namespace Bari.Plugins.Csharp.Build
{
    public interface ICsprojBuilderFactory
    {
        CsprojBuilder CreateCsprojBuilder(Project project);
    }
}