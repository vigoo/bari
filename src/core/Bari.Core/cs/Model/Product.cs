using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Bari.Core.Model
{
    /// <summary>
    /// A product is a named subset of a suite's modules
    /// </summary>
    public class Product
    {
        private readonly string name;
        private readonly ISet<Module> modules;

        /// <summary>
        /// Gets the product's name
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
        /// Gets the modules belonging to this product
        /// </summary>
        public IEnumerable<Module> Modules
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Module>>() != null);
                
                return modules;
            }
        }

        /// <summary>
        /// Creates the product, initially with empty module set
        /// </summary>
        /// <param name="name">Name of the product</param>
        public Product(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            this.name = name;
            modules = new SortedSet<Module>(new ModuleComparer());
        }

        /// <summary>
        /// Adds a module to the product 
        /// </summary>
        /// <param name="module"></param>
        public void AddModule(Module module)
        {
            Contract.Requires(module != null);
            Contract.Ensures(Modules.Contains(module));

            modules.Add(module);
        }
    }
}