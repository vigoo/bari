using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Plugins.PythonScripts.Scripting;
using Bari.Core.UI;
using System;

namespace Bari.Plugins.PythonScripts.Build.Dependencies
{
    public class ScriptDependency : DependenciesBase
    {
        private readonly IScript buildScript;

        public ScriptDependency(IScript buildScript)
         {
             this.buildScript = buildScript;
         }

        protected override IDependencyFingerprint CreateFingerprint()
        {
            return new ObjectPropertiesFingerprint(buildScript, new[] { "Source"});
        }

        public override void Dump(IUserOutput output)
        {
            output.Message(String.Format("Script source `{0}`", buildScript.Name));
        }
    }
}