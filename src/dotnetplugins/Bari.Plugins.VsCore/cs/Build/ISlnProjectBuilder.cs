using Bari.Core.Build;

namespace Bari.Plugins.VsCore.Build
{
    /// <summary>
    /// Common interface for builders generating Visual Studio projects (csproj, fsproj, etc.)
    /// </summary>
    public interface ISlnProjectBuilder: IProjectBuilder
    {
        /// <summary>
        /// Gets the builder's full source code dependencies
        /// </summary>
         IDependencies FullSourceDependencies { get; }

        void ReplaceReferenceBuilder(IReferenceBuilder original, IReferenceBuilder replacement);
    }
}