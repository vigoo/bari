using System;
using System.Diagnostics.Contracts;
using Bari.Core.Exceptions;
using Ninject;
using Ninject.Syntax;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Base class for YAML based loaders 
    /// </summary>
    public abstract class YamlModelLoaderBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (YamlModelLoaderBase));

        private readonly IResolutionRoot root;

        /// <summary>
        /// Initializes the yaml loader
        /// </summary>
        /// <param name="root">Path to resolve instances</param>
        protected YamlModelLoaderBase(IResolutionRoot root)
        {
            Contract.Requires(root != null);
            Contract.Ensures(this.root == root);

            this.root = root;
        }

        /// <summary>
        /// Process an already loaded <c>YamlDocument</c> and returns the loaded suite model.
        /// </summary>
        /// <param name="yaml">The yaml document to process.</param>
        /// <returns>Returns a loaded model if succeeds. On error it throws an exception, never
        /// returns <c>null</c>.</returns>
        protected Suite LoadYaml(YamlDocument yaml)
        {
            Contract.Requires(yaml != null);
            Contract.Requires(yaml.RootNode != null);
            Contract.Ensures(Contract.Result<Suite>() != null);

            log.Debug("Processing YAML document...");

            var suite = root.Get<Suite>();

            suite.Name = GetScalarValue(yaml.RootNode, "suite", "Error reading the name of the suite");

            log.Debug("Finished processing YAML document.");
            return suite;
        }

        private string GetScalarValue(YamlNode parent, string key, string errorMessage = null)
        {
            Contract.Requires(parent != null);
            Contract.Requires(key != null);
            Contract.Ensures(Contract.Result<string>() != null);            

            try
            {
                var mapping = (YamlMappingNode) parent;
                var child = mapping.Children[new YamlScalarNode(key)];

                if (child == null)
                    throw new InvalidSpecificationException(String.Format("Parent has no child with key {0}", key));

                string value = ((YamlScalarNode) child).Value;
                if (value == null)
                    throw new InvalidSpecificationException(errorMessage ?? String.Format("No value for key {0}", key));

                return value;
            }
            catch (Exception ex)
            {
                throw new InvalidSpecificationException(errorMessage ?? ex.Message, ex);
            }
        }
    }
}