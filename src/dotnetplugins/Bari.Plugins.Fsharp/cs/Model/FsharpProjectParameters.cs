using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.Fsharp.Model
{
    public class FsharpProjectParametersDef : ProjectParametersPropertyDefs<FsharpProjectParameters>
    {
        public FsharpProjectParametersDef()
        {
            Define<uint>("BaseAddress");
            Define<string>("CodePage");
            Define<bool>("DebugSymbols");
            Define<DebugLevel>("Debug");
            Define<string[]>("Defines", mergeWithInherited: true);
            Define<bool>("DelaySign");
            Define<string>("DocOutput");
            Define<int[]>("SuppressedWarnings", mergeWithInherited: true);
            Define<string>("KeyFile");
            Define<bool>("Optimize");
            Define<CLRPlatform>("Platform");
            Define<string>("OtherFlags");
            Define<bool>("Tailcalls");
            Define<WarningLevel>("WarningLevel");
            Define<bool>("AllWarningsAsError");
            Define<int[]>("SpecificWarningsAsError", mergeWithInherited: true);
            Define<bool>("HighEntropyVirtualAddressSpace");
            Define<FrameworkVersion>("TargetFrameworkVersion");
            Define<FrameworkProfile>("TargetFrameworkProfile");
        }

        public override FsharpProjectParameters CreateDefault(Suite suite, FsharpProjectParameters parent)
        {
            return new FsharpProjectParameters(suite, parent);
        }
    }


    // TODO: merge common code with CsharpProjectParameters
    public class FsharpProjectParameters : InheritableProjectParameters<FsharpProjectParameters, FsharpProjectParametersDef>
    {
        private readonly Suite suite;

        public uint? BaseAddress
        {
            get { return GetAsNullable<uint>("BaseAddress"); }
            set { SetAsNullable("BaseAddress", value); }
        }

        public string CodePage
        {
            get { return Get<string>("CodePage"); }
            set { Set("CodePage", value); }
        }

        public bool IsCodePageSpecified { get { return IsSpecified("CodePage"); } }

        public bool DebugSymbols
        {
            get { return Get<bool>("DebugSymbols"); }
            set { Set("DebugSymbols", value); }
        }

        public bool IsDebugSymbolsSpecified { get { return IsSpecified("DebugSymbols"); } }

        public DebugLevel Debug
        {
            get { return Get<DebugLevel>("Debug"); }
            set { Set("Debug", value); }
        }

        public bool IsDebugSpecified { get { return IsSpecified("Debug"); } }

        public string[] Defines
        {
            get { return Get<string[]>("Defines"); }
            set { Set("Defines", value); }
        }

        public bool AreDefinesSpecified { get { return IsSpecified("Defines"); } }

        public string DocOutput
        {
            get { return Get<string>("DocOutput"); }
            set { Set("DocOutput", value); }
        }

        public bool IsDocOutputSpecified { get { return IsSpecified("DocOutput"); } }

        public int[] SuppressedWarnings
        {
            get { return Get<int[]>("SuppressedWarnings"); }
            set { Set("SuppressedWarnings", value); }
        }

        public bool AreSuppressedWarningsSpecified { get { return IsSpecified("SuppressedWarnings"); } }

        public string KeyFile
        {
            get { return Get<string>("KeyFile"); }
            set { Set("KeyFile", value); }
        }

        public bool IsKeyFileSpecified { get { return IsSpecified("KeyFile"); } }

        public bool Optimize
        {
            get { return Get<bool>("Optimize"); }
            set { Set("Optimize", value); }
        }

        public bool IsOptimizeSpecified { get { return IsSpecified("Optimize"); } }

        public string OtherFlags
        {
            get { return Get<string>("OtherFlags"); }
            set { Set("OtherFlags", value); }
        }

        public bool AreOtherFlagsSpecified { get { return IsSpecified("OtherFlags"); } }


        public CLRPlatform Platform
        {
            get { return Get<CLRPlatform>("Platform"); }
            set { Set("Platform", value); }
        }

        public bool IsPlatformSpecified { get { return IsSpecified("Platform"); } }

        public bool Tailcalls
        {
            get { return Get<bool>("Tailcalls"); }
            set { Set("Tailcalls", value); }
        }

        public bool IsTailcallsSpecified { get { return IsSpecified("Tailcalls"); } }


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

        public bool HighEntropyVirtualAddressSpace
        {
            get { return Get<bool>("HighEntropyVirtualAddressSpace"); }
            set { Set("HighEntropyVirtualAddressSpace", value); }
        }

        public bool IsHighEntropyVirtualAddressSpaceSpecified { get { return IsSpecified("HighEntropyVirtualAddressSpace"); } }

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

        public FsharpProjectParameters(Suite suite, FsharpProjectParameters parent = null)
            : base(parent)
        {
            this.suite = suite;
        }

        public void FillProjectSpecificMissingInfo(Project project)
        {                     
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

        public void ToFsprojProperties(XmlWriter writer)
        {
            if (BaseAddress.HasValue)
                writer.WriteElementString("BaseAddress", "0x"+BaseAddress.Value.ToString("X", CultureInfo.InvariantCulture));
            if (IsCodePageSpecified)
                writer.WriteElementString("CodePage", CodePage);
            writer.WriteElementString("DebugSymbols",
                XmlConvert.ToString(IsDebugSymbolsSpecified ? DebugSymbols : suite.ActiveGoal.Has(Suite.DebugGoal.Name)));

            var debug = IsDebugSpecified
                ? Debug
                : suite.ActiveGoal.Has(Suite.DebugGoal.Name) ? DebugLevel.Full : DebugLevel.None;
            writer.WriteElementString("DebugType", debug.ToString().ToLowerInvariant());

            if (AreDefinesSpecified)
                writer.WriteElementString("DefineConstants", string.Join(";", Defines));

            if (IsDocOutputSpecified)
                writer.WriteElementString("DocumentationFile", DocOutput);

            if (IsHighEntropyVirtualAddressSpaceSpecified)
                writer.WriteElementString("HighEntropyVA", XmlConvert.ToString(HighEntropyVirtualAddressSpace));

            if (IsKeyFileSpecified)
                writer.WriteElementString("KeyOriginatorFile", KeyFile);

            if (AreSuppressedWarningsSpecified)
                writer.WriteElementString("NoWarn", 
                    String.Join(";", SuppressedWarnings.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));

            writer.WriteElementString("Optimize",
                XmlConvert.ToString(IsOptimizeSpecified ? Optimize : suite.ActiveGoal.Has(Suite.ReleaseGoal.Name)));
            writer.WriteElementString("Tailcalls",
                XmlConvert.ToString(IsTailcallsSpecified ? Tailcalls : suite.ActiveGoal.Has(Suite.ReleaseGoal.Name)));

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

            writer.WriteElementString("WarningLevel",
                XmlConvert.ToString((int) (IsWarningLevelSpecified ? WarningLevel : WarningLevel.All)));

            writer.WriteElementString("TreatWarningsAsErrors", XmlConvert.ToString(IsAllWarningsAsErrorSpecified && AllWarningsAsError));
            if (AreSpecificWarningsAsErrorSpecified)
                writer.WriteElementString("WarningsAsErrors",
                                          String.Join(";", SpecificWarningsAsError.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));
            if (AreOtherFlagsSpecified)
                writer.WriteElementString("OtherFlags", OtherFlags);

            var targetFrameworkVersion = IsTargetFrameworkVersionSpecified
               ? TargetFrameworkVersion
               : FrameworkVersion.v4;
            writer.WriteElementString("TargetFrameworkVersion", ToFrameworkVersion(targetFrameworkVersion));

            var targetFrameworkProfile = IsTargetFrameworkProfileSpecified
                ? TargetFrameworkProfile
                : FrameworkProfile.Default;
            writer.WriteElementString("TargetFrameworkProfile", ToFrameworkProfile(targetFrameworkProfile));

            writer.WriteStartElement("MinimumVisualStudioVersion");
            writer.WriteAttributeString("Condition", "'$(MinimumVisualStudioVersion)' == '11.0'");
            writer.WriteString("11");
            writer.WriteEndElement();
        }
    }
}