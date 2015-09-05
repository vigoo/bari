using System;
using System.Collections.Generic;
using Bari.Core.Model.Parameters;

namespace Bari.Core.Model
{
    /// <summary>
    /// Project parameter block holding a set of reference aliases (<see cref="ReferenceAlias"/>)
    /// </summary>
    public class ReferenceAliases: IProjectParameters
    {
        private readonly IDictionary<string, ReferenceAlias> aliases = new Dictionary<string, ReferenceAlias>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets the list of registered alias names
        /// </summary>
        public IEnumerable<string> Names
        {
            get { return aliases.Keys; }
        }

        /// <summary>
        /// Gets a reference alias by its name
        /// </summary>
        /// <param name="name">Name of the alias</param>
        /// <returns>Returns the reference alias or <c>null</c> if it is undefined</returns>
        public ReferenceAlias Get(string name)
        {
            ReferenceAlias alias;
            if (aliases.TryGetValue(name, out alias))
                return alias;
            else
                return null;
        }

        /// <summary>
        /// Adds a new reference alias to the parameter block
        /// </summary>
        /// <param name="name">Name of the alias</param>
        /// <param name="references">Set of references the alias is for</param>
        public void Add(string name, IEnumerable<Reference> references)
        {
            var alias = new ReferenceAlias(name, references);
            aliases.Add(name, alias);
        }
    }
}