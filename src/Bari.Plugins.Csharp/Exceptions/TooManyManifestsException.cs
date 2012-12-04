using System;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.Exceptions
{
    /// <summary>
    /// Exception thrown if there are more than one manifest files for a project
    /// </summary>
    public class TooManyManifestsException: Exception
    {
        private readonly Project project;

        /// <summary>
        /// Creates the exception
        /// </summary>
        /// <param name="project">Project which has too many manifest files</param>
        public TooManyManifestsException(Project project)
        {
            this.project = project;
        }

        public override string ToString()
        {
            return
                String.Format("Project {0} has more than one manifest files, which is ambiguous: {1}",
                              project.Name,
                              String.Join(", ", project.GetSourceSet("manifest").Files));
        }
    }
}