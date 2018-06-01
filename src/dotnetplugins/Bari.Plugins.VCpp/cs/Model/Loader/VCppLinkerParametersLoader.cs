using System;
using System.Collections.Generic;
using System.Threading;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.VCpp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="VCppProjectLinkerParameters"/> parameter block from YAML files
    /// </summary>
    public class VCppLinkerParametersLoader : YamlProjectParametersLoaderBase<VCppProjectLinkerParameters>
    {
        public VCppLinkerParametersLoader(IUserOutput output) : base(output)
        {
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "cpp-linker"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="suite">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override VCppProjectLinkerParameters CreateNewParameters(Suite suite)
        {
            return new VCppProjectLinkerParameters(suite);
        }

        protected override Dictionary<string, Action> GetActions(VCppProjectLinkerParameters target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>
                {
                    { "additional-dependencies", () => target.AdditionalDependencies = ParseStringArray(parser, value) },
                    { "additional-library-dependencies", () => target.AdditionalLibraryDirectories = ParseStringArray(parser, value) },
                    { "additional-manifest-dependencies", () => target.AdditionalManifestDependencies = ParseStringArray(parser, value) },
                    { "additional-options", () => target.AdditionalOptions = ParseStringArray(parser, value) },
                    { "add-module-names-to-assembly", () => target.AddModuleNamesToAssembly = ParseStringArray(parser, value) },
                    { "allow-isolation", () => target.AllowIsolation = ParseBool(parser, value) },
                    { "assembly-debug", () => target.AssemblyDebug = ParseBool(parser, value) },
                    { "base-address", () => target.BaseAddress = ParseString(value) },
                    { "clr-image-type", () => target.CLRImageType = ParseEnum<CLRImageType>(value, "CLR image type") },
                    { "clr-support-last-error", () => target.CLRSupportLastError = ParseEnum<CLRSupportLastError>(value, "CLR last error support")},
                    { "clr-thread-attribute", () => target.CLRThreadAttribute = ParseEnum<ApartmentState>(value, "CLR thread apartment state")},
                    { "clr-unmanaged-code-check", () => target.CLRUnmanagedCodeCheck = ParseBool(parser, value) },
                    { "create-hotpatchable-image", () => target.CreateHotPatchableImage = ParseEnum<LinkerHotPatchingOption>(value, "hot patching") },
                    { "data-execution-prevention", () => target.DataExecutionPrevention = ParseBool(parser, value) },
                    { "delay-load-dlls", () => target.DelayLoadDLLs = ParseStringArray(parser, value) },
                    { "delay-sign", () => target.DelaySign = ParseBool(parser, value) },
                    { "driver", () => target.Driver = ParseEnum<LinkerDriverOption>(value, "driver") },
                    { "enable-comdat-folding", () => target.EnableCOMDATFolding = ParseBool(parser, value)},
                    { "enable-uac", () => target.EnableUAC = ParseBool(parser, value) },
                    { "entry-point-symbol", () => target.EntryPointSymbol = ParseString(value) },
                    { "fixed-base-address", () => target.FixedBaseAddress = ParseBool(parser, value) },
                    { "force-file-output", () => target.ForceFileOutput = ParseEnum<LinkerForceOption>(value, "force option")},
                    { "function-order", () => target.FunctionOrder = ParseString(value) },
                    { "generate-debug-information", () => target.GenerateDebugInformation = ParseBool(parser, value) },
                    { "generate-manifest", () => target.GenerateManifest = ParseBool(parser, value) },
                    { "generate-map-file", () => target.GenerateMapFile = ParseBool(parser, value) },
                    { "heap-commit-size", () => target.HeapCommitSize = ParseInt32(value) },
                    { "heap-reserve-size", () => target.HeapReserveSize = ParseInt32(value) },
                    { "ignore-all-default-libraries", () => target.IgnoreAllDefaultLibraries = ParseBool(parser, value) },
                    { "ignore-embedded-idl", () => target.IgnoreEmbeddedIDL = ParseBool(parser, value) },
                    { "ignore-import-library", () => target.IgnoreImportLibrary = ParseBool(parser, value) },
                    { "ignore-specific-default-libraries", () => target.IgnoreSpecificDefaultLibraries = ParseStringArray(parser, value)},
                    { "image-has-safe-exception-handlers", () => target.ImageHasSafeExceptionHandlers = ParseBool(parser, value) },
                    { "import-library", () => target.ImportLibrary = ParseString(value) },
                    { "key-container", () => target.KeyContainer = ParseString(value) },
                    { "key-file", () => target.KeyContainer = ParseString(value) },
                    { "large-address-aware", () => target.LargeAddressAware = ParseBool(parser, value) },
                    { "link-dll", () => target.LinkDLL = ParseBool(parser, value) },
                    { "link-incremental", () => target.LinkIncremental = ParseBool(parser, value) },
                    { "map-exports", () =>target.MapExports = ParseBool(parser, value) },
                    { "map-file-name", () => target.MapFileName = ParseString(value) },
                    { "merged-idl-base-file-name", () => target.MergedIDLBaseFileName = ParseString(value) },
                    { "merge-sections", () => target.MergeSections = ParseMergeSections(value, parser) },
                    { "minimum-required-vrsion", () => target.MinimumRequiredVersion = ParseInt32(value) },
                    { "no-entry-point", () => target.NoEntryPoint = ParseBool(parser, value) },
                    { "optimize-references", () => target.OptimizeReferences = ParseBool(parser, value) },
                    { "prevent-dll-binding", () => target.PreventDllBinding = ParseBool(parser, value) },
                    { "randomized-base-address", () => target.RandomizedBaseAddress = ParseBool(parser, value) },
                    { "section-alignment", () => target.SectionAlignment = ParseInt32(value) },
                    { "set-checksum", () => target.SetChecksum = ParseBool(parser, value) },
                    { "stack-commit-size", () => target.StackCommitSize = ParseInt32(value) },
                    { "stack-reserve-size", () => target.StackReserveSize = ParseInt32(value) },
                    { "subsystem", () => target.SubSystem = ParseEnum<LinkerSubSystemOption>(value, "subsystem")},
                    { "support-nobind-of-delay-loaded-dlls", () => target.SupportNobindOfDelayLoadedDLL = ParseBool(parser, value) },
                    { "support-unload-of-delay-loaded-dlls", () => target.SupportUnloadOfDelayLoadedDLL = ParseBool(parser, value) },
                    { "swap-run-from-cd", () => target.SwapRunFromCD = ParseBool(parser, value) },
                    { "swap-run-from-net", () => target.SwapRunFromNet = ParseBool(parser, value) },
                    { "target-machine", () => target.TargetMachine = ParseEnum<LinkerTargetMachine>(value, "target machine")},
                    { "terminal-server-aware", () => target.TerminalServerAware = ParseBool(parser, value) },
                    { "treat-linker-warning-as-errors", () => target.TreatLinkerWarningAsErrors = ParseBool(parser, value) },
                    { "turn-off-assembly-generation", () => target.TurnOffAssemblyGeneration = ParseBool(parser, value) },
                    { "type-library-resource-id", () => target.TypeLibraryResourceID = ParseInt32(value) },
                    { "type-library-file", () => target.TypeLibraryFile = ParseString(value) },
                    { "uac-execution-level", () => target.UACExecutionLevel = ParseEnum<UACExecutionLevel>(value, "UAC execution level")},
                    { "uac-ui-access", () => target.UACUIAccess = ParseBool(parser, value) }
                };
        }

        private IDictionary<string, string> ParseMergeSections(YamlNode value, YamlParser parser)
        {
            var result = new Dictionary<string, string>();
            var mapping = value as YamlMappingNode;

            if (mapping != null)
            {
                foreach (var pair in parser.EnumerateNodesOf(mapping))
                {
                    var scalarKey = pair.Key as YamlScalarNode;
                    var scalarValue = pair.Value as YamlScalarNode;
                    if (scalarKey != null && scalarValue != null)
                        result.Add(scalarKey.Value, scalarValue.Value);
                }
            }

            return result;
        }
    }
}