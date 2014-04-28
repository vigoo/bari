using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Model;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Build
{
    /// <summary>
    /// A <see cref="IProjectBuilderFactory"/> implementation that creates <see cref="PythonScriptedBuilder"/>
    /// instances for projects having a source set supported by any of the python scripts belonging to the 
    /// suite.
    /// </summary>
    public class PythonScriptedProjectBuilderFactory : IProjectBuilderFactory
    {
        private readonly BuildScriptMappings buildScriptMappings;
        private readonly ISourceSetFingerprintFactory fingerprintFactory;
        private readonly IProjectBuildScriptRunner scriptRunner;

        public PythonScriptedProjectBuilderFactory(Suite suite, ISourceSetFingerprintFactory fingerprintFactory, IProjectBuildScriptRunner scriptRunner)
        {
            if (suite.HasParameters("build-scripts"))
                buildScriptMappings = suite.GetParameters<BuildScriptMappings>("build-scripts");
            else
                buildScriptMappings = new BuildScriptMappings();

            this.fingerprintFactory = fingerprintFactory;
            this.scriptRunner = scriptRunner;
        }

        /// <summary>
        /// Adds the builders (<see cref="IBuilder"/>) to the given build context which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="projects">Projects to be built</param>
        public IBuilder AddToContext(IBuildContext context, IEnumerable<Project> projects)
        {
            var builders = new List<IBuilder>();

            foreach (var project in projects)
            {
                foreach (var sourceSet in project.SourceSets)
                {
                    if (buildScriptMappings.HasBuildScriptFor(sourceSet))
                    {
                        var buildScript = buildScriptMappings.GetBuildScriptFor(sourceSet);
                        builders.Add(new PythonScriptedBuilder(project, buildScript, fingerprintFactory, scriptRunner));
                    }
                }
            }

            var merged = builders.ToArray().Merge();
            if (merged != null)
                merged.AddToContext(context);
            return merged;
        }
    }
}