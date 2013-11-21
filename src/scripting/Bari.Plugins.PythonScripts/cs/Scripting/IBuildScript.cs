namespace Bari.Plugins.PythonScripts.Scripting
{
    /// <summary>
    /// Build scripts are executed on project's source sets and generate a set of
    /// files under the target directory.
    /// </summary>
    public interface IBuildScript
    {
        /// <summary>
        /// Gets the source set's name to be included in the script's scope
        /// </summary>
        string SourceSetName { get; }

        /// <summary>
        /// Gets the script's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the script source
        /// </summary>
        string Source { get; }
    }
}