using System;
using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Plugins.VsCore.Model;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.VCpp.Model.Loader
{
    /// <summary>
    /// Loads a MIDL parameter block (<see cref="VCppProjectMIDLParameters"/> from YAML
    /// </summary>
    public class VCppMIDLParametersLoader : YamlProjectParametersLoaderBase<VCppProjectMIDLParameters>
    {
        public VCppMIDLParametersLoader(Suite suite)
            : base(suite)
        {
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "midl"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="suite">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override VCppProjectMIDLParameters CreateNewParameters(Suite suite)
        {
            return new VCppProjectMIDLParameters(suite);
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
        protected override Dictionary<string, Action> GetActions(VCppProjectMIDLParameters target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>
            {
                {"additional-include-directories", () => target.AdditionalIncludeDirectories = ParseStringArray(value)},
                {"additional-options", () => target.AdditionalOptions = ParseStringArray(value)},
                {"application-configuration-mode", () => target.ApplicationConfigurationMode = ParseBool(value)},
                {"client-stub-file", () => target.ClientStubFile = ParseString(value)},
                {"c-preprocess-options", () => target.CPreprocessOptions = ParseStringArray(value)},
                {"default-char-type", () => target.DefaultCharType = ParseEnum<CharType>(value, "character type")},
                {"dll-data-file-name", () => target.DllDataFileName = ParseString(value)},
                {"enable-error-checks", () => target.EnableErrorChecks = ParseEnum<MidlErrorChecks>(value, "MIDL error checking mode")},
                {"error-check-allocations", () => target.ErrorCheckAllocations = ParseBool(value)},
                {"error-check-bounds", () => target.ErrorCheckAllocations = ParseBool(value)},
                {"error-check-enum-range", () => target.ErrorCheckEnumRange = ParseBool(value)},
                {"error-check-ref-pointers", () => target.ErrorCheckRefPointers = ParseBool(value)},
                {"error-check-stub-data", () => target.ErrorCheckStubData = ParseBool(value)},
                {"generate-client-stub", () => target.GenerateClientStub = ParseBool(value)},
                {"generate-server-stub", () => target.GenerateServerStub = ParseBool(value)},
                {"generate-stubless-proxies", () => target.GenerateStublessProxies = ParseBool(value)},
                {"generate-type-library", () => target.GenerateTypeLibrary = ParseBool(value)},
                {"header-file-name", () => target.HeaderFileName = ParseString(value)},
                {"ignore-standard-include-path", () => target.IgnoreStandardIncludePath = ParseBool(value)},
                {"interface-identifier-file-name", () => target.InterfaceIdentifierFileName = ParseString(value)},
                {"locale-id", () => target.LocaleID = ParseInt32(value)},
                {"mktyplib-compatible", () => target.MkTypLibCompatible = ParseBool(value)},
                {"preprocessor-definitions", () => target.PreprocessorDefinitions = ParseStringArray(value)},
                {"proxy-file-name", () => target.ProxyFileName = ParseString(value)},
                {"server-stub-file", () => target.ServerStubFile = ParseString(value)},
                {"struct-member-alignment", () => target.StructMemberAlignment = ParseInt32(value)},
                {"suppress-compiler-warnings", () => target.SuppressCompilerWarnings = ParseBool(value)},
                {"target-environment", () => target.TargetEnvironment = ParseEnum<MidlTargetEnvironment>(value, "MIDL target environment")},
                {"new-typelib-format", () => target.NewTypeLibFormat = ParseBool(value)},
                {"type-library-name", () => target.TypeLibraryName = ParseString(value)},
                {"undefine-preprocessor-definitions", () => target.UndefinePreprocessorDefinitions = ParseStringArray(value)},
                {"validate-all-parameters", () => target.ValidateAllParameters = ParseBool(value)},
                {"warnings-as-error", () => target.WarningsAsError = ParseBool(value)},
                {"warning-level", () => target.WarningLevel = ParseEnum<WarningLevel>(value, "warning level")}
            };
        }
    }
}