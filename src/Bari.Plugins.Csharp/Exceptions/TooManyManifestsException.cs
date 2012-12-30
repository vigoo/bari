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

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/></PermissionSet>
        public override string ToString()
        {
            return
                String.Format("Project {0} has more than one manifest files, which is ambiguous: {1}",
                              project.Name,
                              String.Join(", ", project.GetSourceSet("manifest").Files));
        }
    }
}