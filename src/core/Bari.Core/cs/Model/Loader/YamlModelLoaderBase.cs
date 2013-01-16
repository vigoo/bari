using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Exceptions;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Base class for YAML based loaders 
    /// </summary>
    public abstract class YamlModelLoaderBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(YamlModelLoaderBase));

        private readonly Func<Suite> suiteFactory;
        private readonly IEnumerable<IYamlProjectParametersLoader> parametersLoaders;

        /// <summary>
        /// Initializes the yaml loader
        /// </summary>
        /// <param name="suiteFactory">Factory method to create new suite instances</param>
        /// <param name="parametersLoaders">Parameter loader implementations</param>
        protected YamlModelLoaderBase(Func<Suite> suiteFactory, IEnumerable<IYamlProjectParametersLoader> parametersLoaders)
        {
            Contract.Requires(suiteFactory != null);
            Contract.Ensures(this.suiteFactory == suiteFactory);
            Contract.Ensures(this.parametersLoaders == parametersLoaders);

            this.suiteFactory = suiteFactory;
            this.parametersLoaders = parametersLoaders;            
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

            var suite = suiteFactory();

            suite.Name = GetScalarValue(yaml.RootNode, "suite", "Error reading the name of the suite");
            suite.Version = GetOptionalScalarValue(yaml.RootNode, "version", null);

            foreach (KeyValuePair<string, YamlNode> item in EnumerateNamedNodesOf(yaml.RootNode, "modules"))
            {
                var module = suite.GetModule(item.Key);

                if (item.Value != null)
                    LoadModule(module, item.Value);
            }

            foreach (KeyValuePair<string, YamlNode> item in EnumerateNamedNodesOf(yaml.RootNode, "products"))
            {
                var product = suite.GetProduct(item.Key);

                if (item.Value != null)
                    LoadProduct(suite, product, item.Value);
            }
            
            LoadParameters(suite, yaml.RootNode);

            log.Debug("Finished processing YAML document.");
            return suite;
        }

        private void LoadParameters(IProjectParametersHolder target, YamlNode node)
        {
            var mapping = node as YamlMappingNode;
            if (mapping != null)
            {
                foreach (var pair in mapping)
                    TryAddParameters(target, pair.Key, pair.Value);
            }
        }

        private void LoadProduct(Suite suite, Product product, YamlNode productNode)
        {
            Contract.Requires(product != null);
            Contract.Requires(productNode != null);

            foreach (KeyValuePair<string, YamlNode> item in EnumerateNamedNodesOf(productNode, "modules"))
            {
                var module = suite.GetModule(item.Key);
                product.AddModule(module);
            }
        }

        private void LoadModule(Module module, YamlNode moduleNode)
        {
            Contract.Requires(module != null);
            Contract.Requires(moduleNode != null);

            LoadModuleVersion(module, moduleNode);
            LoadProjects(module, moduleNode);
            LoadTestProjects(module, moduleNode);
            LoadParameters(module, moduleNode);
        }

        private void LoadModuleVersion(Module module, YamlNode moduleNode)
        {
            module.Version = GetOptionalScalarValue(moduleNode, "version", null);
        }

        private void LoadTestProjects(Module module, YamlNode moduleNode)
        {
            foreach (KeyValuePair<string, YamlNode> item in EnumerateNamedNodesOf(moduleNode, "tests"))
            {
                var testProject = module.GetTestProject(item.Key);

                if (item.Value != null)
                    LoadProject(testProject, item.Value);
            }
        }

        private void LoadProjects(Module module, YamlNode moduleNode)
        {
            foreach (KeyValuePair<string, YamlNode> item in EnumerateNamedNodesOf(moduleNode, "projects"))
            {
                var project = module.GetProject(item.Key);

                if (item.Value != null)
                    LoadProject(project, item.Value);
            }
        }

        private void LoadProject(Project project, YamlNode projectNode)
        {
            Contract.Requires(project != null);
            Contract.Requires(projectNode != null);

            var mapping = projectNode as YamlMappingNode;
            if (mapping != null)
            {
                foreach (var pair in mapping)
                {
                    if (new YamlScalarNode("type").Equals(pair.Key) &&
                        pair.Value is YamlScalarNode)
                    {
                        SetProjectType(project, ((YamlScalarNode) pair.Value).Value);
                    }
                    else if (new YamlScalarNode("version").Equals(pair.Key) &&
                    pair.Value is YamlScalarNode)
                    {
                        project.Version = ((YamlScalarNode) pair.Value).Value;
                    }
                    else if (new YamlScalarNode("references").Equals(pair.Key) &&
                        pair.Value is YamlSequenceNode)
                    {
                        SetProjectReferences(project, ((YamlSequenceNode)pair.Value).Children);
                    }
                    else
                    {
                        TryAddParameters(project, pair.Key, pair.Value);
                    }
                }
            }
        }

        private void SetProjectReferences(Project project, IEnumerable<YamlNode> referenceNodes)
        {
            Contract.Requires(project != null);
            Contract.Requires(referenceNodes!= null);

            foreach (var referenceNode in referenceNodes)
            {
                var scalar = referenceNode as YamlScalarNode;
                if (scalar != null)
                {
                    var uri = scalar.Value;
                    project.AddReference(new Reference(new Uri(uri)));
                }
            }
        }

        private void SetProjectType(Project project, string typeString)
        {
            Contract.Requires(project != null);
            Contract.Requires(typeString != null);

            switch (typeString.ToLowerInvariant())
            {
                case "executable":
                    project.Type = ProjectType.Executable;
                    break;
                default:
                    project.Type = ProjectType.Library;
                    break;
            }
        }

        private IEnumerable<KeyValuePair<string, YamlNode>> EnumerateNamedNodesOf(YamlNode parent, string groupName)
        {
            Contract.Requires(parent != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(groupName));
            Contract.Ensures(Contract.Result<IEnumerable<YamlNode>>() != null);

            var mapping = parent as YamlMappingNode;
            if (mapping != null)
            {
                var groupNameNode = new YamlScalarNode(groupName);
                if (mapping.Children.ContainsKey(groupNameNode))
                {
                    var groupSeq = mapping.Children[groupNameNode] as YamlSequenceNode;

                    if (groupSeq != null)
                    {
                        foreach (var item in groupSeq.Children)
                        {
                            if (item is YamlScalarNode)
                            {
                                yield return new KeyValuePair<string, YamlNode>(((YamlScalarNode) item).Value, null);
                            }
                            else if (item is YamlMappingNode)
                            {
                                var mappingChild = (YamlMappingNode)item;
                                yield return new KeyValuePair<string, YamlNode>(
                                    GetScalarValue(mappingChild, "name"),
                                    mappingChild);
                            }
                        }
                    }
                }
            }
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

        private string GetOptionalScalarValue(YamlNode parent, string key, string defaultValue)
        {
            Contract.Requires(parent != null);
            Contract.Requires(key != null);
            Contract.Ensures(Contract.Result<string>() != null || Contract.Result<string>() == defaultValue);

            try
            {
                var mapping = (YamlMappingNode)parent;
                var child = mapping.Children[new YamlScalarNode(key)];

                if (child == null)
                    return defaultValue;

                string value = ((YamlScalarNode)child).Value;
                if (value == null)
                    return defaultValue;

                return value;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        private void TryAddParameters(IProjectParametersHolder target, YamlNode key, YamlNode value)
        {
            if (parametersLoaders != null)
            {
                var scalarKey = key as YamlScalarNode;

                if (scalarKey != null)
                {
                    string name = scalarKey.Value;
                    var loader = parametersLoaders.FirstOrDefault(l => l.Supports(name));
                    if (loader != null)
                    {
                        var param = loader.Load(name, value);
                        target.AddParameters(name, param);
                    }
                }
            }
        }
    }
}