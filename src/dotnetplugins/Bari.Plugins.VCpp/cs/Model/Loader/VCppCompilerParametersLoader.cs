using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.VCpp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="VCppProjectCompilerParameters"/> parameter block from YAML files
    /// </summary>
    public class VCppCompilerParametersLoader : YamlProjectParametersLoaderBase<VCppProjectCompilerParameters>
    {
        public VCppCompilerParametersLoader(Suite suite)
            : base(suite)
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
                    {"processor-count", () => target.ProcessorNumber = (int?)ParseUint32(value)},
                    {"runtime-library", () => target.RuntimeLibrary = ParseEnum<RuntimeLibraryType>(value, "runtime library")},
                    {"runtime-type-info", () => target.RuntimeTypeInfo = ParseBool(value)},
                    {"smaller-type-check", () => target.SmallerTypeCheck = ParseBool(value)},
                    {"string-pooling", () => target.StringPooling = ParseBool(value)},
                    {"struct-member-alignment", () => target.StructMemberAlignment = (int?) ParseUint32(value)},
                    {"warnings-as-error", () => ParseWarningsAsError(target, value)},
                    {"treat-wchart-as-buildin-type", () => target.TreatWCharTAsBuiltInType = ParseBool(value)},
                    {"undefine-preprocessor-definitions", () => ParseUndefinePreprocessorDefinitions(target, value)},
                    {"warning-level", () => target.WarningLevel = ParseWarningLevel(value)},
                    {"whole-program-optimization", () => target.WholeProgramOptimization = ParseBool(value)},
                    {"pdb-file-name", () => target.PDBFileName = ParseString(value)}
                    };
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
    }
}