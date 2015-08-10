using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Build.PostProcessors;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Build
{
    public interface IPythonScriptedBuilderFactory
    {
        PythonScriptedBuilder CreatePythnoScriptedBuilder(Project project, IBuildScript buildScript);

        PythonScriptedPostProcessor CreatePythonScriptedPostProcessor(IPostProcessorScript script,
            IPostProcessorsHolder target, PostProcessDefinition definition, IEnumerable<IBuilder> dependencies);
    }
}