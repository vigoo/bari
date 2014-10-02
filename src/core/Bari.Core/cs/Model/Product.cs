using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.UI;

namespace Bari.Core.Model
{
    /// <summary>
    /// A product is a named subset of a suite's modules
    /// </summary>
    public class Product : IPostProcessorsHolder, IProjectParametersHolder
    {
        private readonly string name;
        private readonly ISet<Module> modules;
        private readonly IDictionary<string, PostProcessDefinition> postProcessDefinitions = new Dictionary<string, PostProcessDefinition>();
        private readonly IDictionary<string, IProjectParameters> parameters = new Dictionary<string, IProjectParameters>();
        private PackagerDefinition packager;

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
        /// Gets the relative path to the target directory where this post processable item is compiled to
        /// </summary>
        public string RelativeTargetPath
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets the packager definition for this product
        /// </summary>
        public PackagerDefinition Packager
        {
            get { return packager; }
            set { packager = value; }
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

        /// <summary>
        /// Checks for warnings in the product, and displays them through the given output interface
        /// </summary>
        /// <param name="output">Output interface</param>
        public void CheckForWarnings(IUserOutput output)
        {            
        }

        /// <summary>
        /// Checks whether a parameter block exist with the given name
        /// </summary>
        /// <param name="paramsName">Name of the parameter block</param>
        /// <returns>Returns <c>true</c> if a parameter block with the given name is applied to this model item</returns>
        public bool HasParameters(string paramsName)
        {
            return parameters.ContainsKey(paramsName);
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
            return (TParams)parameters[paramsName];
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
            return "Product__" + name;
        }
    }
}