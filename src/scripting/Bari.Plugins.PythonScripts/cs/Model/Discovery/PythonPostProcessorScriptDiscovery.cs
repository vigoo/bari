using System.IO;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Model.Discovery
{
    public class PythonPostProcessorScriptDiscovery : ISuiteExplorer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PythonBuildScriptDiscovery));

        public void ExtendWithDiscoveries(Suite suite)
        {
            var scriptsDir = suite.SuiteRoot.GetChildDirectory("scripts");
            if (scriptsDir != null)
            {
                var ppScriptsDir = scriptsDir.GetChildDirectory("postprocessors");

                if (ppScriptsDir != null)
                {
                    PostProcessorScriptMappings mappings;
                    if (suite.HasParameters("post-processor-scripts"))
                    {
                        mappings = suite.GetParameters<PostProcessorScriptMappings>("post-processor-scripts");
                    }
                    else
                    {
                        mappings = new PostProcessorScriptMappings();
                        suite.AddParameters("post-processor-scripts", mappings);
                    }

                    foreach (var scriptFile in ppScriptsDir.Files)
                    {
                        var ext = Path.GetExtension(scriptFile);
                        if (ext != null && ext.ToLowerInvariant() == ".py")
                        {
                            var script = new SimplePythonPostProcessorScript(
                                new SuiteRelativePath(Path.Combine("scripts", "postprocessors", scriptFile)),
                                suite.SuiteRoot);

                            mappings.Add(script.PostProcessorId, script);

                            log.DebugFormat("Discovered build script: {0}", script.Name);
                        }
                    }
                }
            }
        }
    }
}