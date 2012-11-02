using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Bari.Core.Model
{
    /// <summary>
    /// Represents a project of a module, which is a separate processable set of inputs creating one or more targets
    /// 
    /// <example>An example is a set of C# sources compiled into one assembly.</example>
    /// </summary>
    public class Project
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (Project));

        private readonly Module module;
        private readonly string name;
        private readonly IDictionary<string, SourceSet> sourceSets = new Dictionary<string, SourceSet>();
        private readonly ISet<Reference> references = new HashSet<Reference>();
        private ProjectType type = ProjectType.Library;

        /// <summary>
        /// Gets the module this project belongs to
        /// </summary>
        public Module Module
        {
            get { return module; }
        }

        /// <summary>
        /// Gets the project's name
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
        /// Gets or sets the project type
        /// </summary>
        public ProjectType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets the source sets belonging to this project
        /// </summary>
        public IEnumerable<SourceSet> SourceSets
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<SourceSet>>() != null);

                return sourceSets.Values;
            }
        }

        /// <summary>
        /// Gets the project references
        /// </summary>
        public IEnumerable<Reference> References
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Reference>>() != null);

                return references;
            }
        }

        /// <summary>
        /// Constructs the project model instance
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="module">The module the project belongs to</param>
        public Project(string name, Module module)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(module != null);

            this.name = name;
            this.module = module;
        }

        /// <summary>
        /// Gets a source set with the given type.
        /// 
        /// <para>If the requested source set does not exist yet, it is created as an empty set
        /// and added to the project.</para>
        /// </summary>
        /// <param name="setType">Source set type name</param>
        /// <returns>Returns the requested source set. Never returns <c>null</c>.</returns>
        public SourceSet GetSourceSet(string setType)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(setType));
            Contract.Ensures(Contract.Result<SourceSet>() != null);
            Contract.Ensures(Contract.Result<SourceSet>().Type == setType);
            Contract.Ensures(sourceSets.ContainsKey(setType));

            SourceSet result;
            if (!sourceSets.TryGetValue(setType, out result))
            {
                result = new SourceSet(setType);
                sourceSets.Add(setType, result);
            }

            Contract.Assume(result != null);
            return result;
        }

        /// <summary>
        /// Checks if the project has a source set with the given type and at least one files
        /// </summary>
        /// <param name="setType">The source set type name</param>
        /// <returns>Returns <c>true</c> if there is a source set with the given type name and at least one files</returns>
        public bool HasNonEmptySourceSet(string setType)
        {
            return sourceSets.ContainsKey(setType) &&
                   sourceSets[setType].Files.Any();
        }

        /// <summary>
        /// Adds a new reference to the set of project references
        /// </summary>
        /// <param name="reference">The new reference to be added</param>
        public void AddReference(Reference reference)
        {
            Contract.Requires(reference != null);
            Contract.Ensures(References.Contains(reference));

            references.Add(reference);
        }
    }
}