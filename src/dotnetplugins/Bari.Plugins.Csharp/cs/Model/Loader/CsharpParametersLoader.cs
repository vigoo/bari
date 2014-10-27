using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using Bari.Plugins.VsCore.Model;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.Csharp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="CsharpProjectParameters"/> parameter block from YAML files
    /// </summary>
    public class CsharpParametersLoader : YamlProjectParametersLoaderBase<CsharpProjectParameters>
    {
        public CsharpParametersLoader(IUserOutput output) : base(output)
        {
        }

        protected override string BlockName
        {
            get { return "csharp"; }
        }

        protected override CsharpProjectParameters CreateNewParameters(Suite suite)
        {
            return new CsharpProjectParameters(suite);            
        }

        protected override Dictionary<string, Action> GetActions(CsharpProjectParameters target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>
                {
                    {"base-address", () => { target.BaseAddress = ParseUint32(value); }},
                    {"checked", () => { target.Checked = ParseBool(value); }},
                    {"code-page", () => { target.CodePage = ParseString(value); }},
                    {"debug", () => { target.Debug = ParseDebugLevel(value); }},
                    {"defines", () => { target.Defines = ParseDefines(parser, value); }},
                    {"delay-sign", () => { target.DelaySign = ParseBool(value); }},
                    {"doc-output", () => { target.DocOutput = ParseString(value); }},
                    {"file-align", () => { target.FileAlign = ParseUint32(value); }},
                    {"high-entropy-virtual-address-space", () => { target.HighEntropyVirtualAddressSpace = ParseBool(value); }},
                    {"key-container", () => { target.KeyContainer = ParseString(value); }},
                    {"key-file", () => { target.KeyFile = ParseString(value); }},
                    {"language-version", () => { target.LanguageVersion = ParseLanguageVersion(value); }},
                    {"main-class", () => { target.MainClass = ParseString(value); }},
                    {"no-std-lib", () => { target.NoStdLib = ParseBool(value); }},
                    {"suppressed-warnings", () => { target.SuppressedWarnings = ParseWarnings(parser, value); }},
                    {"no-win23-manifest", () => { target.NoWin32Manifest = ParseBool(value); }},
                    {"optimize", () => { target.Optimize = ParseBool(value); }},
                    {"platform", () => { target.Platform = ParsePlatform(value); }},
                    {"preferred-ui-lang", () => { target.PreferredUILang = ParseString(value); }},
                    {"subsystem-version", () => { target.SubsystemVersion = ParseString(value); }},
                    {"unsafe", () => { target.Unsafe = ParseBool(value); }},
                    {"warning-level", () => { target.WarningLevel = ParseWarningLevel(value); }},
                    {"warnings-as-error", () => ParseWarningsAsError(parser, target, value) },
                    {"root-namespace", () => { target.RootNamespace = ParseString(value); }},
                    {"application-icon", () => { target.ApplicationIcon = ParseString(value); }},
                    {"target-framework-version", () => { target.TargetFrameworkVersion = ParseFrameworkVersion(ParseString(value)); }},
                    {"target-framework-profile", () => { target.TargetFrameworkProfile= ParseFrameworkProfile(ParseString(value)); }},
                    {"target-framework", () => ApplyFrameworkVersionAndProfile(target, ParseString(value))}
                    };
        }

        private void ApplyFrameworkVersionAndProfile(CsharpProjectParameters target, string value)
        {
            string[] parts = value.Split('-');
            if (parts.Length == 1)
                target.TargetFrameworkVersion = ParseFrameworkVersion(value);
            else
            {
                target.TargetFrameworkVersion = ParseFrameworkVersion(parts[0]);
                target.TargetFrameworkProfile = ParseFrameworkProfile(parts[1]);
            }
        }

        private FrameworkVersion ParseFrameworkVersion(string value)
        {
            switch (value.TrimStart('v'))
            {
                case "2.0": return FrameworkVersion.v20;
                case "3.0": return FrameworkVersion.v30;
                case "3.5": return FrameworkVersion.v35;
                case "4.0": return FrameworkVersion.v4;
                case "4.5": return FrameworkVersion.v45;
                case "4.5.1": return FrameworkVersion.v451;
                default:
                    throw new InvalidSpecificationException(
                        String.Format("Invalid framework version: {0}. Must be '2.0', '3.0', '3.5', '4.0', '4.5' or '4.5.1'", value));
            }
        }

        private FrameworkProfile ParseFrameworkProfile(string value)
        {
            switch (value)
            {
                case "client": return FrameworkProfile.Client;
                default: return FrameworkProfile.Default;
            }
        }

        private void ParseWarningsAsError(YamlParser parser, CsharpProjectParameters target, YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                target.SpecificWarningsAsError = ParseWarnings(parser, value);
            else
                target.AllWarningsAsError = ParseBool(value);
        }

        private int[] ParseWarnings(YamlParser parser, YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                return parser.EnumerateNodesOf(seq).OfType<YamlScalarNode>().Select(childValue => Int32.Parse(childValue.Value)).ToArray();
            else
                return new int[0];
        }

        private string[] ParseDefines(YamlParser parser, YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                return parser.EnumerateNodesOf(seq).OfType<YamlScalarNode>().Select(childValue => childValue.Value).ToArray();
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
    }
}