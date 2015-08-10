using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Model;

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
        private readonly ICoreBuilderFactory coreBuilderFactory;
        private readonly IPythonScriptedBuilderFactory builderFactory;

        public PythonScriptedProjectBuilderFactory(Suite suite, IPythonScriptedBuilderFactory builderFactory, ICoreBuilderFactory coreBuilderFactory)
        {
            if (suite.HasParameters("build-scripts"))
                buildScriptMappings = suite.GetParameters<BuildScriptMappings>("build-scripts");
            else
                buildScriptMappings = new BuildScriptMappings();

            this.builderFactory = builderFactory;
            this.coreBuilderFactory = coreBuilderFactory;
        }       

        public IBuilder Create(IEnumerable<Project> projects)
        {
            var builders = new List<IBuilder>();

            foreach (var project in projects)
            {
                foreach (var sourceSet in project.SourceSets)
                {
                    if (buildScriptMappings.HasBuildScriptFor(sourceSet))
                    {
                        var buildScript = buildScriptMappings.GetBuildScriptFor(sourceSet);
                        builders.Add(builderFactory.CreatePythnoScriptedBuilder(project, buildScript));
                    }
                }
            }

            return coreBuilderFactory.Merge(builders.ToArray());
        }
    }
}