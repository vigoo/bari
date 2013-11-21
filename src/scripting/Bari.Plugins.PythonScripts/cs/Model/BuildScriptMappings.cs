using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Plugins.PythonScripts.Scripting;

namespace Bari.Plugins.PythonScripts.Model
{
    /// <summary>
    /// Maps build scripts to source sets
    /// </summary>
    public class BuildScriptMappings : IProjectParameters
    {
        private readonly IDictionary<string, IBuildScript> buildScriptsForSourceSets;

        public BuildScriptMappings()
        {
            buildScriptsForSourceSets = new Dictionary<string, IBuildScript>();
        }

        /// <summary>
        /// Adds a new build script to the suite
        /// </summary>
        /// <param name="sourceSetName">Name of the source set for which this build script
        /// should be executed.</param>
        /// <param name="buildScript">The build script</param>
        public void Add(string sourceSetName, IBuildScript buildScript)
        {
            buildScriptsForSourceSets.Add(sourceSetName, buildScript);
        }

        /// <summary>
        /// Checks if there is a build script mapped for the given source set
        /// </summary>
        /// <param name="sourceSet">The source set to look a build script for</param>
        /// <returns>Returns <c>true</c> if there is a build script available</returns>
        public bool HasBuildScriptFor(ISourceSet sourceSet)
        {
            return buildScriptsForSourceSets.ContainsKey(sourceSet.Type);
        }

        /// <summary>
        /// Gets the mapped build script for the given source set
        /// </summary>
        /// <param name="sourceSet">The source set to look a build script for</param>
        /// <returns>Returns the build script mapped to the given source set type</returns>
        public IBuildScript GetBuildScriptFor(ISourceSet sourceSet)
        {
            return buildScriptsForSourceSets[sourceSet.Type];
        }
    }
}