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
                    {"compile-as", () => target.CompileAs = ParseCLanguage(value) },
                    {"compile-as-managed", () => target.CompileAsManaged = ParseEnum<ManagedCppType>(value, "managed c++ type")},
                    {"create-hotpatchable-image", () => target.CreateHotpatchableImage = ParseBool(value)},
                    {"debug-information-format", () => target.DebugInformationFormat = ParseEnum<DebugInformationFormat>(value, "debug information format")},
                    {"disable-language-extensions", () => target.DisableLanguageExtensions = ParseBool(value)},
                    {"supressed-warnings", () => target.SuppressedWarnings = ParseWarnings(value)},
                    {"enhanced-instruction-set", () => target.EnableEnhancedInstructionSet = ParseEnum<EnhancedInstructionSet>(value, "enhanced instruction set")},
                    {"fiber-safe-optimizations", () => target.EnableFiberSafeOptimizations = ParseBool(value)},
                    {"code-analysis", () => target.CodeAnalysis = ParseBool(value)},
                    {"exception--handling", () => target.ExceptionHandling = ParseEnum<ExceptionHandlingType>(value, "exception handling type")},
                    {"expand-attributed-source", () => target.ExpandAttributedSource = ParseBool(value)},
                    {"favor", () => target.Favor = ParseEnum<OptimizationFavor>(value, "optimization favor")},
                    {"floating-point-exceptions", () => target.FloatingPointExceptions = ParseBool(value)},
                    {"floating-point-model", () => target.FloatingPointModel = ParseEnum<FloatingPointModel>(value, "floating point model")},
                    {"force-conformance-in-for-loop-scope", () => target.ForceConformanceInForLoopScope = ParseBool(value)},
                    {"forced-include-files", () => target.ForcedIncludeFiles = ParseStringArray(value)},
                    {"forced-using-files", () => target.ForcedUsingFiles = ParseStringArray(value)},
                    {"function-level-linking", () => target.FunctionLevelLinking = ParseBool(value)},
                    {"generate-xml-documentation-files", () => target.GenerateXMLDocumentationFiles = ParseBool(value)},
                    {"ignore-standard-include-path", () => target.IgnoreStandardIncludePath = ParseBool(value)},
                    {"inline-function-expansion", () => target.InlineFunctionExpansion = ParseEnum<InlineExpansion>(value, "inline expansion")},                    
                    {"intrinsic-functions", () => target.IntrinsicFunctions = ParseBool(value)},
                    {"minimal-rebuild", () => target.MinimalRebuild = ParseBool(value)},
                    {"multi-processor-compilation", () => target.MultiProcessorCompilation = ParseBool(value)},
                    {"omit-default-lib-name", () => target.OmitDefaultLibName = ParseBool(value)},
                    {"omit-frame-pointers", () => target.OmitFramePointers = ParseBool(value)},
                    {"openmp-support", () => target.OpenMPSupport = ParseBool(value)},
                    {"optimization", () => target.Optimization = ParseEnum<OptimizationLevel>(value, "optimization level")},
                    {"defines", () => target.Defines = ParseStringArray(value) },
                    {"processor-count", () => target.ProcessorNumber = (int?)ParseUint23(value)},
                    {"runtime-library", () => target.RuntimeLibrary = ParseEnum<RuntimeLibraryType>(value, "runtime library")},
                    {"runtime-type-info", () => target.RuntimeTypeInfo = ParseBool(value)},
                    {"smaller-type-check", () => target.SmallerTypeCheck = ParseBool(value)},
                    {"string-pooling", () => target.StringPooling = ParseBool(value)},
                    {"struct-member-alignment", () => target.StructMemberAlignment = (int?) ParseUint23(value)},
                    {"warnings-as-error", () => ParseWarningsAsError(target, value)},
                    {"treat-wchart-as-buildin-type", () => target.TreatWCharTAsBuiltInType = ParseBool(value)},
                    {"undefine-preprocessor-definitions", () => ParseUndefinePreprocessorDefinitions(target, value)},
                    {"warning-level", () => target.WarningLevel = ParseWarningLevel(value)},
                    {"whole-program-optimization", () => target.WholeProgramOptimization = ParseBool(value)}
                    };

            foreach (var pair in mapping)
                if (NameIs(name, pair.Key))
                    pair.Value();
        }

        private CppWarningLevel ParseWarningLevel(YamlNode value)
        {
            var sval = ParseString(value).ToLowerInvariant();
            switch (sval)
            {
                case "off":
                    return CppWarningLevel.Off;
                case "1":
                    return CppWarningLevel.Level1;
                case "2":
                    return CppWarningLevel.Level2;
                case "3":
                    return CppWarningLevel.Level3;
                case "4":
                    return CppWarningLevel.Level4;
                case "all":
                    return CppWarningLevel.All;
                default:
                    throw new InvalidSpecificationException(
                        String.Format("Invalid warning level: {0}. Should be 'off', '1', '2', '3', '4' or 'all'.", sval));
            }
        }

        private void ParseUndefinePreprocessorDefinitions(VCppProjectCompilerParameters target, YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                target.UndefinePreprocessorDefinitions = ParseStringArray(value);
            else
                target.UndefineAllPreprocessorDefinitions = ParseBool(value);
        }

        private void ParseWarningsAsError(VCppProjectCompilerParameters target, YamlNode value)
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

        private CLanguage ParseCLanguage(YamlNode value)
        {
            var sval = ParseString(value).ToLowerInvariant();
            switch (sval)
            {
                case "default":
                    return CLanguage.Default;
                case "c":
                    return CLanguage.CompileAsC;
                case "c++":
                    return CLanguage.CompileAsCpp;
                default:
                    throw new InvalidSpecificationException(String.Format("Invalid language specification: {0}. Should be 'default', 'c' or 'c++'", sval));
            }
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