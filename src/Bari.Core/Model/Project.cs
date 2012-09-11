using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Bari.Core.Model
{
    /// <summary>
    /// Represents a project of a module, which is a separate processable set of inputs creating one or more targets
    /// 
    /// <example>An example is a set of C# sources compiled into one assembly.</example>
    /// </summary>
    public class Project
    {
        private readonly string name;
        private readonly IDictionary<string, SourceSet> sourceSets = new Dictionary<string, SourceSet>();

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
        /// Constructs the project model instance
        /// </summary>
        /// <param name="name">Name of the project</param>
        public Project(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            this.name = name;
        }

        /// <summary>
        /// Gets a source set with the given type.
        /// 
        /// <para>If the requested source set does not exist yet, it is created as an empty set
        /// and added to the project.</para>
        /// </summary>
        /// <param name="type">Source set type name</param>
        /// <returns>Returns the requested source set. Never returns <c>null</c>.</returns>
        public SourceSet GetSourceSet(string type)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(type));
            Contract.Ensures(Contract.Result<SourceSet>() != null);
            Contract.Ensures(Contract.Result<SourceSet>().Type == type);
            Contract.Ensures(sourceSets.ContainsKey(type));

            SourceSet result;
            if (!sourceSets.TryGetValue(type, out result))
            {
                result = new SourceSet(type);
                sourceSets.Add(type, result);
            }

            return result;
        }
    }
}