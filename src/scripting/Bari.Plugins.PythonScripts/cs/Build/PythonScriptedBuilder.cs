using System;
using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Build.Dependencies;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Build
{
    /// <summary>
    /// Builder that executes a python script on a given project's given source set
    /// </summary>
    public class PythonScriptedBuilder: IBuilder
    {
        private readonly Project project;
        private readonly IBuildScript buildScript;
        private readonly ISourceSetFingerprintFactory fingerprintFactory;
        private readonly IProjectBuildScriptRunner scriptRunner;

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get
            {
                return MultipleDependenciesHelper.CreateMultipleDependencies(
                    new HashSet<IDependencies>(new IDependencies[]
                        {
                            new SourceSetDependencies(fingerprintFactory, project.GetSourceSet(buildScript.SourceSetName)),
                            new ScriptDependency(buildScript)
                        }));
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get { return project.Module + "." + project.Name + "/" + buildScript.Name; }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            context.AddBuilder(this, new IBuilder[0]);
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            return scriptRunner.Run(project, buildScript);
        }

        public PythonScriptedBuilder(Project project, IBuildScript buildScript, ISourceSetFingerprintFactory fingerprintFactory, IProjectBuildScriptRunner scriptRunner)
        {
            this.project = project;
            this.buildScript = buildScript;
            this.fingerprintFactory = fingerprintFactory;
            this.scriptRunner = scriptRunner;
        }

        public override string ToString()
        {
            return String.Format("[{0}.{1}/{2}]", project.Module.Name, project.Name, buildScript.Name);
        }
    }
}