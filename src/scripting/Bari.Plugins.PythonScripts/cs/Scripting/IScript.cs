namespace Bari.Plugins.PythonScripts.Scripting
{
    public interface IScript
    {
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