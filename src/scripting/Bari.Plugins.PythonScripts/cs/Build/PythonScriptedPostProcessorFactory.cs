using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Model;

namespace Bari.Plugins.PythonScripts.Build
{
    public class PythonScriptedPostProcessorFactory: IPostProcessorFactory
    {
        private readonly PostProcessorScriptMappings scriptMappings;
        private readonly IPythonScriptedBuilderFactory builderFactory;

        public PythonScriptedPostProcessorFactory(Suite suite, IPythonScriptedBuilderFactory builderFactory)
        {
            this.builderFactory = builderFactory;

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
                var postProcessor = builderFactory.CreatePythonScriptedPostProcessor(script, holder, definition, dependencies);
                return postProcessor;
            }
            else
            {
                return null;
            }
        }
    }
}