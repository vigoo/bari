using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.Csharp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="CsharpProjectParameters"/> parameter block from YAML files
    /// </summary>
    public class CsharpParametersLoader : IYamlProjectParametersLoader
    {
        private readonly Suite suite;

        public CsharpParametersLoader(Suite suite)
        {
            this.suite = suite;
        }

        public bool Supports(string name)
        {
            return "csharp".Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

        public IProjectParameters Load(string name, YamlNode value)
        {
            var result = new CsharpProjectParameters(suite);
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
                    {"defines", () => { target.Defines = ParseDefines(value); }},
                    {"delay-sign", () => { target.DelaySign = ParseBool(value); }},
                    {"doc-output", () => { target.DocOutput = ParseString(value); }},
                    {"file-align", () => { target.FileAlign = ParseUint23(value); }},
                    {"high-entropy-virtual-address-space", () => { target.HighEntropyVirtualAddressSpace = ParseBool(value); }},
                    {"key-container", () => { target.KeyContainer = ParseString(value); }},
                    {"key-file", () => { target.KeyFile = ParseString(value); }},
                    {"language-version", () => { target.LanguageVersion = ParseLanguageVersion(value); }},
                    {"main-class", () => { target.MainClass = ParseString(value); }},
                    {"no-std-lib", () => { target.NoStdLib = ParseBool(value); }},
                    {"suppressed-warnings", () => { target.SuppressedWarnings = ParseWarnings(value); }},
                    {"no-win23-manifest", () => { target.NoWin32Manifest = ParseBool(value); }},
                    {"optimize", () => { target.Optimize = ParseBool(value); }},
                    {"platform", () => { target.Platform = ParsePlatform(value); }},
                    {"preferred-ui-lang", () => { target.PreferredUILang = ParseString(value); }},
                    {"subsystem-version", () => { target.SubsystemVersion = ParseString(value); }},
                    {"unsafe", () => { target.Unsafe = ParseBool(value); }},
                    {"warning-level", () => { target.WarningLevel = ParseWarningLevel(value); }},
                    {"warnings-as-error", () => ParseWarningsAsError(target, value) },
                    {"root-namespace", () => { target.RootNamespace = ParseString(value); }}
                    };

            foreach (var pair in mapping)
                if (NameIs(name, pair.Key))
                    pair.Value();
        }

        private void ParseWarningsAsError(CsharpProjectParameters target, YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                target.SpecificWarningsAsError = ParseWarnings(value);
            else
                target.AllWarningsAsError = ParseBool(value);
        }

        private int[] ParseWarnings(YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                return seq.Children.OfType<YamlScalarNode>().Select(childValue => Int32.Parse(childValue.Value)).ToArray();
            else
                return new int[0];
        }

        private string[] ParseDefines(YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                return seq.Children.OfType<YamlScalarNode>().Select(childValue => childValue.Value).ToArray();
            else
                return new string[0];
        }

        private WarningLevel ParseWarningLevel(YamlNode value)
        {
            var sval = ParseString(value).ToLowerInvariant();
            switch (sval)
            {
                case "0":
                case "off":
                    return WarningLevel.Off;
                case "1":
                    return WarningLevel.Level1;
                case "2":
                    return WarningLevel.Level2;
                case "3":
                    return WarningLevel.Level3;
                case "4":
                case "all":
                    return WarningLevel.All;
                default:
                    throw new InvalidSpecificationException(
                        String.Format("Invalid warning level: {0}. Must be 'off', '1', '2', '3' or 'all'", sval));
            }
        }

        private CLRPlatform ParsePlatform(YamlNode value)
        {
            var sval = ParseString(value).ToLowerInvariant();
            switch (sval)
            {
                case "anycpu":
                    return CLRPlatform.AnyCPU;
                case "anycpu-32bit-preferred":
                    return CLRPlatform.AnyCPU32BitPreferred;
                case "arm":
                    return CLRPlatform.ARM;
                case "x64":
                    return CLRPlatform.x64;
                case "x86":
                    return CLRPlatform.x86;
                case "itanium":
                    return CLRPlatform.Itanium;
                default:
                    throw new InvalidSpecificationException(
                        String.Format("Invalid CLR platform: {0}. Must be 'anycpu', 'anycpu-32bit-preferred', 'arm', 'x64', 'x86' or 'itanium'", sval));
            }
        }

        private CsharpLanguageVersion ParseLanguageVersion(YamlNode value)
        {
            var sval = ParseString(value).ToLowerInvariant();
            switch (sval)
            {
                case "default":
                    return CsharpLanguageVersion.Default;
                case "iso1":
                    return CsharpLanguageVersion.ISO1;
                case "iso2":
                    return CsharpLanguageVersion.ISO2;
                case "3":
                    return CsharpLanguageVersion.V3;
                default:
                    throw new InvalidSpecificationException(
                        String.Format("Invalid C# language version: {0}. Must be 'default', 'iso1', 'iso2' or '3'", sval));
            }
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