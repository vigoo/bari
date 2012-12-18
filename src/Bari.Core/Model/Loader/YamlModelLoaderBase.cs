using System;
using System.Collections.Generic;
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
            suite.Version = GetOptionalScalarValue(yaml.RootNode, "version", null);

            foreach (KeyValuePair<string, YamlNode> item in EnumerateNamedNodesOf(yaml.RootNode, "modules"))
            {
                var module = suite.GetModule(item.Key);

                if (item.Value != null)
                    LoadModule(module, item.Value);
            }

            log.Debug("Finished processing YAML document.");
            return suite;
        }

        private void LoadModule(Module module, YamlNode moduleNode)
        {
            Contract.Requires(module != null);
            Contract.Requires(moduleNode != null);

            module.Version = GetOptionalScalarValue(moduleNode, "version", null);

            foreach (KeyValuePair<string, YamlNode> item in EnumerateNamedNodesOf(moduleNode, "projects"))
            {
                var project = module.GetProject(item.Key);

                if (item.Value != null)
                    LoadProject(project, item.Value);
            }

            foreach (KeyValuePair<string, YamlNode> item in EnumerateNamedNodesOf(moduleNode, "tests"))
            {
                var testProject = module.GetTestProject(item.Key);

                if (item.Value != null)
                    LoadProject(testProject, item.Value);
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
                }
            }
        }

        private void SetProjectReferences(Project project, IEnumerable<YamlNode> referenceNodes)
        {
            Contract.Requires(project != null);
            Contract.Requires(referenceNodes!= null);

            foreach (var referenceNode in referenceNodes)
            {
                if (referenceNode is YamlScalarNode)
                {
                    var uri = ((YamlScalarNode) referenceNode).Value;
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
            catch (Exception ex)
            {
                return defaultValue;
            }
        }
    }
}