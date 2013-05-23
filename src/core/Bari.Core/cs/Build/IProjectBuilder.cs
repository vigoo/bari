using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// Builders building <see cref="Project"/>s
    /// </summary>
    public interface IProjectBuilder: IBuilder
    {
        /// <summary>
        /// Gets the project this builder builds
        /// </summary>
        Project Project { get; }
    }
}