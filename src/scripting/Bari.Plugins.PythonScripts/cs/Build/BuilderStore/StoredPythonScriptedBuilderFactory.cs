using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.BuilderStore;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Build.PostProcessors;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Build.BuilderStore
{
    public class StoredPythonScriptedBuilderFactory: IPythonScriptedBuilderFactory
    {
        private readonly IPythonScriptedBuilderFactory baseImpl;
        private readonly IBuilderStore store;

        public StoredPythonScriptedBuilderFactory(IPythonScriptedBuilderFactory baseImpl, IBuilderStore store)
        {
            this.baseImpl = baseImpl;
            this.store = store;
        }

        public PythonScriptedBuilder CreatePythnoScriptedBuilder(Project project, IBuildScript buildScript)
        {
            return store.Add(baseImpl.CreatePythnoScriptedBuilder(project, buildScript));
        }

        public PythonScriptedPostProcessor CreatePythonScriptedPostProcessor(IPostProcessorScript script, IPostProcessorsHolder target,
            PostProcessDefinition definition, IEnumerable<IBuilder> dependencies)
        {
            return store.Add(baseImpl.CreatePythonScriptedPostProcessor(script, target, definition, dependencies));
        }
    }
}