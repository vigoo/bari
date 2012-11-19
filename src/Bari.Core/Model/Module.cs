using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Generic;

namespace Bari.Core.Model
{
    /// <summary>
    /// Represents a module of the suite which consists of several projects
    /// </summary>
    public class Module
    {
        private readonly string name;
        private readonly IFileSystemDirectory suiteRoot;

        private readonly IDictionary<string, Project> projects = new Dictionary<string, Project>(
            StringComparer.InvariantCultureIgnoreCase);        

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(name != null);
            Contract.Invariant(projects != null);
            Contract.Invariant(suiteRoot != null);
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
        /// Gets the root directory of the module
        /// </summary>
        public IFileSystemDirectory RootDirectory
        {
            get { return suiteRoot.GetChildDirectory("src").GetChildDirectory(name); }
        }

        /// <summary>
        /// Creates the module instance
        /// </summary>
        /// <param name="name">The name of the module</param>
        /// <param name="suiteRoot">Root directory of the suite</param>
        public Module(string name, [SuiteRoot] IFileSystemDirectory suiteRoot)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            this.name = name;
            this.suiteRoot = suiteRoot;
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
            Contract.Ensures(String.Equals(Contract.Result<Project>().Name, projectName, StringComparison.InvariantCultureIgnoreCase));
            Contract.Ensures(projects.ContainsKey(projectName));

            Project result;
            if (projects.TryGetValue(projectName, out result))
                return result;
            else
            {
                result = new Project(projectName, this);
                projects.Add(projectName, result);
                return result;
            }
        }

        /// <summary>
        /// Returns true if a project with the given name belongs to this module
        /// </summary>
        /// <param name="projectName">Project name to look for</param>
        /// <returns>Returns <c>true</c> if the module has the given project</returns>
        public bool HasProject(string projectName)
        {
            return projects.ContainsKey(projectName);
        }
    }
}