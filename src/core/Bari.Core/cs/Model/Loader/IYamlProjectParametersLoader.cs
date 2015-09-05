using Bari.Core.Model.Parameters;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Interface for loading <see cref="IProjectParameters"/> blocks from YAML files
    /// 
    /// <para>
    /// The loader will ask each registered <see cref="IYamlProjectParametersLoader"/> for every parsed block with the
    /// <see cref="Supports"/> method, and if it returns <c>true</c>, it calls the <see cref="Load"/> method to get
    /// a <see cref="IProjectParameters"/> instance.
    /// </para>
    /// </summary>
    public interface IYamlProjectParametersLoader
    {
        /// <summary>
        /// Checks whether a given parameter block is supported
        /// </summary>
        /// <param name="name">Name of the block</param>
        /// <returns>Returns <c>true</c> if the given block is supported.</returns>
        bool Supports(string name);

        /// <summary>
        /// Loads the YAML block
        /// </summary>
        /// <param name="suite">The suite being loaded (at load time it is not yet bound in the kernel)</param>
        /// <param name="name">Name of the block (same that was passed to <see cref="Supports"/>)</param>
        /// <param name="value">The YAML node representing the value</param>
        /// <param name="parser">The YAML parser to be used</param>
        /// <returns>Returns the loaded node</returns>
        IProjectParameters Load(Suite suite, string name, YamlNode value, YamlParser parser);
    }
}