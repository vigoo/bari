using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using Bari.Plugins.VsCore.Model;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.VCpp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="VCppProjectCompilerParameters"/> parameter block from YAML files
    /// </summary>
    public class VCppCompilerParametersLoader : YamlProjectParametersLoaderBase<VCppProjectCompilerParameters>
    {
        public VCppCompilerParametersLoader(IUserOutput output) : base(output)
        {
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "cpp-compiler"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="suite">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override VCppProjectCompilerParameters CreateNewParameters(Suite suite)
        {
            return new VCppProjectCompilerParameters(suite);
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
        protected override Dictionary<string, Action> GetActions(VCppProjectCompilerParameters target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>
                {
                    {"additional-include-directories", () => target.AdditionalIncludeDirectories = ParseStringArray(parser, value) },
                    {"additional-options", () => target.AdditionalOptions = ParseStringArray(parser, value) },
                    {"additional-using-directories", () => target.AdditionalUsingDirectories = ParseStringArray(parser, value) },
                    {"assembler-listing-location", () => target.AssemblerListingLocation = ParseString(value) },
                    {"assembler-output", () => target.AssemblerOutput = ParseEnum<AssemblerOutputType>(value, "assembler output type") },
                    {"basic-runtime-checks", () => target.BasicRuntimeChecks = ParseEnum<RuntimeCheckType>(value, "runtime check type") },
                    {"browse-information-file", () => target.BrowseInformationFile = ParseString(value) },
                    {"buffer-security-check", () => target.BufferSecurityCheck = ParseBool(parser, value) },
                    {"calling-convention", () => target.CallingConvention = ParseEnum<CallingConvention>(value, "calling convention") },
                    {"compile-as", () => target.CompileAs = ParseCLanguage(value) },
                    {"compile-as-managed", () => target.CompileAsManaged = ParseEnum<ManagedCppType>(value, "managed c++ type")},
                    {"create-hotpatchable-image", () => target.CreateHotpatchableImage = ParseBool(parser, value)},
                    {"debug-information-format", () => target.DebugInformationFormat = ParseEnum<DebugInformationFormat>(value, "debug information format")},
                    {"disable-language-extensions", () => target.DisableLanguageExtensions = ParseBool(parser, value)},
                    {"supressed-warnings", () => target.SuppressedWarnings = ParseWarnings(value)},
                    {"enhanced-instruction-set", () => target.EnableEnhancedInstructionSet = ParseEnum<EnhancedInstructionSet>(value, "enhanced instruction set")},
                    {"fiber-safe-optimizations", () => target.EnableFiberSafeOptimizations = ParseBool(parser, value)},
                    {"code-analysis", () => target.CodeAnalysis = ParseBool(parser, value)},
                    {"exception--handling", () => target.ExceptionHandling = ParseEnum<ExceptionHandlingType>(value, "exception handling type")},
                    {"expand-attributed-source", () => target.ExpandAttributedSource = ParseBool(parser, value)},
                    {"favor", () => target.Favor = ParseEnum<OptimizationFavor>(value, "optimization favor")},
                    {"floating-point-exceptions", () => target.FloatingPointExceptions = ParseBool(parser, value)},
                    {"floating-point-model", () => target.FloatingPointModel = ParseEnum<FloatingPointModel>(value, "floating point model")},
                    {"force-conformance-in-for-loop-scope", () => target.ForceConformanceInForLoopScope = ParseBool(parser, value)},
                    {"forced-include-files", () => target.ForcedIncludeFiles = ParseStringArray(parser, value)},
                    {"forced-using-files", () => target.ForcedUsingFiles = ParseStringArray(parser, value)},
                    {"function-level-linking", () => target.FunctionLevelLinking = ParseBool(parser, value)},
                    {"generate-xml-documentation-files", () => target.GenerateXMLDocumentationFiles = ParseBool(parser, value)},
                    {"ignore-standard-include-path", () => target.IgnoreStandardIncludePath = ParseBool(parser, value)},
                    {"inline-function-expansion", () => target.InlineFunctionExpansion = ParseEnum<InlineExpansion>(value, "inline expansion")},                    
                    {"intrinsic-functions", () => target.IntrinsicFunctions = ParseBool(parser, value)},
                    {"minimal-rebuild", () => target.MinimalRebuild = ParseBool(parser, value)},
                    {"multi-processor-compilation", () => target.MultiProcessorCompilation = ParseBool(parser, value)},
                    {"omit-default-lib-name", () => target.OmitDefaultLibName = ParseBool(parser, value)},
                    {"omit-frame-pointers", () => target.OmitFramePointers = ParseBool(parser, value)},
                    {"openmp-support", () => target.OpenMPSupport = ParseBool(parser, value)},
                    {"optimization", () => target.Optimization = ParseEnum<OptimizationLevel>(value, "optimization level")},
                    {"defines", () => target.Defines = ParseStringArray(parser, value) },
                    {"processor-count", () => target.ProcessorNumber = (int?)ParseUint32(value)},
                    {"runtime-library", () => target.RuntimeLibrary = ParseEnum<RuntimeLibraryType>(value, "runtime library")},
                    {"runtime-type-info", () => target.RuntimeTypeInfo = ParseBool(parser, value)},
                    {"smaller-type-check", () => target.SmallerTypeCheck = ParseBool(parser, value)},
                    {"string-pooling", () => target.StringPooling = ParseBool(parser, value)},
                    {"struct-member-alignment", () => target.StructMemberAlignment = (int?) ParseUint32(value)},
                    {"warnings-as-error", () => ParseWarningsAsError(parser, target, value)},
                    {"treat-wchart-as-buildin-type", () => target.TreatWCharTAsBuiltInType = ParseBool(parser, value)},
                    {"undefine-preprocessor-definitions", () => ParseUndefinePreprocessorDefinitions(parser, target, value)},
                    {"warning-level", () => target.WarningLevel = ParseWarningLevel(value)},
                    {"whole-program-optimization", () => target.WholeProgramOptimization = ParseBool(parser, value)},
                    {"pdb-file-name", () => target.PDBFileName = ParseString(value)},
                    {"target-framework-version", () => { target.TargetFrameworkVersion = ParseFrameworkVersion(ParseString(value)); }},
                    {"target-framework-profile", () => { target.TargetFrameworkProfile= ParseFrameworkProfile(ParseString(value)); }},
                    {"target-framework", () => ApplyFrameworkVersionAndProfile(target, ParseString(value))}
                };
        }

        private void ApplyFrameworkVersionAndProfile(VCppProjectCompilerParameters target, string value)
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

        private void ParseUndefinePreprocessorDefinitions(YamlParser parser, VCppProjectCompilerParameters target, YamlNode value)
        {
            var seq = value as YamlSequenceNode;
            if (seq != null)
                target.UndefinePreprocessorDefinitions = ParseStringArray(parser, value);
            else
                target.UndefineAllPreprocessorDefinitions = ParseBool(parser, value);
        }

        private void ParseWarningsAsError(YamlParser parser, VCppProjectCompilerParameters target, YamlNode value)
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
    }
}