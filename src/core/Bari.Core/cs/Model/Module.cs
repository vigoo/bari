using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Generic;

namespace Bari.Core.Model
{
    /// <summary>
    /// Represents a module of the suite which consists of several projects
    /// </summary>
    public class Module : IProjectParametersHolder
    {
        private readonly string name;
        private string version;
        
        private readonly Suite suite;

        private readonly IDictionary<string, Project> projects = new Dictionary<string, Project>(
            StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, TestProject> testProjects = new Dictionary<string, TestProject>(
            StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, IProjectParameters> parameters =
            new Dictionary<string, IProjectParameters>();
            
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(name != null);
            Contract.Invariant(projects != null);
            Contract.Invariant(suite != null);
            Contract.Invariant(parameters != null);
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
        /// Gets or sets the module's version
        /// 
        /// <para>To use the suite version in <see cref="EffectiveVersion"/>, set this property to <c>null</c>.</para>
        /// </summary>
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Gets the module's version or the suite version if no module specific version was specified
        /// </summary>
        public string EffectiveVersion
        {
            get { return version ?? suite.Version; }
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
        public IEnumerable<TestProject> TestProjects
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
                
                return suite.SuiteRoot.GetChildDirectory("src").GetChildDirectory(name);
            }
        }

        /// <summary>
        /// Gets the suite this module belongs to
        /// </summary>
        public Suite Suite
        {
            get { return suite; }
        }

        /// <summary>
        /// Creates the module instance
        /// </summary>
        /// <param name="name">The name of the module</param>
        /// <param name="suite">Suite the module belongs to</param>
        public Module(string name, Suite suite)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(suite != null);

            this.name = name;
            this.suite = suite;
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
        public TestProject GetTestProject(string testProjectName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(testProjectName));
            Contract.Ensures(Contract.Result<Project>() != null);
            Contract.Ensures(String.Equals(Contract.Result<Project>().Name, testProjectName, StringComparison.InvariantCultureIgnoreCase));
            Contract.Ensures(testProjects.ContainsKey(testProjectName));

            TestProject result;
            if (testProjects.TryGetValue(testProjectName, out result))
                return result;
            else
            {
                result = new TestProject(testProjectName, this);
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

        /// <summary>
        /// Returns a project or test project with the given name, or returns <c>null</c>
        /// if there is no such project in this module.
        /// </summary>
        /// <param name="projectName">Project name to look for</param>
        /// <returns>Returns a project or test project with the given name, or <c>null</c></returns>
        public Project GetProjectOrTestProject(string projectName)
        {
            if (HasProject(projectName))
                return GetProject(projectName);
            else if (HasTestProject(projectName))
                return GetTestProject(projectName);
            else
                return null;
        }

        /// <summary>
        /// Checks whether a parameter block exist with the given name
        /// </summary>
        /// <param name="paramsName">Name of the parameter block</param>
        /// <returns>Returns <c>true</c> if a parameter block with the given name is applied to this model item</returns>
        public bool HasParameters(string paramsName)
        {
            return parameters.ContainsKey(paramsName) || suite.HasParameters(paramsName);
        }

        /// <summary>
        /// Gets a parameter block by its name
        /// </summary>
        /// <typeparam name="TParams">The expected type of the parameter block</typeparam>
        /// <param name="paramsName">Name of the parameter block</param>
        /// <returns>Returns the parameter block</returns>
        public TParams GetParameters<TParams>(string paramsName)
            where TParams : IProjectParameters
        {
            IProjectParameters result;
            if (!parameters.TryGetValue(paramsName, out result))
                return suite.GetParameters<TParams>(paramsName);
            else
                return (TParams)result;
        }

        /// <summary>
        /// Adds a new parameter block to this model item
        /// </summary>
        /// <param name="paramsName">Name of the parameter block</param>
        /// <param name="projectParameters">The parameter block to be added</param>
        public void AddParameters(string paramsName, IProjectParameters projectParameters)
        {
            parameters.Add(paramsName, projectParameters);
        }

        public override string ToString()
        {
            return String.Format("Module {0}", name);
        }
    }
}