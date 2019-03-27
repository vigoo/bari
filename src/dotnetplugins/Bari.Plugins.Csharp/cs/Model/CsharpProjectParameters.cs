using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.Csharp.Model
{
    public class CsharpProjectParametersDef : ProjectParametersPropertyDefs<CsharpProjectParameters>
    {
        public CsharpProjectParametersDef()
        {
            Define<uint>("BaseAddress");
            Define<bool>("Checked");
            Define<string>("CodePage");
            Define<DebugLevel>("Debug");
            Define<string[]>("Defines", mergeWithInherited: true);
            Define<bool>("DelaySign");
            Define<string>("DocOutput");
            Define<uint>("FileAlign");
            Define<bool>("HighEntropyVirtualAddressSpace");
            Define<string>("KeyContainer");
            Define<string>("KeyFile");
            Define<CsharpLanguageVersion>("LanguageVersion");
            Define<string>("MainClass");
            Define<bool>("NoStdLib");
            Define<int[]>("SuppressedWarnings", mergeWithInherited: true);
            Define<bool>("NoWin32Manifest");
            Define<bool>("Optimize");
            Define<CLRPlatform>("Platform");
            Define<string>("PreferredUILang");            
            Define<string>("SubsystemVersion");
            Define<bool>("Unsafe");
            Define<WarningLevel>("WarningLevel");
            Define<bool>("AllWarningsAsError");
            Define<int[]>("SpecificWarningsAsError", mergeWithInherited: true);
            Define<string>("RootNamespace");
            Define<string>("ApplicationIcon");
            Define<FrameworkVersion>("TargetFrameworkVersion");
            Define<FrameworkProfile>("TargetFrameworkProfile");
        }

        public override CsharpProjectParameters CreateDefault(Suite suite, CsharpProjectParameters parent)
        {
            return new CsharpProjectParameters(suite, parent);
        }
    }

    public class CsharpProjectParameters: InheritableProjectParameters<CsharpProjectParameters, CsharpProjectParametersDef>
    {
        private readonly Suite suite;

        public uint? BaseAddress
        {
            get { return GetAsNullable<uint>("BaseAddress"); }
            set { SetAsNullable("BaseAddress", value); }
        }

        public bool Checked
        {
            get { return Get<bool>("Checked"); }
            set { Set("Checked", value); }
        }

        public bool IsCheckedSpecified { get { return IsSpecified("Checked"); } }

        public string CodePage
        {
            get { return Get<string>("CodePage"); }
            set { Set("CodePage", value); }
        }

        public bool IsCodePageSpecified { get { return IsSpecified("CodePage"); } }

        public DebugLevel Debug
        {
            get { return Get<DebugLevel>("Debug"); }
            set { Set("Debug", value); }
        }

        public bool IsDebugSpecified { get { return IsSpecified("Debug"); } }

        public string[] Defines
        {
            get { return Get<string[]>("Defines");}
            set { Set("Defines", value); }
        }

        public bool AreDefinesSpecified { get { return IsSpecified("Defines"); } }

        public bool DelaySign
        {
            get { return Get<bool>("DelaySign"); }
            set { Set("DelaySign", value); }
        }

        public bool IsDelaySignSpecified { get { return IsSpecified("DelaySign"); } }

        public string DocOutput
        {
            get { return Get<string>("DocOutput"); }
            set { Set("DocOutput", value); }
        }

        public bool IsDocOutputSpecified { get { return IsSpecified("DocOutput"); } }

        public uint? FileAlign
        {
            get { return GetAsNullable<uint>("FileAlign"); }
            set { SetAsNullable("FileAlign", value); }
        }

        public bool HighEntropyVirtualAddressSpace
        {
            get { return Get<bool>("HighEntropyVirtualAddressSpace"); }
            set { Set("HighEntropyVirtualAddressSpace", value); }
        }

        public bool IsHighEntropyVirtualAddressSpaceSpecified { get { return IsSpecified("HighEntropyVirtualAddressSpace"); } }

        public string KeyContainer
        {
            get { return Get<string>("KeyContainer"); }
            set { Set("KeyContainer", value); }
        }

        public bool IsKeyContainerSpecified { get { return IsSpecified("KeyContainer"); } }

        public string KeyFile
        {
            get { return Get<string>("KeyFile"); }
            set { Set("KeyFile", value); }
        }

        public bool IsKeyFileSpecified { get { return IsSpecified("KeyFile"); } }

        public CsharpLanguageVersion LanguageVersion
        {
            get { return Get<CsharpLanguageVersion>("LanguageVersion"); }
            set { Set("LanguageVersion", value); }
        }

        public bool IsLanguageVersionSpecified { get { return IsSpecified("LanguageVersion"); } }

        public string MainClass
        {
            get { return Get<string>("MainClass"); }
            set { Set("MainClass", value); }
        }

        public bool IsMainClassSpecified { get { return IsSpecified("MainClass"); } }

        public bool NoStdLib
        {
            get { return Get<bool>("NoStdLib"); }
            set { Set("NoStdLib", value); }
        }

        public bool IsNoStdLibSpecified { get { return IsSpecified("NoStdLib"); } }

        public int[] SuppressedWarnings
        {
            get { return Get<int[]>("SuppressedWarnings"); }
            set { Set("SuppressedWarnings", value); }
        }

        public bool AreSuppressedWarningsSpecified { get { return IsSpecified("SuppressedWarnings"); } }

        public bool NoWin32Manifest
        {
            get { return Get<bool>("NoWin32Manifest"); }
            set { Set("NoWin32Manifest", value); }
        }

        public bool IsNoWin32ManifestSpecified { get { return IsSpecified("NoWin32Manifest"); } }

        public bool Optimize
        {
            get { return Get<bool>("Optimize"); }
            set { Set("Optimize", value); }
        }

        public bool IsOptimizeSpecified { get { return IsSpecified("Optimize"); } }

        public CLRPlatform Platform
        {
            get { return Get<CLRPlatform>("Platform"); }
            set { Set("Platform", value); }
        }

        public bool IsPlatformSpecified { get { return IsSpecified("Platform"); } }

        public string PreferredUILang
        {
            get { return Get<string>("PreferredUILang"); }
            set { Set("PreferredUILang", value); }
        }

        public bool IsPreferredUILangSpecified { get { return IsSpecified("PreferredUILang"); } }

        public string SubsystemVersion
        {
            get { return Get<string>("SubsystemVersion"); }
            set { Set("SubsystemVersion", value); }
        }

        public bool IsSubsystemVersionSpecified { get { return IsSpecified("SubsystemVersion"); } }

        public bool Unsafe
        {
            get { return Get<bool>("Unsafe"); }
            set { Set("Unsafe", value); }
        }

        public bool IsUnsafeSpecified { get { return IsSpecified("Unsafe"); } }

        public WarningLevel WarningLevel
        {
            get { return Get<WarningLevel>("WarningLevel"); }
            set { Set("WarningLevel", value); }
        }

        public bool IsWarningLevelSpecified { get { return IsSpecified("WarningLevel"); } }

        public bool AllWarningsAsError
        {
            get { return Get<bool>("AllWarningsAsError"); }
            set { Set("AllWarningsAsError", value); }
        }

        public bool IsAllWarningsAsErrorSpecified { get { return IsSpecified("AllWarningsAsError"); } }

        public int[] SpecificWarningsAsError
        {
            get { return Get<int[]>("SpecificWarningsAsError"); }
            set { Set("SpecificWarningsAsError", value); }
        }

        public bool AreSpecificWarningsAsErrorSpecified { get { return IsSpecified("SpecificWarningsAsError"); } }

        public string RootNamespace
        {
            get { return Get<string>("RootNamespace"); }
            set { Set("RootNamespace", value); }
        }

        public bool IsRootNamespaceSpecified { get { return IsSpecified("RootNamespace"); } }

        public string ApplicationIcon
        {
            get { return Get<string>("ApplicationIcon"); }
            set { Set("ApplicationIcon", value); }
        }

        public bool IsApplicationIconSpecified { get { return IsSpecified("ApplicationIcon"); } }

        public FrameworkVersion TargetFrameworkVersion
        {
            get { return Get<FrameworkVersion>("TargetFrameworkVersion"); }
            set { Set("TargetFrameworkVersion", value); }
        }

        public bool IsTargetFrameworkVersionSpecified { get { return IsSpecified("TargetFrameworkVersion"); } }

        public FrameworkProfile TargetFrameworkProfile
        {
            get { return Get<FrameworkProfile>("TargetFrameworkProfile"); }
            set { Set("TargetFrameworkProfile", value); }
        }

        public bool IsTargetFrameworkProfileSpecified { get { return IsSpecified("TargetFrameworkProfile"); } }

        public CsharpProjectParameters(Suite suite, CsharpProjectParameters parent = null)
            : base(parent)
        {
            this.suite = suite;
        }

        public void FillProjectSpecificMissingInfo(Project project)
        {
            if (!IsRootNamespaceSpecified)
                RootNamespace = project.Name;

            if (project.HasNonEmptySourceSet("resources"))
            {
                var resources = project.GetSourceSet("resources");
                var icons = resources.Files.Where(p => Path.GetExtension(p) == ".ico").ToList();

                if (icons.Count == 1 && !IsApplicationIconSpecified)
                {
                    ApplicationIcon = project.Module.Suite.SuiteRoot.GetRelativePathFrom(project.RootDirectory.GetChildDirectory("resources"), icons[0]);
                }
            }
        }

        public void ToCsprojProperties(XmlWriter writer)
        {
            if (BaseAddress.HasValue)
                writer.WriteElementString("BaseAddress", "0x"+BaseAddress.Value.ToString("X", CultureInfo.InvariantCulture));
            
            writer.WriteElementString("CheckForOverflowUnderflow", XmlConvert.ToString(IsCheckedSpecified && Checked));
            
            if (IsCodePageSpecified)
                writer.WriteElementString("CodePage", CodePage);

            DebugLevel debug;
            if (IsDebugSpecified)
                debug = Debug;
            else if (suite.ActiveGoal.Has(Suite.DebugGoal.Name))
                debug = DebugLevel.Full;
            else
                debug = DebugLevel.None;
            writer.WriteElementString("DebugType", debug.ToString().ToLowerInvariant());

            string[] defines;
            if (AreDefinesSpecified)
                defines = Defines;
            else if (suite.ActiveGoal.Has(Suite.DebugGoal.Name))
                defines = new[] {"DEBUG"};
            else
                defines = new string[0];
            writer.WriteElementString("DefineConstants", string.Join(";", defines));

            writer.WriteElementString("DelaySign", XmlConvert.ToString(IsDelaySignSpecified && DelaySign));

            if (IsDocOutputSpecified)
                writer.WriteElementString("DocumentationFile", DocOutput);

            if (FileAlign.HasValue)
                writer.WriteElementString("FileAlignment", XmlConvert.ToString(FileAlign.Value));

            writer.WriteElementString("HighEntropyVA", XmlConvert.ToString(IsHighEntropyVirtualAddressSpaceSpecified && HighEntropyVirtualAddressSpace));

            if (IsKeyContainerSpecified)
                writer.WriteElementString("KeyContainerName", KeyContainer);

            if (IsKeyFileSpecified)
                writer.WriteElementString("KeyOriginatorFile", KeyFile);

            writer.WriteElementString("LangVersion", ToParameter(IsLanguageVersionSpecified ? LanguageVersion : CsharpLanguageVersion.Default));

            if (IsMainClassSpecified)
                writer.WriteElementString("StartupObject", MainClass);

            if (IsNoStdLibSpecified && NoStdLib)
                writer.WriteElementString("NoCompilerStandardLib", XmlConvert.ToString(NoStdLib));

            if (AreSuppressedWarningsSpecified)
                writer.WriteElementString("NoWarn", String.Join(";", SuppressedWarnings.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));

            writer.WriteElementString("NoWin32Manifest", XmlConvert.ToString(IsNoWin32ManifestSpecified && NoWin32Manifest));

            bool optimize = IsOptimizeSpecified ? Optimize : suite.ActiveGoal.Has(Suite.ReleaseGoal.Name);
            writer.WriteElementString("Optimize", XmlConvert.ToString(optimize));

            CLRPlatform platform;
            if (IsPlatformSpecified)
                platform = Platform;
            else if (suite.ActiveGoal.Has("x86"))
                platform = CLRPlatform.x86;
            else if (suite.ActiveGoal.Has("x64"))
                platform = CLRPlatform.x64;
            else
                platform = CLRPlatform.AnyCPU;
            writer.WriteElementString("PlatformTarget", platform.ToString().ToLowerInvariant());

            if (IsPreferredUILangSpecified)
                writer.WriteElementString("PreferredUILang", PreferredUILang);

            if (IsSubsystemVersionSpecified)
                writer.WriteElementString("SubsystemVersion", SubsystemVersion);

            writer.WriteElementString("AllowUnsafeBlocks", XmlConvert.ToString(IsUnsafeSpecified && Unsafe));

            WarningLevel warningLevel = IsWarningLevelSpecified ? WarningLevel : WarningLevel.All;            
            writer.WriteElementString("WarningLevel", XmlConvert.ToString((int)warningLevel));

            writer.WriteElementString("TreatWarningsAsErrors", XmlConvert.ToString(IsAllWarningsAsErrorSpecified && AllWarningsAsError));

            if (AreSpecificWarningsAsErrorSpecified)
                writer.WriteElementString("WarningsAsErrors",
                                          String.Join(";", SpecificWarningsAsError.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));
            
            if (IsRootNamespaceSpecified)
                writer.WriteElementString("RootNamespace", RootNamespace);

            var targetFrameworkVersion = IsTargetFrameworkVersionSpecified
                ? TargetFrameworkVersion
                : FrameworkVersion.v4;
            writer.WriteElementString("TargetFrameworkVersion", ToFrameworkVersion(targetFrameworkVersion));

            var targetFrameworkProfile = IsTargetFrameworkProfileSpecified
                ? TargetFrameworkProfile
                : FrameworkProfile.Default;
            writer.WriteElementString("TargetFrameworkProfile", ToFrameworkProfile(targetFrameworkProfile));
        }

        private string ToFrameworkProfile(FrameworkProfile targetFrameworkProfile)
        {
            switch (targetFrameworkProfile)
            {
                case FrameworkProfile.Default:
                    return string.Empty;
                case FrameworkProfile.Client:
                    return "client";
                default:
                    throw new ArgumentOutOfRangeException("targetFrameworkProfile");
            }
        }

        private string ToFrameworkVersion(FrameworkVersion targetFrameworkVersion)
        {
            switch (targetFrameworkVersion)
            {
                case FrameworkVersion.v20:
                    return "v2.0";
                case FrameworkVersion.v30:
                    return "v3.0";
                case FrameworkVersion.v35:
                    return "v3.5";
                case FrameworkVersion.v4:
                    return "v4.0";
                case FrameworkVersion.v45:
                    return "v4.5";
                case FrameworkVersion.v451:
                    return "v4.5.1";
                case FrameworkVersion.v452:
                    return "v4.5.2";
                case FrameworkVersion.v46:
                    return "v4.6";
                case FrameworkVersion.v461:
                    return "v4.6.1";
                case FrameworkVersion.v462:
                    return "v4.6.2";
                case FrameworkVersion.v47:
                    return "v4.7";
                case FrameworkVersion.v471:
                    return "v4.7.1";
                default:
                    throw new ArgumentOutOfRangeException("targetFrameworkVersion");
            }
        }

        private string ToParameter(CsharpLanguageVersion languageVersion)
        {
            switch (languageVersion)
            {
                case CsharpLanguageVersion.Default:
                    return "Default";
                case CsharpLanguageVersion.ISO1:
                    return "ISO-1";
                case CsharpLanguageVersion.ISO2:
                    return "ISO-2";
                case CsharpLanguageVersion.V3:
                    return "3";
                case CsharpLanguageVersion.V4:
                    return "4";
                case CsharpLanguageVersion.V5:
                    return "5";
                case CsharpLanguageVersion.V6:
                    return "6";
                case CsharpLanguageVersion.V7:
                    return "7";
                case CsharpLanguageVersion.V71:
                    return "7.1";
                case CsharpLanguageVersion.V72:
                    return "7.2";
                case CsharpLanguageVersion.V73:
                    return "7.3";
                default:
                    throw new ArgumentOutOfRangeException("languageVersion");
            }
        }
    }
}