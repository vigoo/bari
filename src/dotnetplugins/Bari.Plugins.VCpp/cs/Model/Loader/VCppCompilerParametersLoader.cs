using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.VCpp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="VCppProjectCompilerParameters"/> parameter block from YAML files
    /// </summary>
    // TODO: merge parsing code with all parameter loaders
    public class VCppCompilerParametersLoader : IYamlProjectParametersLoader
    {
        private readonly Suite suite;

        public VCppCompilerParametersLoader(Suite suite)
        {
            this.suite = suite;
        }

        /// <summary>
        /// Checks whether a given parameter block is supported
        /// </summary>
        /// <param name="name">Name of the block</param>
        /// <returns>Returns <c>true</c> if the given block is supported.</returns>
        public bool Supports(string name)
        {
            return "cpp-compiler".Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Loads the YAML block
        /// </summary>
        /// <param name="name">Name of the block (same that was passed to <see cref="IYamlProjectParametersLoader.Supports"/>)</param>
        /// <param name="value">The YAML node representing the value</param>
        /// <param name="parser">The YAML parser to be used</param>
        /// <returns>Returns the loaded node</returns>
        public IProjectParameters Load(string name, YamlNode value, YamlParser parser)
        {
            var result = new VCppProjectCompilerParameters(suite);
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

        private void TryAddParameter(VCppProjectCompilerParameters target, string name, YamlNode value)
        {
            var mapping = new Dictionary<string, Action>
                {
                    {"additional-include-directories", () => target.AdditionalIncludeDirectories = ParseStringArray(value) },
                    {"additional-options", () => target.AdditionalOptions = ParseStringArray(value) },
                    {"additional-using-directories", () => target.AdditionalUsingDirectories = ParseStringArray(value) },
                    {"assembler-listing-location", () => target.AssemblerListingLocation = ParseString(value) },
                    {"assembler-output", () => target.AssemblerOutput = ParseEnum<AssemblerOutputType>(value, "assembler output type") },
                    {"basic-runtime-checks", () => target.BasicRuntimeChecks = ParseEnum<RuntimeCheckType>(value, "runtime check type") },
                    {"browse-information-file", () => target.BrowseInformationFile = ParseString(value) },
                    {"buffer-security-check", () => target.BufferSecurityCheck = ParseBool(value) },
                    {"calling-convention", () => target.CallingConvention = ParseEnum<CallingConvention>(value, "calling convention") },
                    {"compile-as", () => target.CompileAs = }
                    {"defines", () => target.Defines = ParseStringArray(value) },
                };

            foreach (var pair in mapping)
                if (NameIs(name, pair.Key))
                    pair.Value();
        }

        private T ParseEnum<T>(YamlNode value, string description) where T : struct
        {
            var sval = ParseString(value);
            T result;
            if (!Enum.TryParse(sval, ignoreCase: true, result: out result))
            {
                var msg = new StringBuilder();
                msg.AppendFormat("Invalid {0}: {1}. Must be ", description, sval);
                
                var names = Enum.GetNames(typeof (T));
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

        private string ParseString(YamlNode value)
        {
            return ((YamlScalarNode)value).Value;
        }

        private uint ParseUint23(YamlNode value)
        {
            return Convert.ToUInt32(ParseString(value));
        }

        private bool ParseBool(YamlNode value)
        {
            var scalarValue = (YamlScalarNode)value;
            return "true".Equals(scalarValue.Value, StringComparison.InvariantCultureIgnoreCase) ||
                   "yes".Equals(scalarValue.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        private string[] ParseStringArray(YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                return seq.Children.OfType<YamlScalarNode>().Select(childValue => childValue.Value).ToArray();
            else
                return new string[0];
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