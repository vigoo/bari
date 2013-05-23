using Bari.Core.Model;

namespace Bari.Plugins.VsCore.Build
{
    /// <summary>
    /// Special builder factory interface to create <see cref="InSolutionReferenceBuilder"/> instances
    /// </summary>
    public interface IInSolutionReferenceBuilderFactory
    {
        InSolutionReferenceBuilder CreateInSolutionReferenceBuilder(Project project);
    }
}