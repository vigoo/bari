using System;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.Exceptions
{
    /// <summary>
    /// Exception thrown if there are more than one app.config file for a project
    /// </summary>
    public class TooManyAppConfigsException: Exception
    {
        private readonly Project project;

        /// <summary>
        /// Creates the exception
        /// </summary>
        /// <param name="project">Project which has too many app.config files</param>
        public TooManyAppConfigsException(Project project)
        {
            this.project = project;
        }

        public override string ToString()
        {
            return
                String.Format("Project {0} has more than one application configuration files, which is ambiguous: {1}",
                              project.Name,
                              String.Join(", ", project.GetSourceSet("appconfig").Files));
        }
    }
}