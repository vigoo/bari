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

        private readonly IDictionary<string, Project> testProjects = new Dictionary<string, Project>(
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
            Contract.Invariant(Contract.ForAll(testProjects,
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
        /// GEts all the module's test projects
        /// </summary>
        public IEnumerable<Project> TestProjects
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Project>>() != null);

                return testProjects.Values;
            }
        }

        /// <summary>
        /// Gets the root directory of the module
        /// </summary>
        public IFileSystemDirectory RootDirectory
        {
            get
            {
                Contract.Ensures(Contract.Result<IFileSystemDirectory>() != null);
                
                return suiteRoot.GetChildDirectory("src").GetChildDirectory(name);
            }
        }

        /// <summary>
        /// Creates the module instance
        /// </summary>
        /// <param name="name">The name of the module</param>
        /// <param name="suiteRoot">Root directory of the suite</param>
        public Module(string name, [SuiteRoot] IFileSystemDirectory suiteRoot)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(suiteRoot != null);

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
        /// Gets a test project model by its name, or creates and adds it if it was not registered yet.
        /// </summary>
        /// <param name="testProjectName">Name of the test project</param>
        /// <returns>Returns the test project model for the given test project name</returns>
        public Project GetTestProject(string testProjectName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(testProjectName));
            Contract.Ensures(Contract.Result<Project>() != null);
            Contract.Ensures(String.Equals(Contract.Result<Project>().Name, testProjectName, StringComparison.InvariantCultureIgnoreCase));
            Contract.Ensures(testProjects.ContainsKey(testProjectName));

            Project result;
            if (testProjects.TryGetValue(testProjectName, out result))
                return result;
            else
            {
                result = new Project(testProjectName, this);
                testProjects.Add(testProjectName, result);
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

        /// <summary>
        /// Returns true if a test project with the given name belongs to this module
        /// </summary>
        /// <param name="testProjectName">Project name to look for</param>
        /// <returns>Returns <c>true</c> if the module has the given test project</returns>
        public bool HasTestProject(string testProjectName)
        {
            return testProjects.ContainsKey(testProjectName);
        }
    }
}