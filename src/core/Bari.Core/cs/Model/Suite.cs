using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Model.Parameters;
using Bari.Core.UI;

namespace Bari.Core.Model
{
    /// <summary>
    /// Suite is the root item in the object model describing an application suite
    /// </summary>
    public class Suite : IProjectParametersHolder
    {
        public static readonly Goal DebugGoal = new Goal("debug");
        public static readonly Goal ReleaseGoal = new Goal("release");

        private string name = string.Empty;
        private string version;
        private string copyright;
        private readonly IDictionary<string, Module> modules = new Dictionary<string, Module>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, Product> products = new Dictionary<string, Product>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, IProjectParameters> parameters = new Dictionary<string, IProjectParameters>();
        private readonly IDictionary<string, Goal> goals = new Dictionary<string, Goal>();
        private readonly SourceSetIgnoreLists sourceSetIgnoreLists = new SourceSetIgnoreLists();
        private readonly IFileSystemDirectory suiteRoot;

        private readonly Goal activeGoal;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(name != null);
            Contract.Invariant(modules != null);
            Contract.Invariant(products != null);
            Contract.Invariant(suiteRoot != null);
            Contract.Invariant(parameters != null);
            Contract.Invariant(goals != null);
            Contract.Invariant(Contract.ForAll(modules,
                                               pair =>
                                               !string.IsNullOrWhiteSpace(pair.Key) && 
                                               pair.Value != null &&
                                               pair.Value.Name == pair.Key));
            Contract.Invariant(Contract.ForAll(products,
                                               pair =>
                                               !string.IsNullOrWhiteSpace(pair.Key) &&
                                               pair.Value != null &&
                                               pair.Value.Name == pair.Key));
            Contract.Invariant(Contract.ForAll(goals,
                                   pair =>
                                   !string.IsNullOrWhiteSpace(pair.Key) &&
                                   pair.Value != null &&
                                   pair.Value.Name == pair.Key));
            Contract.Invariant(activeGoal != null  && goals.ContainsKey(activeGoal.Name));
            Contract.Invariant(sourceSetIgnoreLists != null);
        }

