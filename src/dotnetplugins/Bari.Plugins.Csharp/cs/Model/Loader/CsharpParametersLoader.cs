using System;
using System.Collections.Generic;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.Csharp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="CsharpProjectParameters"/> parameter block from YAML files
    /// </summary>
    public class CsharpParametersLoader: IYamlProjectParametersLoader
    {
        public bool Supports(string name)
        {
            return "csharp".Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

        public IProjectParameters Load(string name, YamlNode value)
        {
            var result = new CsharpProjectParameters();
            var mapping = value as YamlMappingNode;

            if (mapping != null)
            {
                foreach (var pair in mapping)
                {
                    var scalarKey = pair.Key as YamlScalarNode;
                    if (scalarKey != null)
                        TryAddParameter(result, scalarKey.Value, pair.Value);
                }
            }

            return result;
        }

        private void TryAddParameter(CsharpProjectParameters target, string name, YamlNode value)
        {
            var mapping = new Dictionary<string, Action>
                {
                    {"base-address", () => { target.BaseAddress = ParseUint23(value); }},
                    {"checked", () => { target.Checked = ParseBool(value); }},
                    {"code-page", () => { target.CodePage = ParseString(value); }},
                    {"debug", () => { target.Debug = ParseDebugLevel(value); }},
                };

            foreach (var pair in mapping)
                if (NameIs(name, pair.Key))
                    pair.Value();
        }

        private DebugLevel ParseDebugLevel(YamlNode value)
        {
            var sval = ParseString(value).ToLowerInvariant();
            switch (sval)
            {
                case "none":
                    return DebugLevel.None;
                case "pdbonly":
                    return DebugLevel.PdbOnly;
                case "full":
                    return DebugLevel.Full;
                default:
                    throw new InvalidSpecificationException(
                        String.Format("Invalid debug level: {0}. Must be 'none', 'pdbonly' or 'full'", sval));
            }
        }

        private string ParseString(YamlNode value)
        {
            return ((YamlScalarNode) value).Value;
        }

        private uint ParseUint23(YamlNode value)
        {
            return Convert.ToUInt32(ParseString(value));
        }

        private bool ParseBool(YamlNode value)
        {
            var scalarValue = (YamlScalarNode) value;
            return "true".Equals(scalarValue.Value, StringComparison.InvariantCultureIgnoreCase) ||
                   "yes".Equals(scalarValue.Value, StringComparison.InvariantCultureIgnoreCase);
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