using System;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectMIDLParameters : VCppProjectParametersBase
    {
        public string[] AdditionalIncludeDirectories { get; set; }
        public string[] AdditionalOptions { get; set; }
        public bool ApplicationConfigurationMode { get; set; }
        public string ClientStubFile { get; set; }
        public string[] CPreprocessOptions { get; set; }
        public CharType DefaultCharType { get; set; }
        public string DllDataFileName { get; set; }
        public MidlErrorChecks EnableErrorChecks { get; set; }
        public bool ErrorCheckAllocations { get; set; }
        public bool ErrorCheckBounds { get; set; }
        public bool ErrorCheckEnumRange { get; set; }
        public bool ErrorCheckRefPointers { get; set; }
        public bool ErrorCheckStubData { get; set; }
        public bool GenerateClientStub { get; set; }
        public bool GenerateServerStub { get; set; }
        public bool GenerateStublessProxies { get; set; }
        public bool GenerateTypeLibrary { get; set; }
        public string HeaderFileName { get; set; }
        public bool IgnoreStandardIncludePath { get; set; }
        public string InterfaceIdentifierFileName { get; set; }
        public int? LocaleID { get; set; }
        public bool MkTypLibCompatible { get; set; }
        public string[] PreprocessorDefinitions { get; set; }
        public string ProxyFileName { get; set; }
        public string ServerStubFile { get; set; }
        public int? StructMemberAlignment { get; set; }
        public bool SuppressCompilerWarnings { get; set; }
        public MidlTargetEnvironment TargetEnvironment { get; set; }
        public bool NewTypeLibFormat { get; set; }
        public string TypeLibraryName { get; set; }
        public string[] UndefinePreprocessorDefinitions { get; set; }
        public bool ValidateAllParameters { get; set; }
        public bool WarningsAsError { get; set; }
        public WarningLevel WarningLevel { get; set; }

        public VCppProjectMIDLParameters(Suite suite)
        {
            ApplicationConfigurationMode = false;
            PreprocessorDefinitions = suite.ActiveGoal.Has(Suite.DebugGoal.Name) ? new[] {"_DEBUG"} : new string[0];
            MkTypLibCompatible = false;
            TargetEnvironment = MidlTargetEnvironment.Win32;
            GenerateStublessProxies = true;            
            EnableErrorChecks = MidlErrorChecks.All;
            ErrorCheckAllocations = true;
            ErrorCheckBounds = true;
            ErrorCheckEnumRange = true;
            ErrorCheckRefPointers = true;
            ErrorCheckStubData = true;
            NewTypeLibFormat = true;
            WarningLevel = WarningLevel.All;
        }

        public void FillProjectSpecificMissingInfo(Project project, LocalFileSystemDirectory targetDir)
        {
        }

        public void ToVcxprojProperties(XmlWriter writer)
        {
            WriteStringArray(writer, "AdditionalIncludeDirectories", AdditionalIncludeDirectories);

            if (AdditionalOptions != null && AdditionalOptions.Length > 0)
                writer.WriteElementString("AdditionalOptions", String.Join(" ", AdditionalOptions));
            
            writer.WriteElementString("ApplicationConfigurationMode", XmlConvert.ToString(ApplicationConfigurationMode));

            if (!String.IsNullOrWhiteSpace(ClientStubFile))
                writer.WriteElementString("ClientStubFile", ClientStubFile);

            if (CPreprocessOptions != null && CPreprocessOptions.Length > 0)
                writer.WriteElementString("CPreprocessOptions", String.Join(" ", CPreprocessOptions));

            writer.WriteElementString("DefaultCharType", DefaultCharType.ToString());

            if (!String.IsNullOrWhiteSpace(DllDataFileName))
                writer.WriteElementString("DllDataFileName", DllDataFileName);

            writer.WriteElementString("EnableErrorChecks", ErrorChecksToString(EnableErrorChecks));
            writer.WriteElementString("ErrorCheckAllocations", XmlConvert.ToString(ErrorCheckAllocations));
            writer.WriteElementString("ErrorCheckBounds", XmlConvert.ToString(ErrorCheckBounds));
            writer.WriteElementString("ErrorCheckEnumRange", XmlConvert.ToString(ErrorCheckEnumRange));
            writer.WriteElementString("ErrorCheckRefPointers", XmlConvert.ToString(ErrorCheckRefPointers));
            writer.WriteElementString("ErrorCheckStubData", XmlConvert.ToString(ErrorCheckStubData));
            writer.WriteElementString("GenerateClientFiles", StubToString(GenerateClientStub));
            writer.WriteElementString("GenerateServerFiles", StubToString(GenerateServerStub));
            writer.WriteElementString("GenerateStublessProxies", XmlConvert.ToString(GenerateStublessProxies));
            writer.WriteElementString("GenerateTypeLibrary", XmlConvert.ToString(GenerateTypeLibrary));

            if (!String.IsNullOrWhiteSpace(HeaderFileName))
                writer.WriteElementString("HeaderFileName", HeaderFileName);

            writer.WriteElementString("IgnoreStandardIncludePath", XmlConvert.ToString(IgnoreStandardIncludePath));

            if (!String.IsNullOrWhiteSpace(InterfaceIdentifierFileName))
                writer.WriteElementString("InterfaceIdentifierFileName", InterfaceIdentifierFileName);

            if (LocaleID.HasValue)
                writer.WriteElementString("LocaleID", XmlConvert.ToString(LocaleID.Value));
            writer.WriteElementString("MkTypLibCompatible", XmlConvert.ToString(MkTypLibCompatible));

            WriteStringArray(writer, "PreprocessorDefinitions", PreprocessorDefinitions);

            if (!String.IsNullOrWhiteSpace(ProxyFileName))
                writer.WriteElementString("ProxyFileName", ProxyFileName);

            if (!String.IsNullOrWhiteSpace(ServerStubFile))
                writer.WriteElementString("ServerStubFile", ServerStubFile);

            if (StructMemberAlignment.HasValue)
                writer.WriteElementString("StructMemberAlignment", XmlConvert.ToString(StructMemberAlignment.Value));

            writer.WriteElementString("SuppressCompilerWarnings", XmlConvert.ToString(SuppressCompilerWarnings));
            writer.WriteElementString("TargetEnvironment", TargetEnvironment.ToString());
            writer.WriteElementString("TypeLibFormat", TypeLibFormatToString(NewTypeLibFormat));

            if (!String.IsNullOrWhiteSpace(TypeLibraryName))
                writer.WriteElementString("TypeLibraryName", TypeLibraryName);

            WriteStringArray(writer, "UndefinePreprocessorDefinitions", UndefinePreprocessorDefinitions);
            writer.WriteElementString("ValidateAllParameters", XmlConvert.ToString(ValidateAllParameters));
            writer.WriteElementString("WarnAsError", XmlConvert.ToString(WarningsAsError));
            writer.WriteElementString("WarningLevel", XmlConvert.ToString((int)WarningLevel));

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