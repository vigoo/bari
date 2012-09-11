using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Bari.Core.Model
{
    /// <summary>
    /// Represents a module of the suite which consists of several projects
    /// </summary>
    public class Module
    {
        private readonly string name;
        private readonly IDictionary<string, Project> projects = new Dictionary<string, Project>();


        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(name != null);
            Contract.Invariant(projects != null);
            Contract.Invariant(Contract.ForAll(projects,
                                               pair =>
                                               !string.IsNullOrWhiteSpace(pair.Key) &&
                                               pair.Value != null &&
                                               pair.Value.Name == pair.Key));
        }

        /// <summary>
        /// Gets the module's name
        /// </summary>
        public string Name
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return name;
            }
        }

        /// <summary>
        /// Gets all the module's projects
        /// </summary>
        public IEnumerable<Project> Projects
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Project>>() != null);

                return projects.Values;
            }
        }

        /// <summary>
        /// Creates the module instance
        /// </summary>
        /// <param name="name">The name of the module</param>
        public Module(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            this.name = name;
        }

        /// <summary>
        /// Gets a project model by its name, or creates and adds it if it was not registered yet.
        /// </summary>
        /// <param name="projectName">Name of the project</param>
        /// <returns>Returns the project model for the given project name</returns>
        public Project GetProject(string projectName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(projectName));
            Contract.Ensures(Contract.Result<Project>() != null);
            Contract.Ensures(projects.ContainsKey(projectName));

            Project result;
            if (projects.TryGetValue(projectName, out result))
                return result;
            else
            {
                result = new Project(projectName);
                projects.Add(projectName, result);
                return result;
            }
        }
    }
}