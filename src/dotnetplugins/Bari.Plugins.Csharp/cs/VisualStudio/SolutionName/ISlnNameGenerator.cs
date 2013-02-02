using System.Collections.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio.SolutionName
{
    /// <summary>
    /// Interface for SLN name generators
    /// </summary>
    public interface ISlnNameGenerator
    {
        /// <summary>
        /// Generates a file name for a VS solution file which will contain the given set of projects.
        /// </summary>
        /// <param name="projects">Set of projects to be included in the SLN file</param>
        /// <returns>Returns a valid file name without extension</returns>
        string GetName(IEnumerable<Project> projects);
    }
}