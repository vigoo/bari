using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using Bari.Core.Model;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.Fsharp.Model
{
    // TODO: merge common code with CsharpProjectParameters
    public class FsharpProjectParameters: IProjectParameters
    {
        public uint? BaseAddress { get; set; }
        public string CodePage { get; set; }
        public bool DebugSymbols { get; set; }
        public DebugLevel Debug { get; set; }
        public string[] Defines { get; set; }
        public string DocOutput { get; set; }
        public int[] SuppressedWarnings { get; set; }
        public string KeyFile { get; set; }
        public bool Optimize { get; set; }
        public string OtherFlags { get; set; }
        public CLRPlatform Platform { get; set; }
        public bool Tailcalls { get; set; }
        public WarningLevel WarningLevel { get; set; }
        public bool AllWarningsAsError { get; set; }
        public int[] SpecificWarningsAsError { get; set; }
        public bool HighEntropyVirtualAddressSpace { get; set; } 

        public FsharpProjectParameters(Suite suite)
        {            
            BaseAddress = null;
            CodePage = null;            
            DocOutput = null;
            HighEntropyVirtualAddressSpace = false;
            KeyFile = null;
            SuppressedWarnings = null;

            if (suite.ActiveGoal.Has("x86"))
                Platform = CLRPlatform.x86;
            else if (suite.ActiveGoal.Has("x64"))
                Platform = CLRPlatform.x64;
            else
                Platform = CLRPlatform.AnyCPU;

            WarningLevel = WarningLevel.All;
            AllWarningsAsError = false;
            SpecificWarningsAsError = null;
            OtherFlags = null;

            if (suite.ActiveGoal.Has(Suite.DebugGoal.Name))
            {
                Debug = DebugLevel.Full;
                DebugSymbols = true;
                Optimize = false;
                Tailcalls = false;
                Defines = new[] { "DEBUG" };
            }
            else if (suite.ActiveGoal.Has(Suite.ReleaseGoal.Name))
            {
                Debug = DebugLevel.None;
                DebugSymbols = false;
                Optimize = true;
                Tailcalls = true;
                Defines = new string[0];
            }
        }
 
        public void FillProjectSpecificMissingInfo(Project project)
        {                     
        }

        public void ToFsprojProperties(XmlWriter writer)
        {
            if (BaseAddress.HasValue)
                writer.WriteElementString("BaseAddress", "0x"+BaseAddress.Value.ToString("X", CultureInfo.InvariantCulture));
            if (CodePage != null)
                writer.WriteElementString("CodePage", CodePage);
            writer.WriteElementString("DebugSymbols", XmlConvert.ToString(DebugSymbols));
            writer.WriteElementString("DebugType", Debug.ToString().ToLowerInvariant());
            if (Defines != null)
                writer.WriteElementString("DefineConstants", string.Join(";", Defines));
            if (DocOutput != null)
                writer.WriteElementString("DocumentationFile", DocOutput);
            writer.WriteElementString("HighEntropyVA", XmlConvert.ToString(HighEntropyVirtualAddressSpace));
            if (KeyFile != null)
                writer.WriteElementString("KeyOriginatorFile", KeyFile);
            if (SuppressedWarnings != null)
                writer.WriteElementString("NoWarn", 
                    String.Join(";", SuppressedWarnings.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));
            writer.WriteElementString("Optimize", XmlConvert.ToString(Optimize));
            writer.WriteElementString("Tailcalls", XmlConvert.ToString(Tailcalls));
            writer.WriteElementString("PlatformTarget", Platform.ToString().ToLowerInvariant());
            writer.WriteElementString("WarningLevel", XmlConvert.ToString((int)WarningLevel));
            writer.WriteElementString("TreatWarningsAsErrors", XmlConvert.ToString(AllWarningsAsError));
            if (SpecificWarningsAsError != null)
                writer.WriteElementString("WarningsAsErrors",
                                          String.Join(";", SpecificWarningsAsError.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));
            if (OtherFlags != null)
                writer.WriteElementString("OtherFlags", OtherFlags);

            writer.WriteStartElement("MinimumVisualStudioVersion");
            writer.WriteAttributeString("Condition", "'$(MinimumVisualStudioVersion)' == '11.0'");
            writer.WriteString("11");
            writer.WriteEndElement();
        }
    }
}