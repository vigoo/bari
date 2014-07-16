using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// Post processor
    /// </summary>
    public interface IPostProcessor: IBuilder
    {
        /// <summary>
        /// Gets the post processor definition this builder executes
        /// </summary>
        PostProcessDefinition Definition { get; }
    }
}