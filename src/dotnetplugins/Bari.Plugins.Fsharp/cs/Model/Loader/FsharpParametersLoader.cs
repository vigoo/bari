using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Plugins.VsCore.Model;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.Fsharp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="FsharpProjectParameters"/> parameter block from YAML files
    /// </summary>
    public class FsharpParametersLoader : YamlProjectParametersLoaderBase<FsharpProjectParameters>
    {
        public FsharpParametersLoader(Suite suite)
            : base(suite)
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
                    {"high-entropy-virtual-address-space", () => { target.HighEntropyVirtualAddressSpace = ParseBool(value); }},
                    {"key-file", () => { target.KeyFile = ParseString(value); }},
                    {"suppressed-warnings", () => { target.SuppressedWarnings = ParseWarnings(value); }},
                    {"optimize", () => { target.Optimize = ParseBool(value); }},
                    {"platform", () => { target.Platform = ParsePlatform(value); }},
                    {"warning-level", () => { target.WarningLevel = ParseWarningLevel(value); }},
                    {"warnings-as-error", () => ParseWarningsAsError(target, value)},
                    {"other-flags", () => { target.OtherFlags = ParseString(value); }},
                    {"tailcalls", () => { target.Tailcalls = ParseBool(value); }}
                };
        }

        private void ParseWarningsAsError(FsharpProjectParameters target, YamlNode value)
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