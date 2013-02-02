using System.Collections.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio.SolutionName
{
    /// <summary>
    /// Helper interface for suite processing 
    /// </summary>
    public interface ISuiteContentsAnalyzer
    {
        /// <summary>
        /// Gets the modules covered by the given set of projects
        /// </summary>
        /// <param name="projects">A set of projects</param>
        /// <returns>Returns a set of module matches. See <see cref="ModuleMatch"/></returns>
        IEnumerable<ModuleMatch> GetCoveredModules(IEnumerable<Project> projects);

        /// <summary>
        /// Gets the product which contains exactly the given set of modules.
        /// </summary>
        /// <param name="modules">A set of modules</param>
        /// <returns>Name of the matching product, or <c>null</c></returns>
        string GetProductName(IEnumerable<Module> modules);
    }
}