using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Build.Dependencies
{
    public class ScriptDependency : IDependencies
    {
        private readonly IScript buildScript;

        public ScriptDependency(IScript buildScript)
         {
             this.buildScript = buildScript;
         }

        public IDependencyFingerprint CreateFingerprint()
        {
            return new ObjectPropertiesFingerprint(buildScript, new[] { "Source"});
        }
    }
}