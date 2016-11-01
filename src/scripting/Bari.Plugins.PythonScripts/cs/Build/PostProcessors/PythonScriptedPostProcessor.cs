using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Cache;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Build.Dependencies;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Build.PostProcessors
{
    [AggressiveCacheRestore]
    public class PythonScriptedPostProcessor: BuilderBase<PythonScriptedPostProcessor>, IPostProcessor
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

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public override IDependencies Dependencies
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

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get { return string.Format("pp/{0}/{1}", target, script.Name); }
        }

        public override IEnumerable<IBuilder> Prerequisites
        {
            get { return dependencies; }
        }

        public override void AddPrerequisite(IBuilder target)
        {
            if (dependencies != null)
            {
                dependencies.Add(target);
            }

            base.AddPrerequisite(target);
        }

        public override void RemovePrerequisite(IBuilder target)
        {
            if (dependencies != null)
            {
                dependencies.Remove(target);                
            }

            base.RemovePrerequisite(target);
        }


        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            return scriptRunner.Run(target, definition, script);
        }
        
        public PostProcessDefinition Definition
        {
            get { return definition; }
        }
        
        public override BuilderName Name
        {
            get
            {
                return new BuilderName("python-post-processor:" + script.Name); 
            }
        }


        public override string ToString()
        {
            return String.Format("[{0}/{1}]", target, definition.Name);
        }

    }
}