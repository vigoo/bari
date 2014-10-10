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
        private const string ConditionalPrefix = "when ";
        private const string NegationPrefix = "not ";
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
            Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<string, YamlNode>>>() != null);

            var mapping = parent as YamlMappingNode;
            if (mapping != null)
            {
                var groupNameNode = new YamlScalarNode(groupName);
                if (mapping.Children.ContainsKey(groupNameNode))
                {
                    var groupSeq = mapping.Children[groupNameNode] as YamlSequenceNode;

                    if (groupSeq != null)
                    {
                        foreach (var item in EnumerateNodesOf(groupSeq))
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
                var pairs = EnumerateNodesOf(mapping);
                var keyNode = new YamlScalarNode(key);

                foreach (var pair in pairs)
                {
                    if (keyNode.Equals(pair.Key))
                    {
                        if (pair.Value is YamlScalarNode)
                        {
                            var v = ((YamlScalarNode) pair.Value).Value;
                            if (v != null)
                                return v;
                        }

                        throw new InvalidSpecificationException(errorMessage ?? String.Format("No value for key {0}", key));
                    }
                }

                throw new InvalidSpecificationException(String.Format("Parent has no child with key {0}", key));
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

            var mapping = parent as YamlMappingNode;
            if (mapping != null)
            {
                var pairs = EnumerateNodesOf(mapping);
                var keyNode = new YamlScalarNode(key);

                foreach (var pair in pairs)
                {
                    if (keyNode.Equals(pair.Key) &&
                        pair.Value is YamlScalarNode)
                        return ((YamlScalarNode)pair.Value).Value;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Enumerates the key-value pairs in a mapping node
        /// </summary>
        /// <param name="mapping">Mapping node to be enumerated</param>
        /// <returns>Returns a sequence of key-value pairs, both key and value are generic YAML nodes</returns>
        public IEnumerable<KeyValuePair<YamlNode, YamlNode>> EnumerateNodesOf(YamlMappingNode mapping)
        {
            foreach (var pair in mapping)
            {
                var keyScalar = pair.Key as YamlScalarNode;
                if (keyScalar != null)
                {
                    if (keyScalar.Value.StartsWith(ConditionalPrefix))
                    {
                        // This is a conditional node
                        var remaining = keyScalar.Value.Substring(ConditionalPrefix.Length);

                        string condition = remaining;
                        bool negated = false;

                        if (remaining.StartsWith(NegationPrefix))
                        {
                            condition = remaining.Substring(NegationPrefix.Length);
                            negated = true;
                        }
                        
                        if (activeGoal.Has(condition) ^ negated)
                        {
                            var mappingValue = pair.Value as YamlMappingNode;
                            if (mappingValue != null)
                            {
                                foreach (var child in EnumerateNodesOf(mappingValue))
                                    yield return child;
                            }
                        }
                    }
                    else
                    {
                        // This is a normal node
                        yield return pair;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates the nodes in a sequence node while resolving the conditional expressions
        /// </summary>
        /// <param name="seq">Sequence node to be enumerated</param>
        /// <returns>Returns a sequence of YAML nodes</returns>
        public IEnumerable<YamlNode> EnumerateNodesOf(YamlSequenceNode seq)
        {
            foreach (var node in seq)
            {
                var mapping = node as YamlMappingNode;
                if (mapping != null)
                {
                    bool allConditional = true;

                    foreach (var pair in mapping)
                    {
                        var keyScalar = pair.Key as YamlScalarNode;
                        if (keyScalar != null)
                        {
                            if (keyScalar.Value.StartsWith(ConditionalPrefix))
                            {
                                 // This is a conditional node
                                var condition = keyScalar.Value.Substring(ConditionalPrefix.Length);
                                var negated = false;

                                if (condition.StartsWith(NegationPrefix))
                                {
                                    condition = condition.Substring(NegationPrefix.Length);
                                    negated = true;
                                }

                                if (activeGoal.Has(condition) ^ negated)
                                {
                                    var seqValue = pair.Value as YamlSequenceNode;
                                    if (seqValue != null)
                                    {
                                        foreach (var child in EnumerateNodesOf(seqValue))
                                            yield return child;
                                    }
                                    else
                                    {
                                        yield return pair.Value;
                                    }
                                }
                            }
                            else
                            {
                                allConditional = false;
                                break;
                            }
                        }
                        else
                        {
                            allConditional = false;
                            break;
                        }
                    }

                    if (!allConditional)
                        yield return mapping;
                }
                else
                {
                    yield return node;
                }
            }
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
