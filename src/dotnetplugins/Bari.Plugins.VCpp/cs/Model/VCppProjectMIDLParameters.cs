using System;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectMIDLParametersDef : ProjectParametersPropertyDefs<VCppProjectMIDLParameters>
    {
        public VCppProjectMIDLParametersDef()
        {
            Define<string[]>("AdditionalIncludeDirectories");
            Define<string[]>("AdditionalOptions");
            Define<bool>("ApplicationConfigurationMode");
            Define<string>("ClientStubFile");
            Define<string[]>("CPreprocessOptions");
            Define<CharType>("DefaultCharType");
            Define<string>("DllDataFileName");
            Define<MidlErrorChecks>("EnableErrorChecks");
            Define<bool>("ErrorCheckAllocations");
            Define<bool>("ErrorCheckBounds");
            Define<bool>("ErrorCheckEnumRange");
            Define<bool>("ErrorCheckRefPointers");
            Define<bool>("ErrorCheckStubData");
            Define<bool>("GenerateClientStub");
            Define<bool>("GenerateServerStub");
            Define<bool>("GenerateStublessProxies");
            Define<bool>("GenerateTypeLibrary");
            Define<string>("HeaderFileName");
            Define<bool>("IgnoreStandardIncludePath");
            Define<string>("InterfaceIdentifierFileName");
            Define<int>("LocaleID");
            Define<bool>("MkTypLibCompatible");
            Define<string[]>("PreprocessorDefinitions");
            Define<string>("ProxyFileName");
            Define<string>("ServerStubFile");
            Define<int>("StructMemberAlignment");
            Define<bool>("SuppressCompilerWarnings");
            Define<MidlTargetEnvironment>("TargetEnvironment");
            Define<bool>("NewTypeLibFormat");
            Define<string>("TypeLibraryName");
            Define<string>("ComponentFileName");
            Define<string[]>("UndefinePreprocessorDefinitions");
            Define<bool>("ValidateAllParameters");
            Define<bool>("WarningsAsError");
            Define<WarningLevel>("WarningLevel");
        }


        public override VCppProjectMIDLParameters CreateDefault(Suite suite, VCppProjectMIDLParameters parent)
        {
            return new VCppProjectMIDLParameters(suite, parent);
        }
    }

    public class VCppProjectMIDLParameters : VCppProjectParametersBase<VCppProjectMIDLParameters, VCppProjectMIDLParametersDef>
    {
        private readonly Suite suite;
        public string[] AdditionalIncludeDirectories { get { return Get<string[]>("AdditionalIncludeDirectories"); } set { Set("AdditionalIncludeDirectories", value); } }
        public bool AreAdditionalIncludeDirectoriesSpecified { get { return IsSpecified("AdditionalIncludeDirectories"); } }

        public string[] AdditionalOptions { get { return Get<string[]>("AdditionalOptions"); } set { Set("AdditionalOptions", value); } }
        public bool AreAdditionalOptionsSpecified { get { return IsSpecified("AdditionalOptions"); } }

        public bool ApplicationConfigurationMode { get { return Get<bool>("ApplicationConfigurationMode"); } set { Set("ApplicationConfigurationMode", value); } }
        public bool IsApplicationConfigurationModeSpecified { get { return IsSpecified("ApplicationConfigurationMode"); } }

        public string ClientStubFile { get { return Get<string>("ClientStubFile"); } set { Set("ClientStubFile", value); } }
        public bool IsClientStubFileSpecified { get { return IsSpecified("ClientStubFile"); } }

        public string[] CPreprocessOptions { get { return Get<string[]>("CPreprocessOptions"); } set { Set("CPreprocessOptions", value); } }
        public bool AreCPreprocessOptionsSpecified { get { return IsSpecified("CPreprocessOptions"); }}
        
        public CharType DefaultCharType { get { return Get<CharType>("DefaultCharType"); } set { Set("DefaultCharType", value); } }
        public bool IsDefaultCharTypeSpecified { get { return IsSpecified("DefaultCharType"); }}
        
        public string DllDataFileName { get { return Get<string>("DllDataFileName"); } set { Set("DllDataFileName", value); } }
        public bool IsDllDataFileNameSpecified { get { return IsSpecified("DllDataFileName"); }}
        
        public MidlErrorChecks EnableErrorChecks { get { return Get<MidlErrorChecks>("EnableErrorChecks"); } set { Set("EnableErrorChecks", value); } }
        public bool IsEnableErrorChecksSpecified { get { return IsSpecified("EnableErrorChecks"); }}
        
        public bool ErrorCheckAllocations { get { return Get<bool>("ErrorCheckAllocations"); } set { Set("ErrorCheckAllocations", value); } }
        public bool IsErrorCheckAllocationsSpecified { get { return IsSpecified("ErrorCheckAllocations"); }}
        
        public bool ErrorCheckBounds { get { return Get<bool>("ErrorCheckBounds"); } set { Set("ErrorCheckBounds", value); } }
        public bool IsErrorCheckBoundsSpecified { get { return IsSpecified("ErrorCheckBounds"); }}
        
        public bool ErrorCheckEnumRange { get { return Get<bool>("ErrorCheckEnumRange"); } set { Set("ErrorCheckEnumRange", value); } }
        public bool IsErrorCheckEnumRangeSpecified { get { return IsSpecified("ErrorCheckEnumRange"); }}
        
        public bool ErrorCheckRefPointers { get { return Get<bool>("ErrorCheckRefPointers"); } set { Set("ErrorCheckRefPointers", value); } }
        public bool IsErrorCheckRefPointersSpecified { get { return IsSpecified("ErrorCheckRefPointers"); }}
        
        public bool ErrorCheckStubData { get { return Get<bool>("ErrorCheckStubData"); } set { Set("ErrorCheckStubData", value); } }
        public bool IsErrorCheckStubDataSpecified { get { return IsSpecified("ErrorCheckStubData"); }}
        
        public bool GenerateClientStub { get { return Get<bool>("GenerateClientStub"); } set { Set("GenerateClientStub", value); } }
        public bool IsGenerateClientStubSpecified { get { return IsSpecified("GenerateClientStub"); }}
        
        public bool GenerateServerStub { get { return Get<bool>("GenerateServerStub"); } set { Set("GenerateServerStub", value); } }
        public bool IsGenerateServerStubSpecified { get { return IsSpecified("GenerateServerStub"); }}
        
        public bool GenerateStublessProxies { get { return Get<bool>("GenerateStublessProxies"); } set { Set("GenerateStublessProxies", value); } }
        public bool IsGenerateStublessProxiesSpecified { get { return IsSpecified("GenerateStublessProxies"); }}
        
        public bool GenerateTypeLibrary { get { return Get<bool>("GenerateTypeLibrary"); } set { Set("GenerateTypeLibrary", value); } }
        public bool IsGenerateTypeLibrarySpecified { get { return IsSpecified("GenerateTypeLibrary"); }}
        
        public string HeaderFileName { get { return Get<string>("HeaderFileName"); } set { Set("HeaderFileName", value); } }
        public bool IsHeaderFileNameSpecified { get { return IsSpecified("HeaderFileName"); }}
        
        public bool IgnoreStandardIncludePath { get { return Get<bool>("IgnoreStandardIncludePath"); } set { Set("IgnoreStandardIncludePath", value); } }
        public bool IsIgnoreStandardIncludePathSpecified { get { return IsSpecified("IgnoreStandardIncludePath"); }}
        
        public string InterfaceIdentifierFileName { get { return Get<string>("InterfaceIdentifierFileName"); } set { Set("InterfaceIdentifierFileName", value); } }
        public bool IsInterfaceIdentifierFileNameSpecified { get { return IsSpecified("InterfaceIdentifierFileName"); }}
        
        public int? LocaleID { get { return GetAsNullable<int>("LocaleID"); } set { SetAsNullable("LocaleID", value); } }
        
        public bool MkTypLibCompatible { get { return Get<bool>("MkTypLibCompatible"); } set { Set("MkTypLibCompatible", value); } }
        public bool IsMkTypLibCompatibleSpecified { get { return IsSpecified("MkTypLibCompatible"); }}
        
        public string[] PreprocessorDefinitions { get { return Get<string[]>("PreprocessorDefinitions"); } set { Set("PreprocessorDefinitions", value); } }
        public bool ArePreprocessorDefinitionsSpecified { get { return IsSpecified("PreprocessorDefinitions"); }}
        
        public string ProxyFileName { get { return Get<string>("ProxyFileName"); } set { Set("ProxyFileName", value); } }
        public bool IsProxyFileNameSpecified { get { return IsSpecified("ProxyFileName"); }}
        
        public string ServerStubFile { get { return Get<string>("ServerStubFile"); } set { Set("ServerStubFile", value); } }
        public bool IsServerStubFileSpecified { get { return IsSpecified("ServerStubFile"); }}
        
        public int? StructMemberAlignment { get { return GetAsNullable<int>("StructMemberAlignment"); } set { SetAsNullable("StructMemberAlignment", value); } }
        
        public bool SuppressCompilerWarnings { get { return Get<bool>("SuppressCompilerWarnings"); } set { Set("SuppressCompilerWarnings", value); } }
        public bool IsSuppressCompilerWarningsSpecified { get { return IsSpecified("SuppressCompilerWarnings"); }}
        
        public MidlTargetEnvironment TargetEnvironment { get { return Get<MidlTargetEnvironment>("TargetEnvironment"); } set { Set("TargetEnvironment", value); } }
        public bool IsTargetEnvironmentSpecified { get { return IsSpecified("TargetEnvironment"); }}
        
        public bool NewTypeLibFormat { get { return Get<bool>("NewTypeLibFormat"); } set { Set("NewTypeLibFormat", value); } }
        public bool IsNewTypeLibFormatSpecified { get { return IsSpecified("NewTypeLibFormat"); }}
        
        public string TypeLibraryName { get { return Get<string>("TypeLibraryName"); } set { Set("TypeLibraryName", value); } }
        public bool IsTypeLibraryNameSpecified { get { return IsSpecified("TypeLibraryName"); }}

        public string ComponentFileName { get { return Get<string>("ComponentFileName"); } set { Set("ComponentFileName", value); } }
        public bool IsComponentFileNameSpecified { get { return IsSpecified("ComponentFileName"); } }
        
        public string[] UndefinePreprocessorDefinitions { get { return Get<string[]>("UndefinePreprocessorDefinitions"); } set { Set("UndefinePreprocessorDefinitions", value); } }
        public bool AreUndefinePreprocessorDefinitionsSpecified { get { return IsSpecified("UndefinePreprocessorDefinitions"); }}
        
        public bool ValidateAllParameters { get { return Get<bool>("ValidateAllParameters"); } set { Set("ValidateAllParameters", value); } }
        public bool IsValidateAllParametersSpecified { get { return IsSpecified("ValidateAllParameters"); }}
        
        public bool WarningsAsError { get { return Get<bool>("WarningsAsError"); } set { Set("WarningsAsError", value); } }
        public bool IsWarningsAsErrorSpecified { get { return IsSpecified("WarningsAsError"); }}
        
        public WarningLevel WarningLevel { get { return Get<WarningLevel>("WarningLevel"); } set { Set("WarningLevel", value); } }
        public bool IsWarningLevelSpecified { get { return IsSpecified("WarningLevel"); }}
        

        public VCppProjectMIDLParameters(Suite suite, VCppProjectMIDLParameters parent = null)
            : base(parent)
        {
            this.suite = suite;
            /*
            PreprocessorDefinitions = suite.ActiveGoal.Has(Suite.DebugGoal.Name) ? new[] { "_DEBUG" } : new string[0];
            MkTypLibCompatible = false;
            TargetEnvironment = suite.ActiveGoal.Has("x64") ? MidlTargetEnvironment.X64 : MidlTargetEnvironment.Win32;
            GenerateStublessProxies = true;
            EnableErrorChecks = MidlErrorChecks.All;
            ErrorCheckAllocations = true;
            ErrorCheckBounds = true;
            ErrorCheckEnumRange = true;
            ErrorCheckRefPointers = true;
            ErrorCheckStubData = true;
            NewTypeLibFormat = true;
            WarningLevel = WarningLevel.All;*/
        }

        public void FillProjectSpecificMissingInfo(Project project, LocalFileSystemDirectory targetDir)
        {
        }

        public void ToVcxprojProperties(XmlWriter writer)
        {
            WriteStringArray(writer, "AdditionalIncludeDirectories", AreAdditionalIncludeDirectoriesSpecified ? AdditionalIncludeDirectories : new string[0]);

            if (AreAdditionalOptionsSpecified && AdditionalOptions.Length > 0)
                writer.WriteElementString("AdditionalOptions", String.Join(" ", AdditionalOptions));

            writer.WriteElementString("ApplicationConfigurationMode", XmlConvert.ToString(IsApplicationConfigurationModeSpecified && ApplicationConfigurationMode));

            if (IsClientStubFileSpecified)
                writer.WriteElementString("ClientStubFile", ClientStubFile);

            if (AreCPreprocessOptionsSpecified && CPreprocessOptions.Length > 0)
                writer.WriteElementString("CPreprocessOptions", String.Join(" ", CPreprocessOptions));

            writer.WriteElementString("DefaultCharType", (IsDefaultCharTypeSpecified ? DefaultCharType : CharType.Ascii).ToString());

            if (IsDllDataFileNameSpecified)
                writer.WriteElementString("DllDataFileName", DllDataFileName);

            writer.WriteElementString("EnableErrorChecks", ErrorChecksToString(IsEnableErrorChecksSpecified ? EnableErrorChecks : MidlErrorChecks.All));
            writer.WriteElementString("ErrorCheckAllocations", XmlConvert.ToString(!IsErrorCheckAllocationsSpecified || ErrorCheckAllocations));
            writer.WriteElementString("ErrorCheckBounds", XmlConvert.ToString(!IsErrorCheckBoundsSpecified || ErrorCheckBounds));
            writer.WriteElementString("ErrorCheckEnumRange", XmlConvert.ToString(!IsErrorCheckEnumRangeSpecified || ErrorCheckEnumRange));
            writer.WriteElementString("ErrorCheckRefPointers", XmlConvert.ToString(!IsErrorCheckRefPointersSpecified || ErrorCheckRefPointers));
            writer.WriteElementString("ErrorCheckStubData", XmlConvert.ToString(!IsErrorCheckStubDataSpecified || ErrorCheckStubData));
            writer.WriteElementString("GenerateClientFiles", StubToString(IsGenerateClientStubSpecified && GenerateClientStub));
            writer.WriteElementString("GenerateServerFiles", StubToString(IsGenerateServerStubSpecified && GenerateServerStub));
            writer.WriteElementString("GenerateStublessProxies", XmlConvert.ToString(!IsGenerateStublessProxiesSpecified || GenerateStublessProxies));
            writer.WriteElementString("GenerateTypeLibrary", XmlConvert.ToString(IsGenerateTypeLibrarySpecified && GenerateTypeLibrary));

            if (IsHeaderFileNameSpecified)
                writer.WriteElementString("HeaderFileName", HeaderFileName);

            writer.WriteElementString("IgnoreStandardIncludePath", XmlConvert.ToString(IsIgnoreStandardIncludePathSpecified && IgnoreStandardIncludePath));

            if (IsInterfaceIdentifierFileNameSpecified)
                writer.WriteElementString("InterfaceIdentifierFileName", InterfaceIdentifierFileName);

            if (LocaleID.HasValue)
                writer.WriteElementString("LocaleID", XmlConvert.ToString(LocaleID.Value));

            writer.WriteElementString("MkTypLibCompatible", XmlConvert.ToString(IsMkTypLibCompatibleSpecified && MkTypLibCompatible));

            string[] preprocessorDefinitions;
            if (ArePreprocessorDefinitionsSpecified)
                preprocessorDefinitions = PreprocessorDefinitions;
            else
                preprocessorDefinitions = suite.ActiveGoal.Has(Suite.DebugGoal.Name) ? new[] {"_DEBUG"} : new string[0];

            WriteStringArray(writer, "PreprocessorDefinitions", preprocessorDefinitions);

            if (IsProxyFileNameSpecified)
                writer.WriteElementString("ProxyFileName", ProxyFileName);

            if (IsServerStubFileSpecified)
                writer.WriteElementString("ServerStubFile", ServerStubFile);

            if (StructMemberAlignment.HasValue)
                writer.WriteElementString("StructMemberAlignment", XmlConvert.ToString(StructMemberAlignment.Value));

            writer.WriteElementString("SuppressCompilerWarnings", XmlConvert.ToString(IsSuppressCompilerWarningsSpecified && SuppressCompilerWarnings));

            MidlTargetEnvironment targetEnvironment;
            if (IsTargetEnvironmentSpecified)
                targetEnvironment = TargetEnvironment;
            else
                targetEnvironment = suite.ActiveGoal.Has("x64")
                    ? MidlTargetEnvironment.X64
                    : MidlTargetEnvironment.Win32;
            writer.WriteElementString("TargetEnvironment", targetEnvironment.ToString());

            writer.WriteElementString("TypeLibFormat", TypeLibFormatToString(!IsNewTypeLibFormatSpecified || NewTypeLibFormat));

            if (IsTypeLibraryNameSpecified)
                writer.WriteElementString("TypeLibraryName", TypeLibraryName);

            if (IsComponentFileNameSpecified)
                writer.WriteElementString("ComponentFileName", ComponentFileName);

            if (AreUndefinePreprocessorDefinitionsSpecified)
                WriteStringArray(writer, "UndefinePreprocessorDefinitions", UndefinePreprocessorDefinitions);

            writer.WriteElementString("ValidateAllParameters", XmlConvert.ToString(IsValidateAllParametersSpecified && ValidateAllParameters));
            writer.WriteElementString("WarnAsError", XmlConvert.ToString(IsWarningsAsErrorSpecified && WarningsAsError));
            writer.WriteElementString("WarningLevel", XmlConvert.ToString((int)(IsWarningLevelSpecified ? WarningLevel : WarningLevel.All)));

            writer.WriteElementString("OutputDirectory", "$(IntDir)");
        }

        private string TypeLibFormatToString(bool isNew)
        {
            if (isNew)
                return "NewFormat";
            else
                return "OldFormat";
        }

        private string StubToString(bool stub)
        {
            if (stub)
                return "Stub";
            else
                return "None";
        }

        private string ErrorChecksToString(MidlErrorChecks enableErrorChecks)
        {
            switch (enableErrorChecks)
            {
                case MidlErrorChecks.None:
                    return "None";
                case MidlErrorChecks.Custom:
                    return "EnableCustom";
                case MidlErrorChecks.All:
                    return "All";
                default:
                    throw new ArgumentOutOfRangeException("enableErrorChecks");
            }
        }
    }
}