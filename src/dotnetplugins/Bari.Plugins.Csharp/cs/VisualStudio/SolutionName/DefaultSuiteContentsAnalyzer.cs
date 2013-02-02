using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio.SolutionName
{
    /// <summary>
    /// Default implementation for the <see cref="ISuiteContentsAnalyzer"/> interface
    /// </summary>
    public class DefaultSuiteContentsAnalyzer: ISuiteContentsAnalyzer
    {
        private readonly Suite suite;
        
        /// <summary>
        /// Creates the analyzer for the given suite
        /// </summary>
        /// <param name="suite">Suite to analyze</param>
        public DefaultSuiteContentsAnalyzer(Suite suite)
        {
            this.suite = suite;
        }

        /// <summary>
        /// Gets the modules covered by the given set of projects
        /// </summary>
        /// <param name="projects">A set of projects</param>
        /// <returns>Returns a set of module matches. See <see cref="ModuleMatch"/></returns>
        public IEnumerable<ModuleMatch> GetCoveredModules(IEnumerable<Project> projects)
        {
            var projectSet = new HashSet<Project>(projects);

            var involvedModules = new HashSet<Module>(
                from module in suite.Modules
                from project in projectSet
                where module.Projects.Contains(project) || module.TestProjects.Contains(project)
                select module);

            foreach (var involvedModule in involvedModules)
            {
                bool allProjectsInvolved = involvedModule.Projects.All(projectSet.Contains);
                bool allTestProjectsInvolved = involvedModule.TestProjects.All(projectSet.Contains);
                bool anyTestProjectsInvolved = involvedModule.TestProjects.Any(projectSet.Contains);

                yield return new ModuleMatch(
                    involvedModule,
                    partial: !allProjectsInvolved || (anyTestProjectsInvolved && !allTestProjectsInvolved),
                    includingTests: anyTestProjectsInvolved);
            }
        }

        /// <summary>
        /// Gets the product which contains exactly the given set of modules.
        /// </summary>
        /// <param name="modules">A set of modules</param>
        /// <returns>Name of the matching product, or <c>null</c></returns>
        public string GetProductName(IEnumerable<Module> modules)
        {
            var set1 = new HashSet<Module>(modules);

            return (
                from product in suite.Products 
                let set2 = new HashSet<Module>(product.Modules) 
                where set1.SetEquals(set2) 
                select product.Name).FirstOrDefault();
        }
    }
}