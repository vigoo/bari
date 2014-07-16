using System;
using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Build.PostProcessors;
using Bari.Plugins.PythonScripts.Model;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Build
{
    public class PythonScriptedPostProcessorFactory: IPostProcessorFactory
    {
        private readonly PostProcessorScriptMappings scriptMappings;
        private readonly IPostProcessorScriptRunner scriptRunner;

        public PythonScriptedPostProcessorFactory(Suite suite, IPostProcessorScriptRunner scriptRunner)
        {
            this.scriptRunner = scriptRunner;
            if (suite.HasParameters("post-processor-scripts"))
                scriptMappings = suite.GetParameters<PostProcessorScriptMappings>("post-processor-scripts");
            else
                scriptMappings = new PostProcessorScriptMappings();
        }

        public IPostProcessor CreatePostProcessorFor(IPostProcessorsHolder holder, PostProcessDefinition definition, IEnumerable<IBuilder> dependencies)
        {
            if (scriptMappings.HasScriptFor(definition.PostProcessorId))
            {
                var script = scriptMappings.GetScriptFor(definition.PostProcessorId);
                var postProcessor = new PythonScriptedPostProcessor(script, holder, definition, dependencies, scriptRunner);
                return postProcessor;
            }
            else
            {
                return null;
            }
        }
    }
}