using System.Collections.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// Factory for <see cref="IPostProcessor"/> implementations. 
    /// 
    /// <para>This is the extension point to add new post processor types</para>
    /// </summary>
    public interface IPostProcessorFactory
    {
        /// <summary>
        /// Creates a new post processor if supported by this factory
        /// </summary>
        /// <param name="holder">The object the post processor will run for</param>
        /// <param name="definition">The post processing definition</param>
        /// <param name="dependencies">The dependencies the post processor will work on</param>
        /// <returns>Returns a post processor, or <c>null</c> if it is not supported</returns>
        IPostProcessor CreatePostProcessorFor(IPostProcessorsHolder holder, PostProcessDefinition definition, IEnumerable<IBuilder> dependencies);
    }
}