        /// <summary>
        /// Gets or sets the suite's name
        /// </summary>
        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                
                return name;
            }
            set
            {
                Contract.Requires(value != null);
                
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the suite's version 
        /// 
        /// <para>Can be <c>null</c>, in this case there is no suite-wise version specified.</para>
        /// </summary>
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Gets or sets the suite's copyright
        /// 
        /// <para>Can be <c>null</c>, in this case there is no suite-wise copyright specified.</para>
        /// </summary>
        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        /// <summary>
        /// Gets the suite's root directory
        /// </summary>
        public IFileSystemDirectory SuiteRoot
        {
            get { return suiteRoot; }
        }

        /// <summary>
        /// Creates the suite definition
        /// </summary>
        /// <param name="suiteRoot">Root directory of the suite</param>
        public Suite([SuiteRoot] IFileSystemDirectory suiteRoot)
            : this(suiteRoot, new Goal[] { DebugGoal, ReleaseGoal }, DebugGoal)
        {
        }

        /// <summary>
        /// Creates the suite definition
        /// </summary>
        /// <param name="suiteRoot">Root directory of the suite</param>
        /// <param name="goals">The set of goals available for the suite</param>
        /// <param name="activeGoal">The goal selected for this run</param>
        public Suite([SuiteRoot] IFileSystemDirectory suiteRoot, IEnumerable<Goal> goals, Goal activeGoal)
        {
            Contract.Requires(suiteRoot != null);
            Contract.Requires(goals != null);
            Contract.Requires(activeGoal != null);
            Contract.Requires(goals.Contains(activeGoal));

            this.suiteRoot = suiteRoot;
            foreach (var goal in goals)
                this.goals.Add(goal.Name, goal);
            this.activeGoal = activeGoal;
        }

        /// <summary>
        /// Gets all the suite's modules
        /// </summary>
        public IEnumerable<Module> Modules
        {
            get { return modules.Values; }
        }

        /// <summary>
        /// Get all the products of the suite
        /// </summary>
        public IEnumerable<Product> Products
        {
            get { return products.Values; }
        }

        /// <summary>
        /// Get all the goals defined for this suite
        /// 
        /// <para>This information can be used to help the user selecting another goal,
        /// but otherwise the active goal is selected before loading the suite and all
        /// the suite model is loaded according to the selected goal.</para>
        /// </summary>
        public IEnumerable<Goal> Goals
        {
            get { return goals.Values; }
        }

        /// <summary>
        /// Gets the active goal.
        /// 
        /// <para>The active goal is selected before the suite model has been loaded,
        /// and all the model data are loaded according the active goal. This means that
        /// the active goal cannot be changed after the suite has been loaded.</para>
        /// </summary>
        public Goal ActiveGoal
        {
            get { return activeGoal; }
        }

        /// <summary>
        /// Gets a module's model and creates it if it is not registered yet
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <returns>Returns a module instance</returns>
        public Module GetModule(string moduleName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(moduleName));
            Contract.Ensures(Contract.Result<Module>() != null);
            Contract.Ensures(String.Equals(Contract.Result<Module>().Name, moduleName, StringComparison.InvariantCultureIgnoreCase));
            Contract.Ensures(modules.ContainsKey(moduleName));

            Module result;
            if (modules.TryGetValue(moduleName, out result))
                return result;
            else
            {
                result = new Module(moduleName, this);
                modules.Add(moduleName, result);
                return result;
            }
        }

        /// <summary>
        /// Checks whether the suite has a module with the given name or not
        /// </summary>
        /// <param name="moduleName">Name of the module to look for</param>
        /// <returns>Returns <c>true</c> if the suite already has a module with the given name.</returns>
        public bool HasModule(string moduleName)
        {
            return modules.ContainsKey(moduleName);
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
        /// Gets a possibly inherited parameter block by its name. If the block does not exists, a default
        /// block is returned.
        /// </summary>
        /// <typeparam name="TParams">Parameter block type</typeparam>
        /// <typeparam name="TParamsDef">Inheritable parameter block definition ype</typeparam>
        /// <param name="paramsName">Name of the parameter block</param>
        /// <returns>Returns the composed parameter block</returns>
        public TParams GetInheritableParameters<TParams, TParamsDef>(string paramsName) 
            where TParamsDef : ProjectParametersPropertyDefs<TParams>, new()
            where TParams : InheritableProjectParameters<TParams, TParamsDef>
        {
            return (TParams) GetInheritableParameters(paramsName, new TParamsDef());
        }

        public IInheritableProjectParameters GetInheritableParameters(string paramsName, IInheritableProjectParametersDef defs)
        {
            if (parameters.ContainsKey(paramsName))
                return (IInheritableProjectParameters) parameters[paramsName];
            else
            {
                return defs.CreateDefault(this, null);
            }
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

        /// <summary>
        /// Checks whether the suite has a product with the given name
        /// </summary>
        /// <param name="productName">Name of the product to look for</param>
        /// <returns>Returns <c>true</c> if the suite has a product with the given name registered.</returns>
        public bool HasProduct(string productName)
        {
            return products.ContainsKey(productName);
        }

        /// <summary>
        /// Gets a product with the given name, or creates a new one if it does not exist yet.
        /// </summary>
        /// <param name="productName">Name of the product to be returned</param>
        /// <returns>Returns a product of the suite with the given name.</returns>
        public Product GetProduct(string productName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(productName));
            Contract.Ensures(Contract.Result<Product>() != null);
            Contract.Ensures(String.Equals(Contract.Result<Product>().Name, productName, StringComparison.InvariantCultureIgnoreCase));
            Contract.Ensures(products.ContainsKey(productName));

            Product result;
            if (products.TryGetValue(productName, out result))
                return result;
            else
            {
                result = new Product(productName);
                products.Add(productName, result);
                return result;
            }           
        }

        /// <summary>
        /// Checks for warnings in the suite, and displays them through the given output interface
        /// </summary>
        /// <param name="output">Output interface</param>
        public void CheckForWarnings(IUserOutput output)
        {
            foreach (var module in modules.Values)            
                module.CheckForWarnings(output);

            foreach (var product in products.Values)
                product.CheckForWarnings(output);
        }

        /// <summary>
        /// Gets the source-set ignore lists
        /// </summary>
        public SourceSetIgnoreLists SourceSetIgnoreLists
        {
            get { return sourceSetIgnoreLists; }
        }
    }
}