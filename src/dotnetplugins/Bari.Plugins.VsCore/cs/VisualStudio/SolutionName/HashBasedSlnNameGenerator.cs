using System.Collections.Generic;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio.SolutionName
{
    /// <summary>
    /// Simple solution name generator, the file name is an MD5 checksum computed from the 
    /// set of projects.
    /// </summary>
    public class HashBasedSlnNameGenerator: ISlnNameGenerator
    {
        /// <summary>
        /// Generates a file name for a VS solution file which will contain the given set of projects.
        /// </summary>
        /// <param name="projects">Set of projects to be included in the SLN file</param>
        /// <returns>Returns a valid file name without extension</returns>
        public string GetName(IEnumerable<Project> projects)
        {
            var result = MD5.Encode(string.Join(",",
                                   from project in projects
                                   let module = project.Module
                                   let fullName = module + "." + project.Name
                                   select fullName));

            return result;
        }
    }
}