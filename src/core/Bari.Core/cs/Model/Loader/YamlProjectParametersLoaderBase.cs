using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bari.Core.Exceptions;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Generic base class for simple project parameter loaders
    /// </summary>
    /// <typeparam name="TParamType">Type of the project parameters it supports</typeparam>
    public abstract class YamlProjectParametersLoaderBase<TParamType>: IYamlProjectParametersLoader
        where TParamType: IProjectParameters
    {
        private readonly IUserOutput output;
        private Suite suite;

        protected Suite Suite
        {
            get { return suite; }
            set { suite = value; }
        }

        protected YamlProjectParametersLoaderBase(IUserOutput output)
        {
            this.output = output;
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected abstract string BlockName { get; }

        /// <summary>
        /// Checks whether a given parameter block is supported
        /// </summary>
        /// <param name="name">Name of the block</param>
        /// <returns>Returns <c>true</c> if the given block is supported.</returns>
        public bool Supports(string name)
        {
            return BlockName.Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="suite">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected abstract TParamType CreateNewParameters(Suite suite);

        public virtual IProjectParameters Load(Suite suite, string name, YamlNode value, YamlParser parser)
        {
            this.suite = suite;

            var result = CreateNewParameters(suite);
            var mapping = value as YamlMappingNode;

            if (mapping != null)
            {
                foreach (var pair in parser.EnumerateNodesOf(mapping))
                {
                    var scalarKey = pair.Key as YamlScalarNode;
                    if (scalarKey != null)
                        TryAddParameter(result, scalarKey.Value, pair.Value, parser);
                }
            }
            else
            {
                var hints = new List<string>();
                
                if (value is YamlSequenceNode)
                    hints.Add("Remove the `-` characters to make it a mapping instead of sequence");
                
                output.Warning(String.Format("{0} block (line {1}) is not a mapping node", BlockName, value.Start.Line), hints.ToArray());
            }

            return result;
        }

        /// <summary>
        /// Gets the mapping table
        /// 
        /// <para>The table contains the action to be performed for each supported option key</para>
        /// </summary>
        /// <param name="target">Target model object to be filled</param>
        /// <param name="value">Value to be parsed</param>
        /// <param name="parser">Parser to be used</param>
        /// <returns>Returns the mapping</returns>
        protected abstract Dictionary<string, Action> GetActions(TParamType target, YamlNode value, YamlParser parser);

        /// <summary>
        /// Applies a parameter definition to the given parameters model.
        /// 
        /// <para>This only has to be overridden to provide a custom parameter parsing logic. Otherwise
        /// just override <see cref="GetActions"/></para>
        /// </summary>
        /// <param name="target">Target model object to be filled</param>
        /// <param name="name">Option name</param>
        /// <param name="value">Option value to be parsed</param>
        /// <param name="parser">Parser to be used</param>
        protected virtual void TryAddParameter(TParamType target, string name, YamlNode value, YamlParser parser)
        {
            var mapping = GetActions(target, value, parser);

            foreach (var pair in mapping)
                if (NameIs(name, pair.Key))
                    pair.Value();
        }


        protected string ParseString(YamlNode value)
        {
            return ((YamlScalarNode)value).Value;
        }

        protected uint ParseUint32(YamlNode value)
        {
            return Convert.ToUInt32(ParseString(value));
        }

        protected bool ParseBool(YamlNode value)
        {
            var scalarValue = (YamlScalarNode)value;
            return "true".Equals(scalarValue.Value, StringComparison.InvariantCultureIgnoreCase) ||
                   "yes".Equals(scalarValue.Value, StringComparison.InvariantCultureIgnoreCase);
        }


        protected T ParseEnum<T>(YamlNode value, string description) where T : struct
        {
            var sval = ParseString(value);
            T result;
            if (!Enum.TryParse(sval, ignoreCase: true, result: out result))
            {
                var msg = new StringBuilder();
                msg.AppendFormat("Invalid {0}: {1}. Must be ", description, sval);

                var names = Enum.GetNames(typeof(T));
                for (int i = 0; i < names.Length; i++)
                {
                    msg.Append('\'');
                    msg.Append(names[i]);
                    msg.Append('\'');

                    if (i < names.Length - 2)
                        msg.Append(", ");
                    else if (i < names.Length - 1)
                        msg.Append(" or ");
                }

                throw new InvalidSpecificationException(msg.ToString());
            }

            return result;
        }

        protected string[] ParseStringArray(YamlParser parser, YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                return parser.EnumerateNodesOf(seq).OfType<YamlScalarNode>().Select(childValue => childValue.Value).ToArray();
            else
                return new string[0];
        }

        protected int ParseInt32(YamlNode value)
        {
            return Convert.ToInt32(ParseString(value));
        }

        private bool NameIs(string name, string expectedName)
        {
            if (name.Equals(expectedName, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
            {
                var alternativeName = expectedName.Replace("-", "");
                return name.Equals(alternativeName, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}