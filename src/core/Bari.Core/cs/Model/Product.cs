using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Bari.Core.Model
{
    /// <summary>
    /// A product is a named subset of a suite's modules
    /// </summary>
    public class Product : IPostProcessorsHolder
    {
        private readonly string name;
        private readonly ISet<Module> modules;
        private readonly IDictionary<string, PostProcessDefinition> postProcessDefinitions = new Dictionary<string, PostProcessDefinition>();

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
        /// Gets the postprocessors associated with this product
        /// </summary>
        public IEnumerable<PostProcessDefinition> PostProcessors
        {
            get { return postProcessDefinitions.Values; }
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

        /// <summary>
        /// Adds a new postprocessor to this product
        /// </summary>
        /// <param name="postProcessDefinition">Post processor type and parameters</param>
        public void AddPostProcessor(PostProcessDefinition postProcessDefinition)
        {
            postProcessDefinitions.Add(postProcessDefinition.Name, postProcessDefinition);
        }

        /// <summary>
        /// Gets a registered post processor by its name
        /// </summary>
        /// <param name="key">Name of the post processor</param>
        /// <returns>Returns the post processor definition</returns>
        public PostProcessDefinition GetPostProcessor(string key)
        {
            return postProcessDefinitions[key];
        }
    }
}