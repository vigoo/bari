using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Build.Dependencies;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Build.PostProcessors
{
    public class PythonScriptedPostProcessor: IPostProcessor
    {
        private readonly IPostProcessorScript script;
        private readonly IPostProcessorsHolder target;
        private readonly PostProcessDefinition definition;
        private readonly ISet<IBuilder> dependencies;
        private readonly IPostProcessorScriptRunner scriptRunner;

        public PythonScriptedPostProcessor(IPostProcessorScript script, IPostProcessorsHolder target, PostProcessDefinition definition, IEnumerable<IBuilder> dependencies, IPostProcessorScriptRunner scriptRunner)
        {
            this.script = script;
            this.target = target;
            this.definition = definition;
            this.scriptRunner = scriptRunner;
            this.dependencies = new HashSet<IBuilder>(dependencies);
        }

        public IDependencies Dependencies
        {
            get
            {
                return new MultipleDependencies(
                    dependencies
                        .Select(subtask => new SubtaskDependency(subtask))
                        .Cast<IDependencies>()
                        .Concat(new[] {new ScriptDependency(script)}));
            }
        }

        public string Uid
        {
            get { return string.Format("pp/{0}/{1}", target, script.Name); }
        }

        public void AddToContext(IBuildContext context)
        {
            context.AddBuilder(this, dependencies);
        }

        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            return scriptRunner.Run(target, definition, script);
        }

        public bool CanRun()
        {
            return true;
        }

        public PostProcessDefinition Definition
        {
            get { return definition; }
        }

        public override string ToString()
        {
            return String.Format("[{0}/{1}]", target, definition.Name);
        }

    }
}