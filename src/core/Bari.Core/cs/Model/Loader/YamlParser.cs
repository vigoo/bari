using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Exceptions;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Helper methods for parsing YAML files, with support of goal-based filtering
    /// </summary>
    public class YamlParser
    {
        private Goal activeGoal = Suite.DebugGoal;

        /// <summary>
        /// Finds a child node by its name and enumerates its contents
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="groupName">Name of the group to find</param>
        /// <returns>Returns a sequence of key-node pairs</returns>
        public IEnumerable<KeyValuePair<string, YamlNode>> EnumerateNamedNodesOf(YamlNode parent, string groupName)
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
                                yield return new KeyValuePair<string, YamlNode>(((YamlScalarNode)item).Value, null);
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

        /// <summary>
        /// Gets a scalar value identified by its key
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="key">Key of the value</param>
        /// <param name="errorMessage">Error message if the key does not exists</param>
        /// <returns>Returns the scalar value</returns>
        public string GetScalarValue(YamlNode parent, string key, string errorMessage = null)
        {
            Contract.Requires(parent != null);
            Contract.Requires(key != null);
            Contract.Ensures(Contract.Result<string>() != null);

            try
            {
                var mapping = (YamlMappingNode)parent;
                var child = mapping.Children[new YamlScalarNode(key)];

                if (child == null)
                    throw new InvalidSpecificationException(String.Format("Parent has no child with key {0}", key));

                string value = ((YamlScalarNode)child).Value;
                if (value == null)
                    throw new InvalidSpecificationException(errorMessage ?? String.Format("No value for key {0}", key));

                return value;
            }
            catch (Exception ex)
            {
                throw new InvalidSpecificationException(errorMessage ?? ex.Message, ex);
            }
        }

        /// <summary>
        /// Gets an optional scalar value identified by its key
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="key">Key of the value</param>
        /// <param name="defaultValue">Default value to be used if the key is not found in parent</param>
        /// <returns>Returns either the scalar value read from the parent node, or the default value.</returns>
        public string GetOptionalScalarValue(YamlNode parent, string key, string defaultValue)
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

        /// <summary>
        /// Enumerates the key-value pairs in a mapping node
        /// </summary>
        /// <param name="mapping">Mapping node to be enumerated</param>
        /// <returns>Returns a sequence of key-value pairs, both key and value are generic YAML nodes</returns>
        public IEnumerable<KeyValuePair<YamlNode, YamlNode>> EnumerateNodesOf(YamlMappingNode mapping)
        {
            return mapping;
        }

        /// <summary>
        /// Sets the active goal to be used for filtering the nodes
        /// </summary>
        /// <param name="goal">New active goal</param>
        public void SetActiveGoal(Goal goal)
        {
            activeGoal = goal;
        }
    }
}