using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using Bari.Plugins.VsCore.Model;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.Fsharp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="FsharpProjectParameters"/> parameter block from YAML files
    /// </summary>
    public class FsharpParametersLoader : YamlProjectParametersLoaderBase<FsharpProjectParameters>
    {
        public FsharpParametersLoader(IUserOutput output) : base(output)
        {
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "fsharp"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="suite">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override FsharpProjectParameters CreateNewParameters(Suite suite)
        {
            return new FsharpProjectParameters(suite);
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
        protected override Dictionary<string, Action> GetActions(FsharpProjectParameters target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>
                {
                    {"base-address", () => { target.BaseAddress = ParseUint32(value); }},
                    {"code-page", () => { target.CodePage = ParseString(value); }},
                    {"debug", () => { target.Debug = ParseDebugLevel(value); }},
                    {"defines", () => { target.Defines = ParseDefines(value); }},
                    {"doc-output", () => { target.DocOutput = ParseString(value); }},
                    {"high-entropy-virtual-address-space", () => { target.HighEntropyVirtualAddressSpace = ParseBool(parser, value); }},
                    {"key-file", () => { target.KeyFile = ParseString(value); }},
                    {"suppressed-warnings", () => { target.SuppressedWarnings = ParseWarnings(value); }},
                    {"optimize", () => { target.Optimize = ParseBool(parser, value); }},
                    {"platform", () => { target.Platform = ParsePlatform(value); }},
                    {"warning-level", () => { target.WarningLevel = ParseWarningLevel(value); }},
                    {"warnings-as-error", () => ParseWarningsAsError(parser, target, value)},
                    {"other-flags", () => { target.OtherFlags = ParseString(value); }},
                    {"tailcalls", () => { target.Tailcalls = ParseBool(parser, value); }},
                    {"target-framework-version", () => { target.TargetFrameworkVersion = ParseFrameworkVersion(ParseString(value)); }},
                    {"target-framework-profile", () => { target.TargetFrameworkProfile= ParseFrameworkProfile(ParseString(value)); }},
                    {"target-framework", () => ApplyFrameworkVersionAndProfile(target, ParseString(value))}
                };
        }

        private void ApplyFrameworkVersionAndProfile(FsharpProjectParameters target, string value)
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
                case "4.5.2": return FrameworkVersion.v452;
                case "4.6": return FrameworkVersion.v46;
                case "4.6.1": return FrameworkVersion.v461;
                case "4.6.2": return FrameworkVersion.v462;
                case "4.7": return FrameworkVersion.v47;
                case "4.7.1": return FrameworkVersion.v471;
                case "4.7.2": return FrameworkVersion.v472;
                case "4.8": return FrameworkVersion.v48;
                default:
                    throw new InvalidSpecificationException(
                        String.Format("Invalid framework version: {0}. Must be '2.0', '3.0', '3.5', '4.0', '4.5', '4.5.1', '4.5.2', '4.6', '4.6.1', '4.6.2', '4.7', '4.7.1', '4.7.2' or '4.8'", value));
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

        private void ParseWarningsAsError(YamlParser parser, FsharpProjectParameters target, YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                target.SpecificWarningsAsError = ParseWarnings(value);
            else
                target.AllWarningsAsError = ParseBool(parser, value);
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