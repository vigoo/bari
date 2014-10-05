using System;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Exceptions
{
    public class ScriptException: Exception
    {
        public ScriptException(IScript script)
            : base(String.Format("Failed to execute script {0}", script.Name))
        {            
        }

        public override string ToString()
        {
            return Message;
        }
    }
}