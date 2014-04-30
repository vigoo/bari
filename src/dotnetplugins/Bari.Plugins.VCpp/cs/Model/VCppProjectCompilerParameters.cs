using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Model
{
    /// <summary>
    /// Parameters for the C++ compiler (cl.exe, ClCompile MSBuild task) 
    /// </summary>
    public class VCppProjectCompilerParameters : VCppProjectParametersBase
    {
        /// <summary>
        /// Adds a directory to the list of directories that are searched for include files. (/I)
        /// </summary>
        public string[] AdditionalIncludeDirectories { get; set; }

        /// <summary>
        /// Custom options for the compiler
        /// </summary>
        public string[] AdditionalOptions { get; set; }

        /// <summary>
        /// Specifies a directory that the compiler will search to resolve file references passed to the #using directive. (/AI)
        /// </summary>
        public string[] AdditionalUsingDirectories { get; set; }

        /// <summary>
        /// Creates a listing file that contains assembly code. (/Fa)
        /// </summary>
        public string AssemblerListingLocation { get; set; }

        /// <summary>
        /// Type of the assembly listing
        /// </summary>
        public AssemblerOutputType AssemblerOutput { get; set; }

        /// <summary>
        /// Basic run-time error checking (/RTC)
        /// </summary>
        public RuntimeCheckType BasicRuntimeChecks { get; set; }

        /// <summary>
        /// If <c>true</c>, creates a browse information file (/FR)
        /// 
        /// <para>If <c>null</c>, no browse information file will be created.</para>
        /// </summary>
        public string BrowseInformationFile { get; set; }

        /// <summary>
        /// If true, detects some buffer overruns that overwrite the return address, 
        /// a common technique for exploiting code that does not enforce buffer size restrictions. (/GS)
        /// </summary>
        public bool BufferSecurityCheck { get; set; }

        /// <summary>
        /// Calling convention (/Gd, /Gr, /Gz)
        /// 
        /// <para>
        /// Supported values: Cdecl, FastCall or StdCall
        /// </para>
        /// </summary>
        public CallingConvention CallingConvention { get; set; }

        /// <summary>
        /// Compile as C or C++ (/TC, /TP)
        /// </summary>
        public CLanguage CompileAs { get; set; }

        /// <summary>
        /// Enables or disables managed C++ (C++/CLI) compilation (/clr)
        /// </summary>
        public ManagedCppType CompileAsManaged { get; set; }

        /// <summary>
        /// If <c>true</c>, the compiler prepares the image for hot patching (/hotpatch)
        /// </summary>
        public bool CreateHotpatchableImage { get; set; }

        /// <summary>
        /// Selects the debugging information format
        /// </summary>
        public DebugInformationFormat DebugInformationFormat { get; set; }

        /// <summary>
        /// Force ANSI C++ (/Za)
        /// </summary>
        public bool DisableLanguageExtensions { get; set; }

        /// <summary>
        /// Set of disabled warnings (/wd)
        /// </summary>
        public int[] SuppressedWarnings { get; set; }

        /// <summary>
        /// Enable enhanced instruction set (/arch)
        /// </summary>
        public EnhancedInstructionSet EnableEnhancedInstructionSet { get; set; }

        /// <summary>
        /// Fiber safety for TLS (/GT)
        /// </summary>
        public bool EnableFiberSafeOptimizations { get; set; }

        /// <summary>
        /// Enable code analysis (PREfast, /analyze)
        /// </summary>
        public bool CodeAnalysis { get; set; }

        /// <summary>
        /// Exception handling model (/EH)
        /// </summary>
        public ExceptionHandlingType ExceptionHandling { get; set; }

        /// <summary>
        /// If <c>true</c>, creates a listing with attributes injected in expanded form to the source (/Fx)
        /// </summary>
        public bool ExpandAttributedSource { get; set; }

        /// <summary>
        /// Sets the optimization favor
        /// </summary>
        public OptimizationFavor Favor { get; set; }

        /// <summary>
        /// Enable reliable floating-point exception model (/fp:except)
        /// </summary>
        public bool FloatingPointExceptions { get; set; }

        /// <summary>
        /// Floating point model (/fp)
        /// </summary>
        public FloatingPointModel FloatingPointModel { get; set; }

        /// <summary>
        /// Standard C++ behavior in for loops using Microsoft extensions (/Zc:forScope)
        /// </summary>
        public bool ForceConformanceInForLoopScope { get; set; }

        /// <summary>
        /// Header files to be processed by the preprocessor (/FI)
        /// </summary>
        public string[] ForcedIncludeFiles { get; set; }

        /// <summary>
        /// #using files to be processed by the preprocessor (/FU)
        /// </summary>
        public string[] ForcedUsingFiles { get; set; }

        /// <summary>
        /// Package individual functions in the form of packaged functions (COMDATs) (/Gy)
        /// </summary>
        public bool FunctionLevelLinking { get; set; }

        /// <summary>
        /// Generate XML documentation .xdc files for each source code file (/doc)
        /// </summary>
        public bool GenerateXMLDocumentationFiles { get; set; }

        /// <summary>
        /// Ignore the standard directories when searching for include files (/X)
        /// </summary>
        public bool IgnoreStandardIncludePath { get; set; }

        /// <summary>
        /// Sets the inline expansion level (/Ob)
        /// </summary>
        public InlineExpansion InlineFunctionExpansion { get; set; }

        /// <summary>
        /// If <c>true</c>, some function calls will be replaced with intrinsic forms (/Oi)
        /// </summary>
        public bool IntrinsicFunctions { get; set; }

        /// <summary>
        /// Enables minimal rebuild (/Gm)
        /// </summary>
        public bool MinimalRebuild { get; set; }

        /// <summary>
        /// Use multiple processors to compile (/MP)
        /// </summary>
        public bool MultiProcessorCompilation { get; set; }

        /// <summary>
        /// Omit the default run time library's name from the generated object files (/Zl)
        /// </summary>
        public bool OmitDefaultLibName { get; set; }

        /// <summary>
        /// Suppress creation of frame pointers on the call stack (/Oy)
        /// </summary>
        public bool OmitFramePointers { get; set; }

        /// <summary>
        /// Support for OpenMP 2.0 constructs (/openmp)
        /// </summary>
        public bool OpenMPSupport { get; set; }

        /// <summary>
        /// Optimization level (/O)
        /// </summary>
        public OptimizationLevel Optimization { get; set; }

        /// <summary>
        /// Preprocessor defines (/D)
        /// </summary>
        public string[] Defines { get; set; }

        /// <summary>
        /// Maximum number of processors to use if <see cref="MultiProcessorCompilation"/> is enabled
        /// </summary>
        public int? ProcessorNumber { get; set; }

        /// <summary>
        /// The runtime library to use (/MD, /MT, /LD)
        /// </summary>
        public RuntimeLibraryType RuntimeLibrary { get; set; }

        /// <summary>
        /// Add code to check C++ types at runtime (RTI) (/GR)
        /// </summary>
        public bool RuntimeTypeInfo { get; set; }

        /// <summary>
        /// Report a runtime error if value is assigned to a smaller data type and causes data loss (/RTCc)
        /// </summary>
        public bool SmallerTypeCheck { get; set; }

        /// <summary>
        /// Enables string pooling (/GF)
        /// </summary>
        public bool StringPooling { get; set; }

        /// <summary>
        /// Byte alignment for members in a structure
        /// 
        /// <para><c>null</c> means default. Other possible values: 1, 2, 4, 8, 16
        /// </para>
        /// </summary>
        public int? StructMemberAlignment { get; set; }

        public bool AllWarningsAsError { get; set; }
        public int[] SpecificWarningsAsError { get; set; }

        /// <summary>
        /// Treat <c>wchar_t</c> as a native type (/Zc:wchar_t)
        /// </summary>
        public bool TreatWCharTAsBuiltInType { get; set; }

        /// <summary>
        /// Undefine Microsoft-specific symbols defined by the compiler (/u)
        /// </summary>
        public bool UndefineAllPreprocessorDefinitions { get; set; }

        /// <summary>
        /// A specific set of defines to be undefined (/U)
        /// </summary>
        public string[] UndefinePreprocessorDefinitions { get; set; }

        /// <summary>
        /// Warning level (/Wn)
        /// </summary>
        public CppWarningLevel WarningLevel { get; set; }

        /// <summary>
        /// Enables whole program optimization (/GL)
        /// </summary>
        public bool WholeProgramOptimization { get; set; }

        /// <summary>
        /// Name of the PDB file to be generated
        /// </summary>
        public string PDBFileName { get; set; }

        public VCppProjectCompilerParameters(Suite suite)
        {
            AdditionalIncludeDirectories = new string[0];
            AdditionalOptions = new string[0];
            AdditionalUsingDirectories = new string[0];
            AssemblerListingLocation = null;
            AssemblerOutput = AssemblerOutputType.NoListing;
            BasicRuntimeChecks = RuntimeCheckType.Default;
            BrowseInformationFile = null;
            BufferSecurityCheck = true;
            CallingConvention = CallingConvention.Cdecl;
            CompileAs = CLanguage.Default;
            CompileAsManaged = ManagedCppType.NotManaged;
            CreateHotpatchableImage = false;

            DebugInformationFormat = suite.ActiveGoal.Has(Suite.DebugGoal.Name)
                                         ? DebugInformationFormat.EditAndContinue
                                         : DebugInformationFormat.None;

            DisableLanguageExtensions = false;
            SuppressedWarnings = new int[0];
            EnableEnhancedInstructionSet = EnhancedInstructionSet.SSE2;
            EnableFiberSafeOptimizations = false;
            CodeAnalysis = true;
            ExceptionHandling = ExceptionHandlingType.NotSpecified;
            ExpandAttributedSource = false;
            Favor = OptimizationFavor.Speed;
            FloatingPointExceptions = true;
            FloatingPointModel = FloatingPointModel.Precise;
            ForceConformanceInForLoopScope = false;
            ForcedIncludeFiles = new string[0];
            ForcedUsingFiles = new string[0];
            FunctionLevelLinking = false;
            GenerateXMLDocumentationFiles = false;
            IgnoreStandardIncludePath = false;
            InlineFunctionExpansion = InlineExpansion.Default;
            IntrinsicFunctions = false;
            MinimalRebuild = true;
            MultiProcessorCompilation = true;
            OmitDefaultLibName = false;
            OmitFramePointers = false;
            OpenMPSupport = false;

            Optimization = suite.ActiveGoal.Has(Suite.DebugGoal.Name)
                               ? OptimizationLevel.Disabled
                               : OptimizationLevel.Full;

            Defines = suite.ActiveGoal.Has(Suite.DebugGoal.Name)
                          ? new[] { "_DEBUG" }
                          : new[] { "NDEBUG" };

            ProcessorNumber = null;

            RuntimeLibrary = suite.ActiveGoal.Has(Suite.DebugGoal.Name)
                                 ? RuntimeLibraryType.MultiThreadedDebugDLL
                                 : RuntimeLibraryType.MultiThreadedDLL;

            RuntimeTypeInfo = true;
            SmallerTypeCheck = true;
            StringPooling = true;
            StructMemberAlignment = null;
            AllWarningsAsError = false;
            SpecificWarningsAsError = new int[0];
            TreatWCharTAsBuiltInType = true;
            UndefineAllPreprocessorDefinitions = false;
            UndefinePreprocessorDefinitions = new string[0];
            WarningLevel = CppWarningLevel.All;
            WholeProgramOptimization = suite.ActiveGoal.Has(Suite.ReleaseGoal.Name);
        }

        public void FillProjectSpecificMissingInfo(Project project, CppCliMode cliMode, LocalFileSystemDirectory targetDir)
        {
            if (targetDir != null)
            {
                PDBFileName = string.Format("{0}\\{1}.{2}.pdb",
                                            targetDir.AbsolutePath,
                                            project.Module.Name, project.Name);
            }

            if (cliMode != CppCliMode.Disabled)
            {
                // Fixing some settings to support C++/CLI mode
                switch (cliMode)
                {
                    case CppCliMode.Enabled:
                        CompileAsManaged = ManagedCppType.Managed;
                        break;
                    case CppCliMode.Pure:
                        CompileAsManaged = ManagedCppType.Pure;
                        break;
                    case CppCliMode.Safe:
                        CompileAsManaged = ManagedCppType.Safe;
                        break;
                    case CppCliMode.OldSyntax:
                        CompileAsManaged = ManagedCppType.OldSyntax;
                        break;
                }

                if (DebugInformationFormat == DebugInformationFormat.EditAndContinue)
                    DebugInformationFormat = DebugInformationFormat.ProgramDatabase;

                MinimalRebuild = false;
                SmallerTypeCheck = false;
                FloatingPointExceptions = false;

                if (project.Type == ProjectType.StaticLibrary)
                {
                    Defines = Defines.Concat(new[] {"_LIB"}).ToArray();
                }
            }
        }

        public void ToVcxprojProperties(XmlWriter writer)
        {
            if (AdditionalIncludeDirectories != null && AdditionalIncludeDirectories.Length > 0)
                writer.WriteElementString("AdditionalIncludeDirectories", string.Join(";", AdditionalIncludeDirectories));
            if (AdditionalOptions != null && AdditionalOptions.Length > 0)
                writer.WriteElementString("AdditionalOptions", string.Join(";", AdditionalOptions));
            if (AdditionalUsingDirectories != null && AdditionalUsingDirectories.Length > 0)
                writer.WriteElementString("AdditionalUsingDirectories", string.Join(";", AdditionalUsingDirectories));
            if (!string.IsNullOrWhiteSpace(AssemblerListingLocation))
                writer.WriteElementString("AssemblerListingLocation", AssemblerListingLocation);
            writer.WriteElementString("AssemblerOutput", AssemblerOutput.ToString());
            if (BasicRuntimeChecks != RuntimeCheckType.Default)
                writer.WriteElementString("BasicRuntimeChecks", BasicRuntimeChecks.ToString());
            if (!string.IsNullOrWhiteSpace("BrowseInformationFile"))
                writer.WriteElementString("BrowseInformationFile", BrowseInformationFile);
            writer.WriteElementString("BufferSecurityCheck", XmlConvert.ToString(BufferSecurityCheck));
            writer.WriteElementString("CallingConvention", CallingConvention.ToString());
            if (CompileAs != CLanguage.Default)
                writer.WriteElementString("CompileAs", CompileAs.ToString());
            writer.WriteElementString("CompileAsManaged", CompileAsManagedToString(CompileAsManaged));
            writer.WriteElementString("CreateHotpatchableImage", XmlConvert.ToString(CreateHotpatchableImage));
            if (DebugInformationFormat != DebugInformationFormat.None)
                writer.WriteElementString("DebugInformationFormat", DebugInformationFormat.ToString());
            writer.WriteElementString("DisableLanguageExtensions", XmlConvert.ToString(DisableLanguageExtensions));
            WriteStringArray(writer, "DisableSpecificWarnings", SuppressedWarnings.Select(warn => warn.ToString(CultureInfo.InvariantCulture)).ToArray());
            if (EnableEnhancedInstructionSet != EnhancedInstructionSet.None)
                writer.WriteElementString("EnhancedInstructionSet", EnableEnhancedInstructionSet.ToString());
            writer.WriteElementString("EnableFiberSafeOptimizations", XmlConvert.ToString(EnableFiberSafeOptimizations));
            writer.WriteElementString("CodeAnalysis", XmlConvert.ToString(CodeAnalysis));
            if (ExceptionHandling != ExceptionHandlingType.NotSpecified)
                writer.WriteElementString("ExceptionHandling", ExceptionHandling.ToString());
            writer.WriteElementString("ExpandAttributedSource", XmlConvert.ToString(ExpandAttributedSource));
            writer.WriteElementString("FavorSizeOrSpeed", Favor.ToString());
            writer.WriteElementString("FloatingPointExceptions", XmlConvert.ToString(FloatingPointExceptions));
            writer.WriteElementString("FloatingPointModel", FloatingPointModel.ToString());
            writer.WriteElementString("ForceConformanceInForLoopScope", XmlConvert.ToString(ForceConformanceInForLoopScope));
            WriteStringArray(writer, "ForcedUsingFiles", ForcedUsingFiles);
            writer.WriteElementString("FunctionLevelLinking", XmlConvert.ToString(FunctionLevelLinking));
            writer.WriteElementString("GenerateXMLDocumentationFiles", XmlConvert.ToString(GenerateXMLDocumentationFiles));
            writer.WriteElementString("IgnoreStandardIncludePath", XmlConvert.ToString(IgnoreStandardIncludePath));
            if (InlineFunctionExpansion != InlineExpansion.Default)
                writer.WriteElementString("InlineFunctionExpansion", InlineFunctionExpansion.ToString());
            writer.WriteElementString("IntrinsicFunctions", XmlConvert.ToString(IntrinsicFunctions));
            writer.WriteElementString("MinimalRebuild", XmlConvert.ToString(MinimalRebuild));
            writer.WriteElementString("MultiProcessorCompilation", XmlConvert.ToString(MultiProcessorCompilation));
            writer.WriteElementString("OmitDefaultLibName", XmlConvert.ToString(OmitDefaultLibName));
            writer.WriteElementString("OmitFramePointers", XmlConvert.ToString(OmitFramePointers));
            writer.WriteElementString("OpenMPSupport", XmlConvert.ToString(OpenMPSupport));
            writer.WriteElementString("Optimization", Optimization.ToString());
            WriteStringArray(writer, "PreprocessorDefinitions", Defines);
            if (ProcessorNumber.HasValue)
                writer.WriteElementString("ProcessorNumber", ProcessorNumber.Value.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("RuntimeLibrary", RuntimeLibrary.ToString());
            writer.WriteElementString("RuntimeTypeInfo", XmlConvert.ToString(RuntimeTypeInfo));
            writer.WriteElementString("SmallerTypeCheck", XmlConvert.ToString(SmallerTypeCheck));
            writer.WriteElementString("StringPooling", XmlConvert.ToString(StringPooling));
            if (StructMemberAlignment.HasValue)
                writer.WriteElementString("StructMemberAlignment", StructMemberAlignment.Value.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("AllWarningsAsError", XmlConvert.ToString(AllWarningsAsError));
            WriteStringArray(writer, "SpecificWarningsAsError", SpecificWarningsAsError.Select(warn => warn.ToString(CultureInfo.InvariantCulture)).ToArray());
            writer.WriteElementString("TreatWCharTAsBuiltInType", XmlConvert.ToString(TreatWCharTAsBuiltInType));
            writer.WriteElementString("UndefineAllPreprocessorDefinitions", XmlConvert.ToString(UndefineAllPreprocessorDefinitions));
            WriteStringArray(writer, "UndefinePreprocessorDefinitions", UndefinePreprocessorDefinitions);
            writer.WriteElementString("WarningLevel", WarningLevelToString(WarningLevel));
            writer.WriteElementString("WholeProgramOptimization", XmlConvert.ToString(WholeProgramOptimization));
            writer.WriteElementString("ProgramDataBaseFileName", PDBFileName);
        }

        private string WarningLevelToString(CppWarningLevel warningLevel)
        {
            switch (warningLevel)
            {
                case CppWarningLevel.Off:
                    return "TurnOffAllWarnings";
                case CppWarningLevel.Level1:
                    return "Level1";
                case CppWarningLevel.Level2:
                    return "Level2";
                case CppWarningLevel.Level3:
                    return "Level3";
                case CppWarningLevel.Level4:
                    return "Level4";
                case CppWarningLevel.All:
                    return "EnableAllWarnings";
                default:
                    throw new ArgumentOutOfRangeException("warningLevel");
            }
        }

        private string CompileAsManagedToString(ManagedCppType managedCppType)
        {
            switch (managedCppType)
            {
                case ManagedCppType.NotManaged:
                    return "false";
                case ManagedCppType.Managed:
                    return "true";
                case ManagedCppType.Pure:
                    return "Pure";
                case ManagedCppType.Safe:
                    return "Safe";
                case ManagedCppType.OldSyntax:
                    return "OldSyntax";
                default:
                    throw new ArgumentOutOfRangeException("managedCppType");
            }
        }
    }
}