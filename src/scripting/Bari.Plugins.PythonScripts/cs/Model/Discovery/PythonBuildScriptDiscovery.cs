using System.IO;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Model.Discovery
{
    /// <summary>
    /// Adds the simple python build scripts from the suite's <c>scripts</c> directory
    /// to the suite's build script mappings (<see cref="BuildScriptMappings"/>)
    /// </summary>
    public class PythonBuildScriptDiscovery : ISuiteExplorer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (PythonBuildScriptDiscovery));

        public void ExtendWithDiscoveries(Suite suite)
        {           
            var scriptsDir = suite.SuiteRoot.GetChildDirectory("scripts");
            if (scriptsDir != null)
            {
                BuildScriptMappings mappings;
                if (suite.HasParameters("build-scripts"))
                {
                    mappings = suite.GetParameters<BuildScriptMappings>("build-scripts");
                }
                else 
                {
                    mappings = new BuildScriptMappings();
                    suite.AddParameters("build-scripts", mappings);
                }

                foreach (var scriptFile in scriptsDir.Files)
                {
                    var ext = Path.GetExtension(scriptFile);
                    if (ext != null && ext.ToLowerInvariant() == ".py")
                    {
                        var script = new SimplePythonBuildScript(
                            new SuiteRelativePath(Path.Combine("scripts", scriptFile)),
                            suite.SuiteRoot);

                        mappings.Add(script.SourceSetName, script);

                        log.DebugFormat("Discovered build script: {0}", script.Name);
                    }
                }
            }
        }
    }
}