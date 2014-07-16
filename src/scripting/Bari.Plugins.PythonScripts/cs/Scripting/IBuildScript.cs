namespace Bari.Plugins.PythonScripts.Scripting
{
    /// <summary>
    /// Build scripts are executed on project's source sets and generate a set of
    /// files under the target directory.
    /// </summary>
    public interface IBuildScript: IScript
    {
        /// <summary>
        /// Gets the source set's name to be included in the script's scope
        /// </summary>
        string SourceSetName { get; }
    }
}