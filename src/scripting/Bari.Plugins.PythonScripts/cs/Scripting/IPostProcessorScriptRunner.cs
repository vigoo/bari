using System.Collections.Generic;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.PythonScripts.Scripting
{
    public interface IPostProcessorScriptRunner
    {
        ISet<TargetRelativePath> Run(IPostProcessorsHolder target, PostProcessDefinition definition,
            IPostProcessorScript script);
    }
}