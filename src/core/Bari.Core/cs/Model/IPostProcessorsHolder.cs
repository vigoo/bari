using System.Collections.Generic;

namespace Bari.Core.Model
{
    public interface IPostProcessorsHolder
    {
        /// <summary>
        /// Gets the postprocessors associated with this project
        /// </summary>
        IEnumerable<PostProcessDefinition> PostProcessors { get; }

        /// <summary>
        /// Gets the relative path to the target directory where this post processable item is compiled to
        /// </summary>
        string RelativeTargetPath { get; }

        /// <summary>
        /// Adds a new postprocessor to this project
        /// </summary>
        /// <param name="postProcessDefinition">Post processor type and parameters</param>
        void AddPostProcessor(PostProcessDefinition postProcessDefinition);

        /// <summary>
        /// Gets a registered post processor by its name
        /// </summary>
        /// <param name="key">Name of the post processor</param>
        /// <returns>Returns the post processor definition</returns>
        PostProcessDefinition GetPostProcessor(string key);
    }
}