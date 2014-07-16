using System.IO;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.PythonScripts.Scripting
{
    public class SimplePythonPostProcessorScript: IPostProcessorScript
    {
        private readonly string name;
        private readonly string script;

        public SimplePythonPostProcessorScript(SuiteRelativePath scriptPath, [SuiteRoot] IFileSystemDirectory suiteRoot)
        {
            name = Path.GetFileNameWithoutExtension(scriptPath);
            using (var reader = suiteRoot.ReadTextFile(scriptPath))
            {
                script = reader.ReadToEnd();
            }
        }

        public PostProcessorId PostProcessorId
        {
            get { return new PostProcessorId(name); }
        }

        public string Name
        {
            get { return name; }
        }

        public string Source
        {
            get { return script; }
        }
    }
}