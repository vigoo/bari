using System;
using System.Reflection;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.CodeContracts.Model.Loader
{
    /// <summary>
    /// Loads <see cref="ContractsProjectParameters"/> parameter block from YAML files
    /// </summary>
    public class YamlContractsParametersLoader : IYamlProjectParametersLoader
    {
        /// <summary>
        /// Checks whether a given parameter block is supported
        /// </summary>
        /// <param name="name">Name of the block</param>
        /// <returns>Returns <c>true</c> if the given block is supported.</returns>
        public bool Supports(string name)
        {
            return "contracts".Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Loads the YAML block
        /// </summary>
        /// <param name="name">Name of the block (same that was passed to <see cref="IYamlProjectParametersLoader.Supports"/>)</param>
        /// <param name="value">The YAML node representing the value</param>
        /// <param name="parser"></param>
        /// <returns>Returns the loaded node</returns>
        public IProjectParameters Load(string name, YamlNode value, YamlParser parser)
        {
            var result = new ContractsProjectParameters();
            var mapping = value as YamlMappingNode;
            if (mapping != null)
            {
                foreach (var pair in parser.EnumerateNodesOf(mapping))
                {
                    var scalarKey = pair.Key as YamlScalarNode;
                    if (scalarKey != null)
                        TryAddParameter(result, scalarKey.Value, pair.Value);
                }
            }

            return result;
        }

        private void TryAddParameter(ContractsProjectParameters target, string name, YamlNode value)
        {
            var T = typeof(ContractsProjectParameters);
            var pi = T.GetProperty(name, BindingFlags.IgnoreCase);
            if (pi == null)
            {
                var alternativeName = name.Replace("-", "");
                pi = T.GetProperty(alternativeName, BindingFlags.IgnoreCase);
            }

            if (pi != null)
            {
                pi.SetValue(target, ParseValue(value, pi.PropertyType), null);
            }
        }

        private object ParseValue(YamlNode value, Type propertyType)
        {
            var scalarValue = value as YamlScalarNode;
            if (scalarValue != null)
            {
                if (propertyType == typeof (bool))
                {
                    return "true".Equals(scalarValue.Value, StringComparison.InvariantCultureIgnoreCase) ||
                           "yes".Equals(scalarValue.Value, StringComparison.InvariantCultureIgnoreCase);
                }
                else if (propertyType == typeof (string))
                {
                    return scalarValue.Value;
                }
                else if (propertyType.IsEnum)
                {
                    return Enum.Parse(propertyType, scalarValue.Value, ignoreCase: true);
                }
            }

            return null;
        }
    }
}