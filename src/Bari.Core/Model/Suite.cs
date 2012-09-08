using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Bari.Core.Model
{
    /// <summary>
    /// Suite is the root item in the object model describing an application suite
    /// </summary>
    public class Suite
    {
        private string name = string.Empty;
        private readonly IDictionary<string, Module> modules = new Dictionary<string, Module>();

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
        /// Gets all the suite's modules
        /// </summary>
        public IEnumerable<Module> Modules
        {
            get { return modules.Values; }
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

            Module result;
            if (modules.TryGetValue(moduleName, out result))
                return result;
            else
            {
                result = new Module(moduleName);
                modules.Add(moduleName, result);
                return result;
            }
        }
    }
}