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
    public class CsharpProjectParameters: IProjectParameters
    {
        public uint? BaseAddress { get; set; }
        public bool Checked { get; set; }
        public string CodePage { get; set; }
        public DebugLevel Debug { get; set; }
        public string[] Defines { get; set; }
        public bool DelaySign { get; set; }
        public string DocOutput { get; set; }
        public uint? FileAlign { get; set; }
        public bool HighEntropyVirtualAddressSpace { get; set; }
        public string KeyContainer { get; set; }
        public string KeyFile { get; set; }
        public CsharpLanguageVersion LanguageVersion { get; set; }
        public string MainClass { get; set; }
        public bool NoStdLib { get; set; }
        public int[] SuppressedWarnings { get; set; }
        public bool NoWin32Manifest { get; set; }
        public bool Optimize { get; set; }
        public CLRPlatform Platform { get; set; }
        public string PreferredUILang { get; set; }
        public string SubsystemVersion { get; set; }
        public bool Unsafe { get; set; }
        public WarningLevel WarningLevel { get; set; }
        public bool AllWarningsAsError { get; set; }
        public int[] SpecificWarningsAsError { get; set; }
        public string RootNamespace { get; set; }
        public string ApplicationIcon { get; set; }
        public FrameworkVersion TargetFrameworkVersion { get; set; }
        public FrameworkProfile TargetFrameworkProfile { get; set; }

        public CsharpProjectParameters(Suite suite)
        {
            BaseAddress = null;
            Checked = false;
            CodePage = null;            
            DelaySign = false;
            DocOutput = null;
            FileAlign = null;
            HighEntropyVirtualAddressSpace = false;
            KeyContainer = null;
            KeyFile = null;
            LanguageVersion = CsharpLanguageVersion.Default;
            MainClass = null;
            NoStdLib = false;
            SuppressedWarnings = null;
            NoWin32Manifest = false;
            
            if (suite.ActiveGoal.Has("x86"))
                Platform = CLRPlatform.x86;
            else if (suite.ActiveGoal.Has("x64"))
                Platform = CLRPlatform.x64;
            else
                Platform = CLRPlatform.AnyCPU;

            PreferredUILang = null;
            SubsystemVersion = null;
            Unsafe = false;
            WarningLevel = WarningLevel.All;
            AllWarningsAsError = false;
            SpecificWarningsAsError = null;

            if (suite.ActiveGoal.Has(Suite.DebugGoal.Name))
            {
                Debug = DebugLevel.Full;
                Optimize = false;
                Defines = new[] { "DEBUG" };
            }
            else if (suite.ActiveGoal.Has(Suite.ReleaseGoal.Name))
            {
                Debug = DebugLevel.None;
                Optimize = true;
                Defines = new string[0];
            }

            TargetFrameworkVersion = FrameworkVersion.v4;
            TargetFrameworkProfile = FrameworkProfile.Default;
        }

        public void FillProjectSpecificMissingInfo(Project project)
        {
            if (RootNamespace == null)
                RootNamespace = project.Name;

            if (project.HasNonEmptySourceSet("resources"))
            {
                var resources = project.GetSourceSet("resources");
                var icons = resources.Files.Where(p => Path.GetExtension(p) == ".ico").ToList();

                if (icons.Count == 1 && ApplicationIcon == null)
                {
                    ApplicationIcon = project.Module.Suite.SuiteRoot.GetRelativePathFrom(project.RootDirectory.GetChildDirectory("resources"), icons[0]);
                }
            }
        }

        public void ToCsprojProperties(XmlWriter writer)
        {
            if (BaseAddress.HasValue)
                writer.WriteElementString("BaseAddress", "0x"+BaseAddress.Value.ToString("X", CultureInfo.InvariantCulture));
            writer.WriteElementString("CheckForOverflowUnderflow", XmlConvert.ToString(Checked));
            if (CodePage != null)
                writer.WriteElementString("CodePage", CodePage);
            writer.WriteElementString("DebugType", Debug.ToString().ToLowerInvariant());
            if (Defines != null)
                writer.WriteElementString("DefineConstants", string.Join(";", Defines));
            writer.WriteElementString("DelaySign", XmlConvert.ToString(DelaySign));
            if (DocOutput != null)
                writer.WriteElementString("DocumentationFile", DocOutput);
            if (FileAlign.HasValue)
                writer.WriteElementString("FileAlignment", XmlConvert.ToString(FileAlign.Value));
            writer.WriteElementString("HighEntropyVA", XmlConvert.ToString(HighEntropyVirtualAddressSpace));
            if (KeyContainer != null)
                writer.WriteElementString("KeyContainerName", KeyContainer);
            if (KeyFile != null)
                writer.WriteElementString("KeyOriginatorFile", KeyFile);
            writer.WriteElementString("LangVersion", ToParameter(LanguageVersion));
            if (MainClass != null)
                writer.WriteElementString("StartupObject", MainClass);
            if (NoStdLib)
                writer.WriteElementString("NoCompilerStandardLib", XmlConvert.ToString(NoStdLib));
            if (SuppressedWarnings != null)
                writer.WriteElementString("NoWarn", 
                    String.Join(";", SuppressedWarnings.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));
            writer.WriteElementString("NoWin32Manifest", XmlConvert.ToString(NoWin32Manifest));
            writer.WriteElementString("Optimize", XmlConvert.ToString(Optimize));
            writer.WriteElementString("PlatformTarget", Platform.ToString().ToLowerInvariant());
            if (PreferredUILang != null)
                writer.WriteElementString("PreferredUILang", PreferredUILang);
            if (SubsystemVersion != null)
                writer.WriteElementString("SubsystemVersion", SubsystemVersion);
            writer.WriteElementString("AllowUnsafeBlocks", XmlConvert.ToString(Unsafe));
            writer.WriteElementString("WarningLevel", XmlConvert.ToString((int)WarningLevel));
            writer.WriteElementString("TreatWarningsAsErrors", XmlConvert.ToString(AllWarningsAsError));
            if (SpecificWarningsAsError != null)
                writer.WriteElementString("WarningsAsErrors",
                                          String.Join(";", SpecificWarningsAsError.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));
            if (RootNamespace != null)
                writer.WriteElementString("RootNamespace", RootNamespace);

            writer.WriteElementString("TargetFrameworkVersion", ToFrameworkVersion(TargetFrameworkVersion));
            writer.WriteElementString("TargetFrameworkProfile", ToFrameworkProfile(TargetFrameworkProfile));
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
                default:
                    throw new ArgumentOutOfRangeException("languageVersion");
            }
        }
    }
}