using System;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio
{
    /// <summary>
    /// Interface for project-guid mappers
    /// 
    /// <para>VisualStudio requires a GUID for every project it works with.</para>
    /// </summary>
    public interface IProjectGuidManagement
    {
        /// <summary>
        /// Gets the GUID associated with the given project
        /// </summary>
        /// <param name="project">The bari project model</param>
        /// <returns>Always returns the same GUID for the same project within one process execution.</returns>
        Guid GetGuid(Project project);
    }
}