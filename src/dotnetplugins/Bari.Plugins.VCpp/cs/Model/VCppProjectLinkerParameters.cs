using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectLinkerParametersDef : ProjectParametersPropertyDefs<VCppProjectLinkerParameters>
    {
        public VCppProjectLinkerParametersDef()
        {
            Define<string[]>("AdditionalDependencies");
            Define<string[]>("AdditionalLibraryDirectories");
            Define<string[]>("AdditionalManifestDependencies");
            Define<string[]>("AdditionalOptions");
            Define<string[]>("AddModuleNamesToAssembly");
            Define<bool>("AllowIsolation");
            Define<bool>("AssemblyDebug");
            Define<string>("BaseAddress");
            Define<CLRImageType>("CLRImageType");
            Define<CLRSupportLastError>("CLRSupportLastError");
            Define<ApartmentState>("CLRThreadAttribute");
            Define<bool>("CLRUnmanagedCodeCheck");
            Define<LinkerHotPatchingOption>("CreateHotPatchableImage");
            Define<bool>("DataExecutionPrevention");
            Define<string[]>("DelayLoadDLLs");
            Define<bool>("DelaySign");
            Define<LinkerDriverOption>("Driver");
            Define<bool>("EnableCOMDATFolding");
            Define<bool>("EnableUAC");
            Define<string>("EntryPointSymbol");
            Define<bool>("FixedBaseAddress");
            Define<LinkerForceOption>("ForceFileOutput");
            Define<string[]>("ForceSymbolReferences");
            Define<string>("FunctionOrder");
            Define<bool>("GenerateDebugInformation");
            Define<bool>("GenerateManifest");
            Define<bool>("GenerateMapFile");
            Define<int>("HeapCommitSize");
            Define<int>("HeapReserveSize");
            Define<bool>("IgnoreAllDefaultLibraries");
            Define<bool>("IgnoreEmbeddedIDL");
            Define<bool>("IgnoreImportLibrary");
            Define<string[]>("IgnoreSpecificDefaultLibraries");
            Define<bool>("ImageHasSafeExceptionHandlers");
            Define<string>("ImportLibrary");
            Define<string>("KeyContainer");
            Define<string>("KeyFile");
            Define<bool>("LargeAddressAware");
            Define<bool>("LinkDLL");
            Define<bool>("LinkIncremental");
            Define<bool>("MapExports");
            Define<string>("MapFileName");
            Define<string>("MergedIDLBaseFileName");
            Define<IDictionary<string, string>>("MergeSections");
            Define<int>("MinimumRequiredVersion");
            Define<bool>("NoEntryPoint");
            Define<bool>("OptimizeReferences");
            Define<bool>("PreventDllBinding");
            Define<bool>("RandomizedBaseAddress");
            Define<int>("SectionAlignment");
            Define<bool>("SetChecksum");
            Define<int>("StackCommitSize");
            Define<int>("StackReserveSize");
            Define<LinkerSubSystemOption>("SubSystem");
            Define<bool>("SupportNobindOfDelayLoadedDLL");
            Define<bool>("SupportUnloadOfDelayLoadedDLL");
            Define<bool>("SwapRunFromCD");
            Define<bool>("SwapRunFromNet");
            Define<LinkerTargetMachine>("TargetMachine");
            Define<bool>("TerminalServerAware");
            Define<bool>("TreatLinkerWarningAsErrors");
            Define<bool>("TurnOffAssemblyGeneration");
            Define<int>("TypeLibraryResourceID");
            Define<string>("TypeLibraryFile");
            Define<UACExecutionLevel>("UACExecutionLevel");
            Define<bool>("UACUIAccess");
        }

        public override VCppProjectLinkerParameters CreateDefault(Suite suite, VCppProjectLinkerParameters parent)
        {
            return new VCppProjectLinkerParameters(suite, parent);
        }
    }

    /// <summary>
    /// Parameters for the linker (Link MSBuild task)
    /// </summary>
    public class VCppProjectLinkerParameters : VCppProjectParametersBase<VCppProjectLinkerParameters, VCppProjectLinkerParametersDef>
    {
        private readonly Suite suite;
        public string[] AdditionalDependencies { get { return Get<string[]>("AdditionalDependencies"); } set { Set("AdditionalDependencies", value); } }
        public bool AreAdditionalDependenciesSpecified { get { return IsSpecified("AdditionalDependencies"); }}

        public string[] AdditionalLibraryDirectories { get { return Get<string[]>("AdditionalLibraryDirectories"); } set { Set("AdditionalLibraryDirectories", value); } }
        public bool AreAdditionalLibraryDirectoriesSpecified { get { return IsSpecified("AdditionalLibraryDirectories"); }}

        public string[] AdditionalManifestDependencies { get { return Get<string[]>("AdditionalManifestDependencies"); } set { Set("AdditionalManifestDependencies", value); } }
        public bool AreAdditionalManifestDependenciesSpecified { get { return IsSpecified("AdditionalManifestDependencies"); }}

        public string[] AdditionalOptions { get { return Get<string[]>("AdditionalOptions"); } set { Set("AdditionalOptions", value); } }
        public bool AreAdditionalOptionsSpecified { get { return IsSpecified("AdditionalOptions"); }}

        public string[] AddModuleNamesToAssembly { get { return Get<string[]>("AddModuleNamesToAssembly"); } set { Set("AddModuleNamesToAssembly", value); } }
        public bool IsAddModuleNamesToAssemblySpecified { get { return IsSpecified("AddModuleNamesToAssembly"); }}

        public bool AllowIsolation { get { return Get<bool>("AllowIsolation"); } set { Set("AllowIsolation", value); } }
        public bool IsAllowIsolationSpecified { get { return IsSpecified("AllowIsolation"); }}

        public bool AssemblyDebug { get { return Get<bool>("AssemblyDebug"); } set { Set("AssemblyDebug", value); } }
        public bool IsAssemblyDebugSpecified { get { return IsSpecified("AssemblyDebug"); }}

        public string BaseAddress { get { return Get<string>("BaseAddress"); } set { Set("BaseAddress", value); } }
        public bool IsBaseAddressSpecified { get { return IsSpecified("BaseAddress"); }}

        public CLRImageType CLRImageType { get { return Get<CLRImageType>("CLRImageType"); } set { Set("CLRImageType", value); } }
        public bool IsCLRImageTypeSpecified { get { return IsSpecified("CLRImageType"); }}

        public CLRSupportLastError CLRSupportLastError { get { return Get<CLRSupportLastError>("CLRSupportLastError"); } set { Set("CLRSupportLastError", value); } }
        public bool IsCLRSupportLastErrorSpecified { get { return IsSpecified("CLRSupportLastError"); }}

        public ApartmentState CLRThreadAttribute { get { return Get<ApartmentState>("CLRThreadAttribute"); } set { Set("CLRThreadAttribute", value); } }
        public bool IsCLRThreadAttributeSpecified { get { return IsSpecified("CLRThreadAttribute"); }}

        public bool CLRUnmanagedCodeCheck { get { return Get<bool>("CLRUnmanagedCodeCheck"); } set { Set("CLRUnmanagedCodeCheck", value); } }
        public bool IsCLRUnmanagedCodeCheckSpecified { get { return IsSpecified("CLRUnmanagedCodeCheck"); }}

        public LinkerHotPatchingOption CreateHotPatchableImage { get { return Get<LinkerHotPatchingOption>("CreateHotPatchableImage"); } set { Set("CreateHotPatchableImage", value); } }
        public bool IsCreateHotPatchableImageSpecified { get { return IsSpecified("CreateHotPatchableImage"); }}

        public bool DataExecutionPrevention { get { return Get<bool>("DataExecutionPrevention"); } set { Set("DataExecutionPrevention", value); } }
        public bool IsDataExecutionPreventionSpecified { get { return IsSpecified("DataExecutionPrevention"); }}

        public string[] DelayLoadDLLs { get { return Get<string[]>("DelayLoadDLLs"); } set { Set("DelayLoadDLLs", value); } }
        public bool AreDelayLoadDLLsSpecified { get { return IsSpecified("DelayLoadDLLs"); }}

        public bool DelaySign { get { return Get<bool>("DelaySign"); } set { Set("DelaySign", value); } }
        public bool IsDelaySignSpecified { get { return IsSpecified("DelaySign"); }}

        public LinkerDriverOption Driver { get { return Get<LinkerDriverOption>("Driver"); } set { Set("Driver", value); } }
        public bool IsDriverSpecified { get { return IsSpecified("Driver"); }}

        public bool EnableCOMDATFolding { get { return Get<bool>("EnableCOMDATFolding"); } set { Set("EnableCOMDATFolding", value); } }
        public bool IsEnableCOMDATFoldingSpecified { get { return IsSpecified("EnableCOMDATFolding"); }}

        public bool EnableUAC { get { return Get<bool>("EnableUAC"); } set { Set("EnableUAC", value); } }
        public bool IsEnableUACSpecified { get { return IsSpecified("EnableUAC"); }}

        public string EntryPointSymbol { get { return Get<string>("EntryPointSymbol"); } set { Set("EntryPointSymbol", value); } }
        public bool IsEntryPointSymbolSpecified { get { return IsSpecified("EntryPointSymbol"); }}

        public bool FixedBaseAddress { get { return Get<bool>("FixedBaseAddress"); } set { Set("FixedBaseAddress", value); } }
        public bool IsFixedBaseAddressSpecified { get { return IsSpecified("FixedBaseAddress"); }}

        public LinkerForceOption ForceFileOutput { get { return Get<LinkerForceOption>("ForceFileOutput"); } set { Set("ForceFileOutput", value); } }
        public bool IsForceFileOutputSpecified { get { return IsSpecified("ForceFileOutput"); }}

        public string[] ForceSymbolReferences { get { return Get<string[]>("ForceSymbolReferences"); } set { Set("ForceSymbolReferences", value); } }
        public bool AreForceSymbolReferencesSpecified { get { return IsSpecified("ForceSymbolReferences"); }}

        public string FunctionOrder { get { return Get<string>("FunctionOrder"); } set { Set("FunctionOrder", value); } }
        public bool IsFunctionOrderSpecified { get { return IsSpecified("FunctionOrder"); }}

        public bool GenerateDebugInformation { get { return Get<bool>("GenerateDebugInformation"); } set { Set("GenerateDebugInformation", value); } }
        public bool IsGenerateDebugInformationSpecified { get { return IsSpecified("GenerateDebugInformation"); }}

        public bool GenerateManifest { get { return Get<bool>("GenerateManifest"); } set { Set("GenerateManifest", value); } }
        public bool IsGenerateManifestSpecified { get { return IsSpecified("GenerateManifest"); }}

        public bool GenerateMapFile { get { return Get<bool>("GenerateMapFile"); } set { Set("GenerateMapFile", value); } }
        public bool IsGenerateMapFileSpecified { get { return IsSpecified("GenerateMapFile"); }}

        public int? HeapCommitSize { get { return GetAsNullable<int>("HeapCommitSize"); } set { SetAsNullable("HeapCommitSize", value); } }

        public int? HeapReserveSize { get { return GetAsNullable<int>("HeapReserveSize"); } set { SetAsNullable("HeapReserveSize", value); } }

        public bool IgnoreAllDefaultLibraries { get { return Get<bool>("IgnoreAllDefaultLibraries"); } set { Set("IgnoreAllDefaultLibraries", value); } }
        public bool IsIgnoreAllDefaultLibrariesSpecified { get { return IsSpecified("IgnoreAllDefaultLibraries"); }}

        public bool IgnoreEmbeddedIDL { get { return Get<bool>("IgnoreEmbeddedIDL"); } set { Set("IgnoreEmbeddedIDL", value); } }
        public bool IsIgnoreEmbeddedIDLSpecified { get { return IsSpecified("IgnoreEmbeddedIDL"); }}

        public bool IgnoreImportLibrary { get { return Get<bool>("IgnoreImportLibrary"); } set { Set("IgnoreImportLibrary", value); } }
        public bool IsIgnoreImportLibrarySpecified { get { return IsSpecified("IgnoreImportLibrary"); }}

        public string[] IgnoreSpecificDefaultLibraries { get { return Get<string[]>("IgnoreSpecificDefaultLibraries"); } set { Set("IgnoreSpecificDefaultLibraries", value); } }
        public bool IsIgnoreSpecificDefaultLibrariesSpecified { get { return IsSpecified("IgnoreSpecificDefaultLibraries"); }}

        public bool ImageHasSafeExceptionHandlers { get { return Get<bool>("ImageHasSafeExceptionHandlers"); } set { Set("ImageHasSafeExceptionHandlers", value); } }
        public bool IsImageHasSafeExceptionHandlersSpecified { get { return IsSpecified("ImageHasSafeExceptionHandlers"); }}

        public string ImportLibrary { get { return Get<string>("ImportLibrary"); } set { Set("ImportLibrary", value); } }
        public bool IsImportLibrarySpecified { get { return IsSpecified("ImportLibrary"); }}

        public string KeyContainer { get { return Get<string>("KeyContainer"); } set { Set("KeyContainer", value); } }
        public bool IsKeyContainerSpecified { get { return IsSpecified("KeyContainer"); }}

        public string KeyFile { get { return Get<string>("KeyFile"); } set { Set("KeyFile", value); } }
        public bool IsKeyFileSpecified { get { return IsSpecified("KeyFile"); }}

        public bool LargeAddressAware { get { return Get<bool>("LargeAddressAware"); } set { Set("LargeAddressAware", value); } }
        public bool IsLargeAddressAwareSpecified { get { return IsSpecified("LargeAddressAware"); }}

        public bool LinkDLL { get { return Get<bool>("LinkDLL"); } set { Set("LinkDLL", value); } }
        public bool IsLinkDLLSpecified { get { return IsSpecified("LinkDLL"); }}

        public bool LinkIncremental { get { return Get<bool>("LinkIncremental"); } set { Set("LinkIncremental", value); } }
        public bool IsLinkIncrementalSpecified { get { return IsSpecified("LinkIncremental"); }}

        public bool MapExports { get { return Get<bool>("MapExports"); } set { Set("MapExports", value); } }
        public bool IsMapExportsSpecified { get { return IsSpecified("MapExports"); }}

        public string MapFileName { get { return Get<string>("MapFileName"); } set { Set("MapFileName", value); } }
        public bool IsMapFileNameSpecified { get { return IsSpecified("MapFileName"); }}

        public string MergedIDLBaseFileName { get { return Get<string>("MergedIDLBaseFileName"); } set { Set("MergedIDLBaseFileName", value); } }
        public bool IsMergedIDLBaseFileNameSpecified { get { return IsSpecified("MergedIDLBaseFileName"); }}

        public IDictionary<string, string> MergeSections { get { return Get<IDictionary<string, string>>("MergeSections"); } set { Set("MergeSections", value); } }
        public bool IsMergeSectionsSpecified { get { return IsSpecified("MergeSections"); }}

        public int? MinimumRequiredVersion { get { return GetAsNullable<int>("MinimumRequiredVersion"); } set { SetAsNullable("MinimumRequiredVersion", value); } }

        public bool NoEntryPoint { get { return Get<bool>("NoEntryPoint"); } set { Set("NoEntryPoint", value); } }
        public bool IsNoEntryPointSpecified { get { return IsSpecified("NoEntryPoint"); }}

        public bool OptimizeReferences { get { return Get<bool>("OptimizeReferences"); } set { Set("OptimizeReferences", value); } }
        public bool IsOptimizeReferencesSpecified { get { return IsSpecified("OptimizeReferences"); }}

        public bool PreventDllBinding { get { return Get<bool>("PreventDllBinding"); } set { Set("PreventDllBinding", value); } }
        public bool IsPreventDllBindingSpecified { get { return IsSpecified("PreventDllBinding"); }}

        public bool RandomizedBaseAddress { get { return Get<bool>("RandomizedBaseAddress"); } set { Set("RandomizedBaseAddress", value); } }
        public bool IsRandomizedBaseAddressSpecified { get { return IsSpecified("RandomizedBaseAddress"); }}

        public int? SectionAlignment { get { return GetAsNullable<int>("SectionAlignment"); } set { SetAsNullable("SectionAlignment", value); } }

        public bool SetChecksum { get { return Get<bool>("SetChecksum"); } set { Set("SetChecksum", value); } }
        public bool IsSetChecksumSpecified { get { return IsSpecified("SetChecksum"); }}

        public int? StackCommitSize { get { return GetAsNullable<int>("StackCommitSize"); } set { SetAsNullable("StackCommitSize", value); } }

        public int? StackReserveSize { get { return GetAsNullable<int>("StackReserveSize"); } set { SetAsNullable("StackReserveSize", value); } }

        public LinkerSubSystemOption SubSystem { get { return Get<LinkerSubSystemOption>("SubSystem"); } set { Set("SubSystem", value); } }
        public bool IsSubSystemSpecified { get { return IsSpecified("SubSystem"); }}

        public bool SupportNobindOfDelayLoadedDLL { get { return Get<bool>("SupportNobindOfDelayLoadedDLL"); } set { Set("SupportNobindOfDelayLoadedDLL", value); } }
        public bool IsSupportNobindOfDelayLoadedDLLSpecified { get { return IsSpecified("SupportNobindOfDelayLoadedDLL"); }}

        public bool SupportUnloadOfDelayLoadedDLL { get { return Get<bool>("SupportUnloadOfDelayLoadedDLL"); } set { Set("SupportUnloadOfDelayLoadedDLL", value); } }
        public bool IsSupportUnloadOfDelayLoadedDLLSpecified { get { return IsSpecified("SupportUnloadOfDelayLoadedDLL"); }}

        public bool SwapRunFromCD { get { return Get<bool>("SwapRunFromCD"); } set { Set("SwapRunFromCD", value); } }
        public bool IsSwapRunFromCDSpecified { get { return IsSpecified("SwapRunFromCD"); }}

        public bool SwapRunFromNet { get { return Get<bool>("SwapRunFromNet"); } set { Set("SwapRunFromNet", value); } }
        public bool IsSwapRunFromNetSpecified { get { return IsSpecified("SwapRunFromNet"); }}

        public LinkerTargetMachine TargetMachine { get { return Get<LinkerTargetMachine>("TargetMachine"); } set { Set("TargetMachine", value); } }
        public bool IsTargetMachineSpecified { get { return IsSpecified("TargetMachine"); }}

        public bool TerminalServerAware { get { return Get<bool>("TerminalServerAware"); } set { Set("TerminalServerAware", value); } }
        public bool IsTerminalServerAwareSpecified { get { return IsSpecified("TerminalServerAware"); }}

        public bool TreatLinkerWarningAsErrors { get { return Get<bool>("TreatLinkerWarningAsErrors"); } set { Set("TreatLinkerWarningAsErrors", value); } }
        public bool IsTreatLinkerWarningAsErrorsSpecified { get { return IsSpecified("TreatLinkerWarningAsErrors"); }}

        public bool TurnOffAssemblyGeneration { get { return Get<bool>("TurnOffAssemblyGeneration"); } set { Set("TurnOffAssemblyGeneration", value); } }
        public bool IsTurnOffAssemblyGenerationSpecified { get { return IsSpecified("TurnOffAssemblyGeneration"); }}

        public int? TypeLibraryResourceID { get { return GetAsNullable<int>("TypeLibraryResourceID"); } set { SetAsNullable("TypeLibraryResourceID", value); } }

        public string TypeLibraryFile { get { return Get<string>("TypeLibraryFile"); } set { Set("TypeLibraryFile", value); } }
        public bool IsTypeLibraryFileSpecified { get { return IsSpecified("TypeLibraryFile"); }}

        public UACExecutionLevel UACExecutionLevel { get { return Get<UACExecutionLevel>("UACExecutionLevel"); } set { Set("UACExecutionLevel", value); } }
        public bool IsUACExecutionLevelSpecified { get { return IsSpecified("UACExecutionLevel"); }}

        public bool UACUIAccess { get { return Get<bool>("UACUIAccess"); } set { Set("UACUIAccess", value); } }
        public bool IsUACUIAccessSpecified { get { return IsSpecified("UACUIAccess"); }}


        public VCppProjectLinkerParameters(Suite suite, VCppProjectLinkerParameters parent = null)
            : base(parent)
        {
            this.suite = suite;
        }

        public void FillProjectSpecificMissingInfo(Project project)
        {
            LinkDLL = project.Type == ProjectType.Library;
        }

        public void ToVcxprojProperties(XmlWriter writer)
        {
            WriteStringArray(writer, "AdditionalDependencies", AreAdditionalDependenciesSpecified ? AdditionalDependencies : new string[0]);
            WriteStringArray(writer, "AdditionalLibraryDirectories", AreAdditionalLibraryDirectoriesSpecified ? AdditionalLibraryDirectories : new string[0]);
            WriteStringArray(writer, "AdditionalManifestDependencies", AreAdditionalManifestDependenciesSpecified ? AdditionalManifestDependencies : new string[0]);
            WriteStringArray(writer, "AdditionalOptions", AreAdditionalOptionsSpecified ? AdditionalOptions : new string[0]);
            WriteStringArray(writer, "AddModuleNamesToAssembly", IsAddModuleNamesToAssemblySpecified ? AddModuleNamesToAssembly : new string[0]);
            if (IsAllowIsolationSpecified && AllowIsolation)
                writer.WriteElementString("AllowIsolation", XmlConvert.ToString(AllowIsolation));
            writer.WriteElementString("AssemblyDebug", XmlConvert.ToString(IsAssemblyDebugSpecified ? AssemblyDebug : suite.ActiveGoal.Has(Suite.DebugGoal.Name)));
            if (IsBaseAddressSpecified)
                writer.WriteElementString("BaseAddress", BaseAddress);
            if (IsCLRImageTypeSpecified && CLRImageType != CLRImageType.Default)
                writer.WriteElementString("CLRImageType", CLRImageType.ToString());
            if (IsCLRSupportLastErrorSpecified && CLRSupportLastError != CLRSupportLastError.Disabled)
                writer.WriteElementString("CLRSupportLastError", CLRSupportLastError.ToString());
            else
                writer.WriteElementString("CLRSupportLastError", Model.CLRSupportLastError.Enabled.ToString());
            if (IsCLRThreadAttributeSpecified && CLRThreadAttribute != ApartmentState.Unknown)
                writer.WriteElementString("CLRThreadAttribute", CLRThreadAttribute.ToString());
            writer.WriteElementString("CLRUnmanagedCodeCheck", XmlConvert.ToString(!IsCLRUnmanagedCodeCheckSpecified || CLRUnmanagedCodeCheck));
            if (IsCreateHotPatchableImageSpecified && CreateHotPatchableImage != LinkerHotPatchingOption.Disabled)
                writer.WriteElementString("CreateHotPatchableImage", CreateHotPatchableImage.ToString());
            writer.WriteElementString("DataExecutionPrevention", XmlConvert.ToString(!IsDataExecutionPreventionSpecified || DataExecutionPrevention));
            WriteStringArray(writer, "DelayLoadDLLs", AreDelayLoadDLLsSpecified ? DelayLoadDLLs : new string[0]);
            writer.WriteElementString("DelaySign", XmlConvert.ToString(IsDelaySignSpecified && DelaySign));
            if (IsDriverSpecified && Driver != LinkerDriverOption.NotSet)
                writer.WriteElementString("Driver", Driver.ToString());
            writer.WriteElementString("EnableCOMDATFolding", XmlConvert.ToString(IsEnableCOMDATFoldingSpecified && EnableCOMDATFolding));
            writer.WriteElementString("EnableUAC", XmlConvert.ToString(!IsEnableUACSpecified || EnableUAC));
            if (IsEntryPointSymbolSpecified)
                writer.WriteElementString("EntryPointSymbol", EntryPointSymbol);
            writer.WriteElementString("FixedBaseAddress", XmlConvert.ToString(IsFixedBaseAddressSpecified && FixedBaseAddress));   
            if (IsForceFileOutputSpecified && ForceFileOutput != LinkerForceOption.Disabled)
                writer.WriteElementString("ForceFileOutput", ForceFileOutput.ToString());
            WriteStringArray(writer, "ForceSymbolReferences", AreForceSymbolReferencesSpecified ? ForceSymbolReferences : new string[0]);
            if (IsFunctionOrderSpecified)
                writer.WriteElementString("FunctionOrder", FunctionOrder);
            writer.WriteElementString("GenerateDebugInformation",
                XmlConvert.ToString(IsGenerateDebugInformationSpecified
                    ? GenerateDebugInformation
                    : suite.ActiveGoal.Has(Suite.DebugGoal.Name)));
            writer.WriteElementString("GenerateManifest", XmlConvert.ToString(IsGenerateManifestSpecified && GenerateManifest));
            writer.WriteElementString("GenerateMapFile", XmlConvert.ToString(IsGenerateMapFileSpecified && GenerateMapFile));
            if (HeapCommitSize.HasValue)
                writer.WriteElementString("HeapCommitSize", XmlConvert.ToString(HeapCommitSize.Value));
            if (HeapReserveSize.HasValue)
                writer.WriteElementString("HeapReserveSize", XmlConvert.ToString(HeapReserveSize.Value));
            writer.WriteElementString("IgnoreAllDefaultLibraries", XmlConvert.ToString(IsIgnoreAllDefaultLibrariesSpecified && IgnoreAllDefaultLibraries));
            writer.WriteElementString("IgnoreEmbeddedIDL", XmlConvert.ToString(IsIgnoreEmbeddedIDLSpecified && IgnoreEmbeddedIDL));
            writer.WriteElementString("IgnoreImportLibrary", XmlConvert.ToString(IsIgnoreImportLibrarySpecified && IgnoreImportLibrary));
            WriteStringArray(writer, "IgnoreSpecificDefaultLibraries", IsIgnoreSpecificDefaultLibrariesSpecified ? IgnoreSpecificDefaultLibraries : new string[0]);
            writer.WriteElementString("ImageHasSafeExceptionHandlers", XmlConvert.ToString(IsImageHasSafeExceptionHandlersSpecified ? ImageHasSafeExceptionHandlers : suite.ActiveGoal.Has("x86")));
            if (IsImportLibrarySpecified)
                writer.WriteElementString("ImportLibrary", ImportLibrary);
            if (IsKeyContainerSpecified)
                writer.WriteElementString("KeyContainer", KeyContainer);
            if (IsKeyFileSpecified)
                writer.WriteElementString("KeyFile", KeyFile);
            writer.WriteElementString("LargeAddressAware", XmlConvert.ToString(!IsLargeAddressAwareSpecified || LargeAddressAware));
            writer.WriteElementString("LinkDLL", XmlConvert.ToString(IsLinkDLLSpecified && LinkDLL));
            writer.WriteElementString("MapExports", XmlConvert.ToString(IsMapExportsSpecified && MapExports));
            if (IsMapFileNameSpecified)
                writer.WriteElementString("MapFileName", MapFileName);
            if (IsMergedIDLBaseFileNameSpecified)
                writer.WriteElementString("MergedIDLBaseFileName", MergedIDLBaseFileName);
            if (IsMergeSectionsSpecified && MergeSections.Count > 0)
            {
                string merges = string.Join(";", MergeSections.Select(pair => pair.Key + "=" + pair.Value));
                writer.WriteElementString("MergeSections", merges);
            }
            if (MinimumRequiredVersion.HasValue)
                writer.WriteElementString("MinimumRequiredVersion", XmlConvert.ToString(MinimumRequiredVersion.Value));
            writer.WriteElementString("NoEntryPoint", XmlConvert.ToString(IsNoEntryPointSpecified && NoEntryPoint));
            writer.WriteElementString("OptimizeReferences", XmlConvert.ToString(!IsOptimizeReferencesSpecified || OptimizeReferences));
            writer.WriteElementString("PreventDllBinding", XmlConvert.ToString(IsPreventDllBindingSpecified && PreventDllBinding));
            writer.WriteElementString("RandomizedBaseAddress", XmlConvert.ToString(!IsRandomizedBaseAddressSpecified || RandomizedBaseAddress));
            if (SectionAlignment.HasValue)
                writer.WriteElementString("SectionAlignment", XmlConvert.ToString(SectionAlignment.Value));
            writer.WriteElementString("SetChecksum", XmlConvert.ToString(IsSetChecksumSpecified && SetChecksum));
            if (StackCommitSize.HasValue)
                writer.WriteElementString("StackCommitSize", XmlConvert.ToString(StackCommitSize.Value));
            if (StackReserveSize.HasValue)
                writer.WriteElementString("StackReserveSize", XmlConvert.ToString(StackReserveSize.Value));
            if (IsSubSystemSpecified && SubSystem != LinkerSubSystemOption.NotSet)
                writer.WriteElementString("SubSystem", SubSystem.ToString());
            else
                writer.WriteElementString("SubSystem", LinkerSubSystemOption.Windows.ToString());
            writer.WriteElementString("SupportNobindOfDelayLoadedDLL", XmlConvert.ToString(IsSupportNobindOfDelayLoadedDLLSpecified && SupportNobindOfDelayLoadedDLL));
            writer.WriteElementString("SupportUnloadOfDelayLoadedDLL", XmlConvert.ToString(IsSupportUnloadOfDelayLoadedDLLSpecified && SupportUnloadOfDelayLoadedDLL));
            writer.WriteElementString("SwapRunFromCD", XmlConvert.ToString(IsSwapRunFromCDSpecified && SwapRunFromCD));
            writer.WriteElementString("SwapRunFromNet", XmlConvert.ToString(IsSwapRunFromNetSpecified && SwapRunFromNet));
            if (IsTargetMachineSpecified && TargetMachine != LinkerTargetMachine.NotSet)
                writer.WriteElementString("TargetMachine", TargetMachine.ToString());
            else
                writer.WriteElementString("TargetMachine",
                    (suite.ActiveGoal.Has("x64") ? LinkerTargetMachine.MachineX64 : LinkerTargetMachine.MachineX86).ToString());
            writer.WriteElementString("TerminalServerAware", XmlConvert.ToString(IsTerminalServerAwareSpecified && TerminalServerAware));
            writer.WriteElementString("TreatLinkerWarningAsErrors", XmlConvert.ToString(IsTreatLinkerWarningAsErrorsSpecified && TreatLinkerWarningAsErrors));
            writer.WriteElementString("TurnOffAssemblyGeneration", XmlConvert.ToString(IsTurnOffAssemblyGenerationSpecified && TurnOffAssemblyGeneration));
            if (TypeLibraryResourceID.HasValue)
                writer.WriteElementString("TypeLibraryResourceID", XmlConvert.ToString(TypeLibraryResourceID.Value));

            if (IsTypeLibraryFileSpecified)
                writer.WriteElementString("TypeLibraryFile", TypeLibraryFile);

            writer.WriteElementString("UACExecutionLevel", (IsUACExecutionLevelSpecified ? UACExecutionLevel : UACExecutionLevel.AsInvoker).ToString());
            writer.WriteElementString("UACUIAccess", XmlConvert.ToString(IsUACUIAccessSpecified && UACUIAccess));
        }
    }
}