using System;
using System.Collections.Generic;
using System.Reflection;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.CodeContracts.Model.Loader
{
    /// <summary>
    /// Loads <see cref="ContractsProjectParameters"/> parameter block from YAML files
    /// </summary>
    public class YamlContractsParametersLoader : YamlProjectParametersLoaderBase<ContractsProjectParameters>
    {
        public YamlContractsParametersLoader(Suite suite) : base(suite)
        {
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "contracts"; }
        }


        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="suite">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override ContractsProjectParameters CreateNewParameters(Suite suite)
        {
            return new ContractsProjectParameters();
        }

        protected override Dictionary<string, Action> GetActions(ContractsProjectParameters target, YamlNode value, YamlParser parser)
        {
            throw new NotImplementedException();
        }

        protected override void TryAddParameter(ContractsProjectParameters target, string name, YamlNode value, YamlParser parser)
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