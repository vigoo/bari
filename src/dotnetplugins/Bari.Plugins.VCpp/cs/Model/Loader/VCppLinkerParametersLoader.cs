using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.VCpp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="VCppProjectLinkerParameters"/> parameter block from YAML files
    /// </summary>
    // TODO: merge parsing code with all parameter loaders
    public class VCppLinkerParametersLoader : IYamlProjectParametersLoader
    {
        private readonly Suite suite;

        public VCppLinkerParametersLoader(Suite suite)
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
            return "cpp-linker".Equals(name, StringComparison.InvariantCultureIgnoreCase);
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
            var result = new VCppProjectLinkerParameters(suite);
            var mapping = value as YamlMappingNode;

            if (mapping != null)
            {
                foreach (var pair in parser.EnumerateNodesOf(mapping))
                {
                    var scalarKey = pair.Key as YamlScalarNode;
                    if (scalarKey != null)
                        TryAddParameter(result, scalarKey.Value, pair.Value, parser);
                }
            }

            return result;
        }

        private void TryAddParameter(VCppProjectLinkerParameters target, string name, YamlNode value, YamlParser parser)
        {
            var mapping = new Dictionary<string, Action>
                {
                    { "additional-dependencies", () => target.AdditionalDependencies = ParseStringArray(value) },
                    { "additional-library-dependencies", () => target.AdditionalLibraryDirectories = ParseStringArray(value) },
                    { "additional-manifest-dependencies", () => target.AdditionalManifestDependencies = ParseStringArray(value) },
                    { "additional-options", () => target.AdditionalOptions = ParseStringArray(value) },
                    { "add-module-names-to-assembly", () => target.AddModuleNamesToAssembly = ParseStringArray(value) },
                    { "allow-isolation", () => target.AllowIsolation = ParseBool(value) },
                    { "assembly-debug", () => target.AssemblyDebug = ParseBool(value) },
                    { "base-address", () => target.BaseAddress = ParseString(value) },
                    { "clr-image-type", () => target.CLRImageType = ParseEnum<CLRImageType>(value, "CLR image type") },
                    { "clr-support-last-error", () => target.CLRSupportLastError = ParseEnum<CLRSupportLastError>(value, "CLR last error support")},
                    { "clr-thread-attribute", () => target.CLRThreadAttribute = ParseEnum<ApartmentState>(value, "CLR thread apartment state")},
                    { "clr-unmanaged-code-check", () => target.CLRUnmanagedCodeCheck = ParseBool(value) },
                    { "create-hotpatchable-image", () => target.CreateHotPatchableImage = ParseEnum<LinkerHotPatchingOption>(value, "hot patching") },
                    { "data-execution-prevention", () => target.DataExecutionPrevention = ParseBool(value) },
                    { "delay-load-dlls", () => target.DelayLoadDLLs = ParseStringArray(value) },
                    { "delay-sign", () => target.DelaySign = ParseBool(value) },
                    { "driver", () => target.Driver = ParseEnum<LinkerDriverOption>(value, "driver") },
                    { "enable-comdat-folding", () => target.EnableCOMDATFolding = ParseBool(value)},
                    { "enable-uac", () => target.EnableUAC = ParseBool(value) },
                    { "entry-point-symbol", () => target.EntryPointSymbol = ParseString(value) },
                    { "fixed-base-address", () => target.FixedBaseAddress = ParseBool(value) },
                    { "force-file-output", () => target.ForceFileOutput = ParseEnum<LinkerForceOption>(value, "force option")},
                    { "function-order", () => target.FunctionOrder = ParseString(value) },
                    { "generate-debug-information", () => target.GenerateDebugInformation = ParseBool(value) },
                    { "generate-manifest", () => target.GenerateManifest = ParseBool(value) },
                    { "generate-map-file", () => target.GenerateMapFile = ParseBool(value) },
                    { "heap-commit-size", () => target.HeapCommitSize = ParseInt32(value) },
                    { "heap-reserve-size", () => target.HeapReserveSize = ParseInt32(value) },
                    { "ignore-all-default-libraries", () => target.IgnoreAllDefaultLibraries = ParseBool(value) },
                    { "ignore-embedded-idl", () => target.IgnoreEmbeddedIDL = ParseBool(value) },
                    { "ignore-import-library", () => target.IgnoreImportLibrary = ParseBool(value) },
                    { "ignore-specific-default-libraries", () => target.IgnoreSpecificDefaultLibraries = ParseStringArray(value)},
                    { "image-has-safe-exception-handlers", () => target.ImageHasSafeExceptionHandlers = ParseBool(value) },
                    { "import-library", () => target.ImportLibrary = ParseString(value) },
                    { "key-container", () => target.KeyContainer = ParseString(value) },
                    { "key-file", () => target.KeyContainer = ParseString(value) },
                    { "large-address-aware", () => target.LargeAddressAware = ParseBool(value) },
                    { "link-dll", () => target.LinkDLL = ParseBool(value) },
                    { "link-incremental", () => target.LinkIncremental = ParseBool(value) },
                    { "map-exports", () =>target.MapExports = ParseBool(value) },
                    { "map-file-name", () => target.MapFileName = ParseString(value) },
                    { "merged-idl-base-file-name", () => target.MergedIDLBaseFileName = ParseString(value) },
                    { "merge-sections", () => target.MergeSections = ParseMergeSections(value, parser) },
                    { "minimum-required-vrsion", () => target.MinimumRequiredVersion = ParseInt32(value) },
                    { "no-entry-point", () => target.NoEntryPoint = ParseBool(value) },
                    { "optimize-references", () => target.OptimizeReferences = ParseBool(value) },
                    { "prevent-dll-binding", () => target.PreventDllBinding = ParseBool(value) },
                    { "randomized-base-address", () => target.RandomizedBaseAddress = ParseBool(value) },
                    { "section-alignment", () => target.SectionAlignment = ParseInt32(value) },
                    { "set-checksum", () => target.SetChecksum = ParseBool(value) },
                    { "stack-commit-size", () => target.StackCommitSize = ParseInt32(value) },
                    { "stack-reserve-size", () => target.StackReserveSize = ParseInt32(value) },
                    { "subsystem", () => target.SubSystem = ParseEnum<LinkerSubSystemOption>(value, "subsystem")},
                    { "support-nobind-of-delay-loaded-dlls", () => target.SupportNobindOfDelayLoadedDLL = ParseBool(value) },
                    { "support-unload-of-delay-loaded-dlls", () => target.SupportUnloadOfDelayLoadedDLL = ParseBool(value) },
                    { "swap-run-from-cd", () => target.SwapRunFromCD = ParseBool(value) },
                    { "swap-run-from-net", () => target.SwapRunFromNet = ParseBool(value) },
                    { "target-machine", () => target.TargetMachine = ParseEnum<LinkerTargetMachine>(value, "target machine")},
                    { "terminal-server-aware", () => target.TerminalServerAware = ParseBool(value) },
                    { "treat-linker-warning-as-errors", () => target.TreatLinkerWarningAsErrors = ParseBool(value) },
                    { "turn-off-assembly-generation", () => target.TurnOffAssemblyGeneration = ParseBool(value) },
                    { "type-library-resource-id", () => target.TypeLibraryResourceID = ParseInt32(value) },
                    { "uac-execution-level", () => target.UACExecutionLevel = ParseEnum<UACExecutionLevel>(value, "UAC execution level")},
                    { "uac-ui-access", () => target.UACUIAccess = ParseBool(value) }
                };

            foreach (var pair in mapping)
                if (NameIs(name, pair.Key))
                    pair.Value();
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


        private T ParseEnum<T>(YamlNode value, string description) where T : struct
        {
            var sval = ParseString(value);
            T result;
            if (!Enum.TryParse(sval, ignoreCase: true, result: out result))
            {
                var msg = new StringBuilder();
                msg.AppendFormat("Invalid {0}: {1}. Must be ", description, sval);

                var names = Enum.GetNames(typeof(T));
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

        private int ParseInt32(YamlNode value)
        {
            return Convert.ToInt32(ParseString(value));
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