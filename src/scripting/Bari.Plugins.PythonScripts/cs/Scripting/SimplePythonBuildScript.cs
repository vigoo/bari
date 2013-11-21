using System.IO;
using Bari.Core.Generic;

namespace Bari.Plugins.PythonScripts.Scripting
{
    /// <summary>
    /// Build script defined by individual files in the suite's <c>scripts</c> directory.
    /// 
    /// <para>
    /// The script file's name definfes the source set for which it is automatically executed.
    /// 
    /// <example>
    /// A build script with the suite relative path <c>scripts\icons.py</c> will be executed on
    /// every project's <c>icons</c> source set, if it has one.
    /// </example>
    /// </para>
    /// </summary>
    public class SimplePythonBuildScript : IBuildScript
    {
        private readonly string name;
        private readonly string script;

        /// <summary>
        /// Gets the source set's name to be included in the script's scope
        /// </summary>
        public string SourceSetName
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the script's name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the script source
        /// </summary>
        public string Source
        {
            get { return script; }
        }

        public SimplePythonBuildScript(SuiteRelativePath scriptPath, [SuiteRoot] IFileSystemDirectory suiteRoot)
        {
            name = Path.GetFileNameWithoutExtension(scriptPath);
            using (var reader = suiteRoot.ReadTextFile(scriptPath))
            {
                script = reader.ReadToEnd();
            }
        }
    }
}