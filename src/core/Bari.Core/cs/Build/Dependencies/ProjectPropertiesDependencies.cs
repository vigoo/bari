using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.UI;

namespace Bari.Core.Build.Dependencies
{
    /// <summary>
    /// Represents dependency on one or more properties of a <see cref="Project"/>
    /// </summary>
    public class ProjectPropertiesDependencies: DependenciesBase
    {
        private readonly Project project;
        private readonly ISet<string> properties;

        /// <summary>
        /// Defines the dependency
        /// </summary>
        /// <param name="project">The project having the dependent properties</param>
        /// <param name="properties">Name of the properties within the project</param>
        public ProjectPropertiesDependencies(Project project, params  string[] properties)
        {
            this.project = project;
            this.properties = new HashSet<string>(properties);
        }

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        protected override IDependencyFingerprint CreateFingerprint()
        {
            return new ObjectPropertiesFingerprint(project, properties);
        }

        public override void Dump(IUserOutput output)
        {
            output.Message("Project {0}.{1}'s properties:", project.Module.Name, project.Name);
            output.Indent();
            try
            {
                foreach (var prop in properties)
                {
                    output.Message(prop);
                }
            }
            finally
            {
                output.Unindent();
            }
        }
    }
}