using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Model
{
    public class PostProcessorScriptMappings: IProjectParameters
    {
        private readonly IDictionary<string, IPostProcessorScript> buildScriptsForPostProcessors;

         public PostProcessorScriptMappings()
        {
            buildScriptsForPostProcessors = new Dictionary<string, IPostProcessorScript>();
        }

         public void Add(PostProcessorId id, IPostProcessorScript buildScript)
        {
            buildScriptsForPostProcessors.Add(id.AsString, buildScript);
        }

        public bool HasScriptFor(PostProcessorId id)
        {
            return buildScriptsForPostProcessors.ContainsKey(id.AsString);
        }

        public IPostProcessorScript GetScriptFor(PostProcessorId id)
        {
            return buildScriptsForPostProcessors[id.AsString];
        }
    }
}