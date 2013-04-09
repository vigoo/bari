using System.Collections.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Factory interface to create new <see cref="SlnBuilder"/> instances
    /// </summary>
    public interface ISlnBuilderFactory
    {
        /// <summary>
        /// Creates a new sln builder
        /// </summary>
        /// <param name="projects">Projects to be included in the built SLN file</param>
        /// <returns>Returns the builder</returns>
        SlnBuilder CreateSlnBuilder(IEnumerable<Project> projects);
    }
}