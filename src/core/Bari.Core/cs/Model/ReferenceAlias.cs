using System.Collections.Generic;

namespace Bari.Core.Model
{
    /// <summary>
    /// Holds an alias to a set of references (<see cref="Reference"/>)
    /// </summary>
    public class ReferenceAlias
    {
        private readonly string name;
        private readonly ISet<Reference> references;

        /// <summary>
        /// Gets the name of the alias
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the set of references
        /// </summary>
        public ISet<Reference> References
        {
            get { return references; }
        }

        /// <summary>
        /// Creates a new alias
        /// </summary>
        /// <param name="name">Name of the alias</param>
        /// <param name="refs">Set of references this alias is for</param>
        public ReferenceAlias(string name, IEnumerable<Reference> refs)
        {
            this.name = name;
            references = new HashSet<Reference>(refs);
        }
    }
}