using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using System.IO;
using Bari.Core.Model.Parameters;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectCompilerParametersDef : ProjectParametersPropertyDefs<VCppProjectCompilerParameters>
    {
        public VCppProjectCompilerParametersDef()
        {
            Define<string[]>("AdditionalIncludeDirectories");
            Define<string[]>("AdditionalOptions");
            Define<string[]>("AdditionalUsingDirectories");
            Define<string>("AssemblerListingLocation");
            Define<AssemblerOutputType>("AssemblerOutput");
            Define<RuntimeCheckType>("BasicRuntimeChecks");
            Define<string>("BrowseInformationFile");
            Define<bool>("BufferSecurityCheck");
            Define<CallingConvention>("CallingConvention");
            Define<CLanguage>("CompileAs");
            Define<ManagedCppType>("CompileAsManaged");
            Define<bool>("CreateHotpatchableImage");
            Define<DebugInformationFormat>("DebugInformationFormat");
            Define<bool>("DisableLanguageExtensions");
            Define<int[]>("SuppressedWarnings");
            Define<EnhancedInstructionSet>("EnableEnhancedInstructionSet");
            Define<bool>("EnableFiberSafeOptimizations");
            Define<bool>("CodeAnalysis");
            Define<ExceptionHandlingType>("ExceptionHandling");
            Define<bool>("ExpandAttributedSource");
            Define<OptimizationFavor>("Favor");
            Define<bool>("FloatingPointExceptions");
            Define<FloatingPointModel>("FloatingPointModel");
            Define<bool>("ForceConformanceInForLoopScope");
            Define<string[]>("ForcedIncludeFiles");
            Define<string[]>("ForcedUsingFiles");
            Define<bool>("FunctionLevelLinking");
            Define<bool>("GenerateXMLDocumentationFiles");
            Define<bool>("IgnoreStandardIncludePath");
            Define<InlineExpansion>("InlineFunctionExpansion");
            Define<bool>("IntrinsicFunctions");
            Define<bool>("MinimalRebuild");
            Define<bool>("MultiProcessorCompilation");
            Define<bool>("OmitDefaultLibName");
            Define<bool>("OmitFramePointers");
            Define<bool>("OpenMPSupport");
            Define<OptimizationLevel>("Optimization");
            Define<string[]>("Defines");
            Define<int>("ProcessorNumber");
            Define<RuntimeLibraryType>("RuntimeLibrary");
            Define<bool>("RuntimeTypeInfo");
            Define<bool>("SmallerTypeCheck");
            Define<bool>("StringPooling");
            Define<int>("StructMemberAlignment");
            Define<bool>("AllWarningsAsError");
            Define<int[]>("SpecificWarningsAsError");
            Define<bool>("TreatWCharTAsBuiltInType");
            Define<bool>("UndefineAllPreprocessorDefinitions");
            Define<string[]>("UndefinePreprocessorDefinitions");
            Define<CppWarningLevel>("WarningLevel");
            Define<bool>("WholeProgramOptimization");
            Define<string>("PDBFileName");
            Define<FrameworkVersion>("TargetFrameworkVersion");
            Define<FrameworkProfile>("TargetFrameworkProfile");
        }

        public override VCppProjectCompilerParameters CreateDefault(Suite suite, VCppProjectCompilerParameters parent)
        {
            return new VCppProjectCompilerParameters(suite, parent);
        }
    }

    /// <summary>
    /// Parameters for the C++ compiler (cl.exe, ClCompile MSBuild task) 
    /// </summary>
    public class VCppProjectCompilerParameters : VCppProjectParametersBase<VCppProjectCompilerParameters, VCppProjectCompilerParametersDef>
    {
        private readonly Suite suite;

        /// <summary>
        /// Adds a directory to the list of directories that are searched for include files. (/I)
        /// </summary>
        public string[] AdditionalIncludeDirectories { get { return Get<string[]>("AdditionalIncludeDirectories"); } set { Set("AdditionalIncludeDirectories", value); } } 
        public bool AreAdditionalIncludeDirectoriesSpecified { get { return IsSpecified("AdditionalIncludeDirectories"); } }

        /// <summary>
        /// Custom options for the compiler
        /// </summary>
        public string[] AdditionalOptions { get { return Get<string[]>("AdditionalOptions"); } set { Set("AdditionalOptions", value); } } 
        public bool AreAdditionalOptionsSpecified { get { return IsSpecified("AdditionalOptions"); } }

        /// <summary>
        /// Specifies a directory that the compiler will search to resolve file references passed to the #using directive. (/AI)
        /// </summary>
        public string[] AdditionalUsingDirectories { get { return Get<string[]>("AdditionalUsingDirectories"); } set { Set("AdditionalUsingDirectories", value); } } 
        public bool AreAdditionalUsingDirectoriesSpecified { get { return IsSpecified("AdditionalUsingDirectories"); } }

        /// <summary>
        /// Creates a listing file that contains assembly code. (/Fa)
        /// </summary>
        public string AssemblerListingLocation { get { return Get<string>("AssemblerListingLocation"); } set { Set("AssemblerListingLocation", value); } } 
        public bool IsAssemblerListingLocationSpecified { get { return IsSpecified("AssemblerListingLocation"); } }

        /// <summary>
        /// Type of the assembly listing
        /// </summary>
        public AssemblerOutputType AssemblerOutput { get { return Get<AssemblerOutputType>("AssemblerOutput"); } set { Set("AssemblerOutput", value); } } 
        public bool IsAssemblerOutputSpecified { get { return IsSpecified("AssemblerOutput"); } }

        /// <summary>
        /// Basic run-time error checking (/RTC)
        /// </summary>
        public RuntimeCheckType BasicRuntimeChecks { get { return Get<RuntimeCheckType>("BasicRuntimeChecks"); } set { Set("BasicRuntimeChecks", value); } } 
        public bool IsBasicRuntimeChecksSpecified { get { return IsSpecified("BasicRuntimeChecks"); } }

        /// <summary>
        /// If <c>true</c>, creates a browse information file (/FR)
        /// 
        /// <para>If <c>null</c>, no browse information file will be created.</para>
        /// </summary>
        public string BrowseInformationFile { get { return Get<string>("BrowseInformationFile"); } set { Set("BrowseInformationFile", value); } } 
        public bool IsBrowseInformationFileSpecified { get { return IsSpecified("BrowseInformationFile"); } }

        /// <summary>
        /// If true, detects some buffer overruns that overwrite the return address, 
        /// a common technique for exploiting code that does not enforce buffer size restrictions. (/GS)
        /// </summary>
        public bool BufferSecurityCheck { get { return Get<bool>("BufferSecurityCheck"); } set { Set("BufferSecurityCheck", value); } } 
        public bool IsBufferSecurityCheckSpecified { get { return IsSpecified("BufferSecurityCheck"); } }

        /// <summary>
        /// Calling convention (/Gd, /Gr, /Gz)
        /// 
        /// <para>
        /// Supported values: Cdecl, FastCall or StdCall
        /// </para>
        /// </summary>
        public CallingConvention CallingConvention { get { return Get<CallingConvention>("CallingConvention"); } set { Set("CallingConvention", value); } } 
        public bool IsCallingConventionSpecified { get { return IsSpecified("CallingConvention"); } }

        /// <summary>
        /// Compile as C or C++ (/TC, /TP)
        /// </summary>
        public CLanguage CompileAs { get { return Get<CLanguage>("CompileAs"); } set { Set("CompileAs", value); } } 
        public bool IsCompileAsSpecified { get { return IsSpecified("CompileAs"); } }

        /// <summary>
        /// Enables or disables managed C++ (C++/CLI) compilation (/clr)
        /// </summary>
        public ManagedCppType CompileAsManaged { get { return Get<ManagedCppType>("CompileAsManaged"); } set { Set("CompileAsManaged", value); } } 
        public bool IsCompileAsManagedSpecified { get { return IsSpecified("CompileAsManaged"); } }

        /// <summary>
        /// If <c>true</c>, the compiler prepares the image for hot patching (/hotpatch)
        /// </summary>
        public bool CreateHotpatchableImage { get { return Get<bool>("CreateHotpatchableImage"); } set { Set("CreateHotpatchableImage", value); } } 
        public bool IsCreateHotpatchableImageSpecified { get { return IsSpecified("CreateHotpatchableImage"); } }

        /// <summary>
        /// Selects the debugging information format
        /// </summary>
        public DebugInformationFormat DebugInformationFormat { get { return Get<DebugInformationFormat>("DebugInformationFormat"); } set { Set("DebugInformationFormat", value); } } 
        public bool IsDebugInformationFormatSpecified { get { return IsSpecified("DebugInformationFormat"); } }

        /// <summary>
        /// Force ANSI C++ (/Za)
        /// </summary>
        public bool DisableLanguageExtensions { get { return Get<bool>("DisableLanguageExtensions"); } set { Set("DisableLanguageExtensions", value); } } 
        public bool IsDisableLanguageExtensionsSpecified { get { return IsSpecified("DisableLanguageExtensions"); } }

        /// <summary>
        /// Set of disabled warnings (/wd)
        /// </summary>
        public int[] SuppressedWarnings { get { return Get<int[]>("SuppressedWarnings"); } set { Set("SuppressedWarnings", value); } } 
        public bool AreSuppressedWarningsSpecified { get { return IsSpecified("SuppressedWarnings"); } }

        /// <summary>
        /// Enable enhanced instruction set (/arch)
        /// </summary>
        public EnhancedInstructionSet EnableEnhancedInstructionSet { get { return Get<EnhancedInstructionSet>("EnableEnhancedInstructionSet"); } set { Set("EnableEnhancedInstructionSet", value); } } 
        public bool IsEnableEnhancedInstructionSetSpecified { get { return IsSpecified("EnableEnhancedInstructionSet"); } }

        /// <summary>
        /// Fiber safety for TLS (/GT)
        /// </summary>
        public bool EnableFiberSafeOptimizations { get { return Get<bool>("EnableFiberSafeOptimizations"); } set { Set("EnableFiberSafeOptimizations", value); } } 
        public bool IsEnableFiberSafeOptimizationsSpecified { get { return IsSpecified("EnableFiberSafeOptimizations"); } }

        /// <summary>
        /// Enable code analysis (PREfast, /analyze)
        /// </summary>
        public bool CodeAnalysis { get { return Get<bool>("CodeAnalysis"); } set { Set("CodeAnalysis", value); } } 
        public bool IsCodeAnalysisSpecified { get { return IsSpecified("CodeAnalysis"); } }

        /// <summary>
        /// Exception handling model (/EH)
        /// </summary>
        public ExceptionHandlingType ExceptionHandling { get { return Get<ExceptionHandlingType>("ExceptionHandling"); } set { Set("ExceptionHandling", value); } } 
        public bool IsExceptionHandlingSpecified { get { return IsSpecified("ExceptionHandling"); } }

        /// <summary>
        /// If <c>true</c>, creates a listing with attributes injected in expanded form to the source (/Fx)
        /// </summary>
        public bool ExpandAttributedSource { get { return Get<bool>("ExpandAttributedSource"); } set { Set("ExpandAttributedSource", value); } } 
        public bool IsExpandAttributedSourceSpecified { get { return IsSpecified("ExpandAttributedSource"); } }

        /// <summary>
        /// Sets the optimization favor
        /// </summary>
        public OptimizationFavor Favor { get { return Get<OptimizationFavor>("Favor"); } set { Set("Favor", value); } } 
        public bool IsFavorSpecified { get { return IsSpecified("Favor"); } }

        /// <summary>
        /// Enable reliable floating-point exception model (/fp:except)
        /// </summary>
        public bool FloatingPointExceptions { get { return Get<bool>("FloatingPointExceptions"); } set { Set("FloatingPointExceptions", value); } } 
        public bool IsFloatingPointExceptionsSpecified { get { return IsSpecified("FloatingPointExceptions"); } }

        /// <summary>
        /// Floating point model (/fp)
        /// </summary>
        public FloatingPointModel FloatingPointModel { get { return Get<FloatingPointModel>("FloatingPointModel"); } set { Set("FloatingPointModel", value); } } 
        public bool IsFloatingPointModelSpecified { get { return IsSpecified("FloatingPointModel"); } }

        /// <summary>
        /// Standard C++ behavior in for loops using Microsoft extensions (/Zc:forScope)
        /// </summary>
        public bool ForceConformanceInForLoopScope { get { return Get<bool>("ForceConformanceInForLoopScope"); } set { Set("ForceConformanceInForLoopScope", value); } } 
        public bool IsForceConformanceInForLoopScopeSpecified { get { return IsSpecified("ForceConformanceInForLoopScope"); } }

        /// <summary>
        /// Header files to be processed by the preprocessor (/FI)
        /// </summary>
        public string[] ForcedIncludeFiles { get { return Get<string[]>("ForcedIncludeFiles"); } set { Set("ForcedIncludeFiles", value); } } 
        public bool IsForcedIncludeFilesSpecified { get { return IsSpecified("ForcedIncludeFiles"); } }

        /// <summary>
        /// #using files to be processed by the preprocessor (/FU)
        /// </summary>
        public string[] ForcedUsingFiles { get { return Get<string[]>("ForcedUsingFiles"); } set { Set("ForcedUsingFiles", value); } } 
        public bool AreForcedUsingFilesSpecified { get { return IsSpecified("ForcedUsingFiles"); } }

        /// <summary>
        /// Package individual functions in the form of packaged functions (COMDATs) (/Gy)
        /// </summary>
        public bool FunctionLevelLinking { get { return Get<bool>("FunctionLevelLinking"); } set { Set("FunctionLevelLinking", value); } } 
        public bool IsFunctionLevelLinkingSpecified { get { return IsSpecified("FunctionLevelLinking"); } }

        /// <summary>
        /// Generate XML documentation .xdc files for each source code file (/doc)
        /// </summary>
        public bool GenerateXMLDocumentationFiles { get { return Get<bool>("GenerateXMLDocumentationFiles"); } set { Set("GenerateXMLDocumentationFiles", value); } } 
        public bool IsGenerateXMLDocumentationFilesSpecified { get { return IsSpecified("GenerateXMLDocumentationFiles"); } }

        /// <summary>
        /// Ignore the standard directories when searching for include files (/X)
        /// </summary>
        public bool IgnoreStandardIncludePath { get { return Get<bool>("IgnoreStandardIncludePath"); } set { Set("IgnoreStandardIncludePath", value); } } 
        public bool IsIgnoreStandardIncludePathSpecified { get { return IsSpecified("IgnoreStandardIncludePath"); } }

        /// <summary>
        /// Sets the inline expansion level (/Ob)
        /// </summary>
        public InlineExpansion InlineFunctionExpansion { get { return Get<InlineExpansion>("InlineFunctionExpansion"); } set { Set("InlineFunctionExpansion", value); } } 
        public bool IsInlineFunctionExpansionSpecified { get { return IsSpecified("InlineFunctionExpansion"); } }

        /// <summary>
        /// If <c>true</c>, some function calls will be replaced with intrinsic forms (/Oi)
        /// </summary>
        public bool IntrinsicFunctions { get { return Get<bool>("IntrinsicFunctions"); } set { Set("IntrinsicFunctions", value); } } 
        public bool IsIntrinsicFunctionsSpecified { get { return IsSpecified("IntrinsicFunctions"); } }

        /// <summary>
        /// Enables minimal rebuild (/Gm)
        /// </summary>
        public bool MinimalRebuild { get { return Get<bool>("MinimalRebuild"); } set { Set("MinimalRebuild", value); } } 
        public bool IsMinimalRebuildSpecified { get { return IsSpecified("MinimalRebuild"); } }

        /// <summary>
        /// Use multiple processors to compile (/MP)
        /// </summary>
        public bool MultiProcessorCompilation { get { return Get<bool>("MultiProcessorCompilation"); } set { Set("MultiProcessorCompilation", value); } } 
        public bool IsMultiProcessorCompilationSpecified { get { return IsSpecified("MultiProcessorCompilation"); } }

        /// <summary>
        /// Omit the default run time library's name from the generated object files (/Zl)
        /// </summary>
        public bool OmitDefaultLibName { get { return Get<bool>("OmitDefaultLibName"); } set { Set("OmitDefaultLibName", value); } } 
        public bool IsOmitDefaultLibNameSpecified { get { return IsSpecified("OmitDefaultLibName"); } }

        /// <summary>
        /// Suppress creation of frame pointers on the call stack (/Oy)
        /// </summary>
        public bool OmitFramePointers { get { return Get<bool>("OmitFramePointers"); } set { Set("OmitFramePointers", value); } } 
        public bool IsOmitFramePointersSpecified { get { return IsSpecified("OmitFramePointers"); } }

        /// <summary>
        /// Support for OpenMP 2.0 constructs (/openmp)
        /// </summary>
        public bool OpenMPSupport { get { return Get<bool>("OpenMPSupport"); } set { Set("OpenMPSupport", value); } } 
        public bool IsOpenMPSupportSpecified { get { return IsSpecified("OpenMPSupport"); } }

        /// <summary>
        /// Optimization level (/O)
        /// </summary>
        public OptimizationLevel Optimization { get { return Get<OptimizationLevel>("Optimization"); } set { Set("Optimization", value); } } 
        public bool IsOptimizationSpecified { get { return IsSpecified("Optimization"); } }

        /// <summary>
        /// Preprocessor defines (/D)
        /// </summary>
        public string[] Defines { get { return Get<string[]>("Defines"); } set { Set("Defines", value); } } 
        public bool AreDefinesSpecified { get { return IsSpecified("Defines"); } }

        /// <summary>
        /// Maximum number of processors to use if <see cref="MultiProcessorCompilation"/> is enabled
        /// </summary>
        public int? ProcessorNumber { get { return GetAsNullable<int>("ProcessorNumber"); } set { SetAsNullable("ProcessorNumber", value); } } 
        public bool IsProcessorNumberSpecified { get { return IsSpecified("ProcessorNumber"); } }

        /// <summary>
        /// The runtime library to use (/MD, /MT, /LD)
        /// </summary>
        public RuntimeLibraryType RuntimeLibrary { get { return Get<RuntimeLibraryType>("RuntimeLibrary"); } set { Set("RuntimeLibrary", value); } } 
        public bool IsRuntimeLibrarySpecified { get { return IsSpecified("RuntimeLibrary"); } }

        /// <summary>
        /// Add code to check C++ types at runtime (RTI) (/GR)
        /// </summary>
        public bool RuntimeTypeInfo { get { return Get<bool>("RuntimeTypeInfo"); } set { Set("RuntimeTypeInfo", value); } } 
        public bool IsRuntimeTypeInfoSpecified { get { return IsSpecified("RuntimeTypeInfo"); } }

        /// <summary>
        /// Report a runtime error if value is assigned to a smaller data type and causes data loss (/RTCc)
        /// </summary>
        public bool SmallerTypeCheck { get { return Get<bool>("SmallerTypeCheck"); } set { Set("SmallerTypeCheck", value); } } 
        public bool IsSmallerTypeCheckSpecified { get { return IsSpecified("SmallerTypeCheck"); } }

        /// <summary>
        /// Enables string pooling (/GF)
        /// </summary>
        public bool StringPooling { get { return Get<bool>("StringPooling"); } set { Set("StringPooling", value); } } 
        public bool IsStringPoolingSpecified { get { return IsSpecified("StringPooling"); } }

        /// <summary>
        /// Byte alignment for members in a structure
        /// 
        /// <para><c>null</c> means default. Other possible values: 1, 2, 4, 8, 16
        /// </para>
        /// </summary>
        public int? StructMemberAlignment { get { return GetAsNullable<int>("StructMemberAlignment"); } set { SetAsNullable("StructMemberAlignment", value); } } 
        public bool IsStructMemberAlignmentSpecified { get { return IsSpecified("StructMemberAlignment"); } }

        public bool AllWarningsAsError { get { return Get<bool>("AllWarningsAsError"); } set { Set("AllWarningsAsError", value); } } 
        public bool IsAllWarningsAsErrorSpecified { get { return IsSpecified("AllWarningsAsError"); } }
        public int[] SpecificWarningsAsError { get { return Get<int[]>("SpecificWarningsAsError"); } set { Set("SpecificWarningsAsError", value); } } 
        public bool AreSpecificWarningsAsErrorSpecified { get { return IsSpecified("SpecificWarningsAsError"); } }

        /// <summary>
        /// Treat <c>wchar_t</c> as a native type (/Zc:wchar_t)
        /// </summary>
        public bool TreatWCharTAsBuiltInType { get { return Get<bool>("TreatWCharTAsBuiltInType"); } set { Set("TreatWCharTAsBuiltInType", value); } } 
        public bool IsTreatWCharTAsBuiltInTypeSpecified { get { return IsSpecified("TreatWCharTAsBuiltInType"); } }

        /// <summary>
        /// Undefine Microsoft-specific symbols defined by the compiler (/u)
        /// </summary>
        public bool UndefineAllPreprocessorDefinitions { get { return Get<bool>("UndefineAllPreprocessorDefinitions"); } set { Set("UndefineAllPreprocessorDefinitions", value); } } 
        public bool IsUndefineAllPreprocessorDefinitionsSpecified { get { return IsSpecified("UndefineAllPreprocessorDefinitions"); } }

        /// <summary>
        /// A specific set of defines to be undefined (/U)
        /// </summary>
        public string[] UndefinePreprocessorDefinitions { get { return Get<string[]>("UndefinePreprocessorDefinitions"); } set { Set("UndefinePreprocessorDefinitions", value); } } 
        public bool AreUndefinePreprocessorDefinitionsSpecified { get { return IsSpecified("UndefinePreprocessorDefinitions"); } }

        /// <summary>
        /// Warning level (/Wn)
        /// </summary>
        public CppWarningLevel WarningLevel { get { return Get<CppWarningLevel>("WarningLevel"); } set { Set("WarningLevel", value); } } 
        public bool IsWarningLevelSpecified { get { return IsSpecified("WarningLevel"); } }

        /// <summary>
        /// Enables whole program optimization (/GL)
        /// </summary>
        public bool WholeProgramOptimization { get { return Get<bool>("WholeProgramOptimization"); } set { Set("WholeProgramOptimization", value); } } 
        public bool IsWholeProgramOptimizationSpecified { get { return IsSpecified("WholeProgramOptimization"); } }

        /// <summary>
        /// Name of the PDB file to be generated
        /// </summary>
        public string PDBFileName { get { return Get<string>("PDBFileName"); } set { Set("PDBFileName", value); } } 
        public bool IsPDBFileNameSpecified { get { return IsSpecified("PDBFileName"); } }

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

        public VCppProjectCompilerParameters(Suite suite, VCppProjectCompilerParameters parent = null)
            : base(parent)
        {
            this.suite = suite;
        }

        public void FillProjectSpecificMissingInfo(Project project, CppCliMode cliMode, LocalFileSystemDirectory targetDir)
        {
            if (targetDir != null)
            {
                PDBFileName = string.Format("{0}{3}{1}.{2}.pdb",
                                            targetDir.AbsolutePath,
                                            project.Module.Name, project.Name,
                                            Path.DirectorySeparatorChar);
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

                if (!IsDebugInformationFormatSpecified && suite.ActiveGoal.Has(Suite.DebugGoal.Name))
                    DebugInformationFormat = DebugInformationFormat.ProgramDatabase;

                MinimalRebuild = false;
                SmallerTypeCheck = false;
                FloatingPointExceptions = false;

                if (project.Type == ProjectType.StaticLibrary)
                {
                    Defines = Defines.Concat(new[] { "_LIB" }).ToArray();
                }
            }

            if (project.EffectiveVersion != null)
            {
                Defines = Defines.Concat(new[]
                {
                    String.Format("BARI_PROJECT_VERSION=\"{0}\"", project.EffectiveVersion)
                }).ToArray();
            }

            if (project.EffectiveCopyright != null)
            {
                Defines = Defines.Concat(new[]
                {
                    String.Format("BARI_PROJECT_COPYRIGHT=\"{0}\"", project.EffectiveCopyright)
                }).ToArray();
            }
        }

        public void ToVcxprojProperties(XmlWriter writer)
        {
            if (AreAdditionalIncludeDirectoriesSpecified && AdditionalIncludeDirectories.Length > 0)
                writer.WriteElementString("AdditionalIncludeDirectories", string.Join(";", AdditionalIncludeDirectories));
            if (AreAdditionalOptionsSpecified && AdditionalOptions.Length > 0)
                writer.WriteElementString("AdditionalOptions", string.Join(";", AdditionalOptions));
            if (AreAdditionalUsingDirectoriesSpecified && AdditionalUsingDirectories.Length > 0)
                writer.WriteElementString("AdditionalUsingDirectories", string.Join(";", AdditionalUsingDirectories));
            if (IsAssemblerListingLocationSpecified)
                writer.WriteElementString("AssemblerListingLocation", AssemblerListingLocation);
            writer.WriteElementString("AssemblerOutput", (IsAssemblerOutputSpecified ? AssemblerOutput : AssemblerOutputType.NoListing).ToString());
            if (IsBasicRuntimeChecksSpecified && BasicRuntimeChecks != RuntimeCheckType.Default)
                writer.WriteElementString("BasicRuntimeChecks", BasicRuntimeChecks.ToString());
            if (IsBrowseInformationFileSpecified)
                writer.WriteElementString("BrowseInformationFile", BrowseInformationFile);
            writer.WriteElementString("BufferSecurityCheck", XmlConvert.ToString(!IsBufferSecurityCheckSpecified || BufferSecurityCheck));
            writer.WriteElementString("CallingConvention", (IsCallingConventionSpecified ? CallingConvention : CallingConvention.Cdecl).ToString());
            if (IsCompileAsSpecified && CompileAs != CLanguage.Default)
                writer.WriteElementString("CompileAs", CompileAs.ToString());
            writer.WriteElementString("CompileAsManaged", CompileAsManagedToString(IsCompileAsManagedSpecified ? CompileAsManaged : ManagedCppType.NotManaged));
            writer.WriteElementString("CreateHotpatchableImage", XmlConvert.ToString(IsCreateHotpatchableImageSpecified && CreateHotpatchableImage));
            if (IsDebugInformationFormatSpecified && DebugInformationFormat != DebugInformationFormat.None)
                writer.WriteElementString("DebugInformationFormat", DebugInformationFormat.ToString());
            else
                writer.WriteElementString("DebugInformationFormat", (suite.ActiveGoal.Has(Suite.DebugGoal.Name)
                    ? DebugInformationFormat.EditAndContinue
                    : DebugInformationFormat.None).ToString());
            writer.WriteElementString("DisableLanguageExtensions", XmlConvert.ToString(IsDisableLanguageExtensionsSpecified && DisableLanguageExtensions));
            WriteStringArray(writer, "DisableSpecificWarnings", AreSuppressedWarningsSpecified ? SuppressedWarnings.Select(warn => warn.ToString(CultureInfo.InvariantCulture)).ToArray() : new string[0]);
            if (IsEnableEnhancedInstructionSetSpecified && EnableEnhancedInstructionSet != EnhancedInstructionSet.None)
                writer.WriteElementString("EnhancedInstructionSet", EnableEnhancedInstructionSet.ToString());
            else
                writer.WriteElementString("EnhancedInstructionSet", (EnhancedInstructionSet.SSE2).ToString());
            writer.WriteElementString("EnableFiberSafeOptimizations", XmlConvert.ToString(IsEnableFiberSafeOptimizationsSpecified && EnableFiberSafeOptimizations));
            writer.WriteElementString("CodeAnalysis", XmlConvert.ToString(!IsCodeAnalysisSpecified || CodeAnalysis));
            if (IsExceptionHandlingSpecified && ExceptionHandling != ExceptionHandlingType.NotSpecified)
                writer.WriteElementString("ExceptionHandling", ExceptionHandling.ToString());
            writer.WriteElementString("ExpandAttributedSource", XmlConvert.ToString(IsExpandAttributedSourceSpecified && ExpandAttributedSource));
            writer.WriteElementString("FavorSizeOrSpeed", (IsFavorSpecified ? Favor : OptimizationFavor.Speed).ToString());
            writer.WriteElementString("FloatingPointExceptions", XmlConvert.ToString(!IsFloatingPointExceptionsSpecified || FloatingPointExceptions));
            writer.WriteElementString("FloatingPointModel", (IsFloatingPointModelSpecified ? FloatingPointModel : FloatingPointModel.Precise).ToString());
            writer.WriteElementString("ForceConformanceInForLoopScope", XmlConvert.ToString(IsForceConformanceInForLoopScopeSpecified && ForceConformanceInForLoopScope));
            WriteStringArray(writer, "ForcedUsingFiles", AreForcedUsingFilesSpecified ? ForcedUsingFiles : new string[0]);
            writer.WriteElementString("FunctionLevelLinking", XmlConvert.ToString(IsFunctionLevelLinkingSpecified && FunctionLevelLinking));
            writer.WriteElementString("GenerateXMLDocumentationFiles", XmlConvert.ToString(IsGenerateXMLDocumentationFilesSpecified && GenerateXMLDocumentationFiles));
            writer.WriteElementString("IgnoreStandardIncludePath", XmlConvert.ToString(IsIgnoreStandardIncludePathSpecified && IgnoreStandardIncludePath));
            if (IsInlineFunctionExpansionSpecified && InlineFunctionExpansion != InlineExpansion.Default)
                writer.WriteElementString("InlineFunctionExpansion", InlineFunctionExpansion.ToString());
            writer.WriteElementString("IntrinsicFunctions", XmlConvert.ToString(IsIntrinsicFunctionsSpecified && IntrinsicFunctions));
            writer.WriteElementString("MinimalRebuild", XmlConvert.ToString(!IsMinimalRebuildSpecified || MinimalRebuild));
            writer.WriteElementString("MultiProcessorCompilation", XmlConvert.ToString(!IsMultiProcessorCompilationSpecified || MultiProcessorCompilation));
            writer.WriteElementString("OmitDefaultLibName", XmlConvert.ToString(IsOmitDefaultLibNameSpecified && OmitDefaultLibName));
            writer.WriteElementString("OmitFramePointers", XmlConvert.ToString(IsOmitFramePointersSpecified && OmitFramePointers));
            writer.WriteElementString("OpenMPSupport", XmlConvert.ToString(IsOpenMPSupportSpecified && OpenMPSupport));
            writer.WriteElementString("Optimization",
                (IsOptimizationSpecified
                    ? Optimization
                    : suite.ActiveGoal.Has(Suite.DebugGoal.Name)
                        ? OptimizationLevel.Disabled
                        : OptimizationLevel.Full).ToString());

            string[] defines;
            if (AreDefinesSpecified)
                defines = Defines;
            else
                defines = suite.ActiveGoal.Has(Suite.DebugGoal.Name)
                          ? new[] { "_DEBUG" }
                          : new[] { "NDEBUG" };
            WriteStringArray(writer, "PreprocessorDefinitions", defines);

            if (ProcessorNumber.HasValue)
                writer.WriteElementString("ProcessorNumber", ProcessorNumber.Value.ToString(CultureInfo.InvariantCulture));

            writer.WriteElementString("RuntimeLibrary",
                (IsRuntimeLibrarySpecified
                    ? RuntimeLibrary
                    : suite.ActiveGoal.Has(Suite.DebugGoal.Name)
                        ? RuntimeLibraryType.MultiThreadedDebugDLL
                        : RuntimeLibraryType.MultiThreadedDLL).ToString());
            writer.WriteElementString("RuntimeTypeInfo", XmlConvert.ToString(!IsRuntimeTypeInfoSpecified || RuntimeTypeInfo));
            writer.WriteElementString("SmallerTypeCheck", XmlConvert.ToString(IsSmallerTypeCheckSpecified ? SmallerTypeCheck : suite.ActiveGoal.Has(Suite.DebugGoal.Name)));
            writer.WriteElementString("StringPooling", XmlConvert.ToString(!IsStringPoolingSpecified || StringPooling));
            if (StructMemberAlignment.HasValue)
                writer.WriteElementString("StructMemberAlignment", StructMemberAlignment.Value.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("TreatWarningAsError", XmlConvert.ToString(IsAllWarningsAsErrorSpecified && AllWarningsAsError));
            WriteStringArray(writer, "SpecificWarningsAsError", AreSpecificWarningsAsErrorSpecified ? SpecificWarningsAsError.Select(warn => warn.ToString(CultureInfo.InvariantCulture)).ToArray() : new string[0]);
            writer.WriteElementString("TreatWCharTAsBuiltInType", XmlConvert.ToString(!IsTreatWCharTAsBuiltInTypeSpecified || TreatWCharTAsBuiltInType));
            writer.WriteElementString("UndefineAllPreprocessorDefinitions", XmlConvert.ToString(IsUndefineAllPreprocessorDefinitionsSpecified && UndefineAllPreprocessorDefinitions));
            WriteStringArray(writer, "UndefinePreprocessorDefinitions", AreUndefinePreprocessorDefinitionsSpecified ? UndefinePreprocessorDefinitions : new string[0]);
            writer.WriteElementString("WarningLevel", WarningLevelToString(IsWarningLevelSpecified ? WarningLevel : CppWarningLevel.All));
            writer.WriteElementString("WholeProgramOptimization", XmlConvert.ToString(IsWholeProgramOptimizationSpecified ? WholeProgramOptimization : suite.ActiveGoal.Has(Suite.ReleaseGoal.Name)));
            writer.WriteElementString("ProgramDataBaseFileName", PDBFileName);
        }

        public void WriteGlobalProperties(XmlWriter writer)
        {
            var targetFrameworkVersion = IsTargetFrameworkVersionSpecified
                ? TargetFrameworkVersion
                : FrameworkVersion.v4;
            writer.WriteElementString("TargetFrameworkVersion", ToFrameworkVersion(targetFrameworkVersion));

            var targetFrameworkProfile = IsTargetFrameworkProfileSpecified
                ? TargetFrameworkProfile
                : FrameworkProfile.Default;
            writer.WriteElementString("TargetFrameworkProfile", ToFrameworkProfile(targetFrameworkProfile));
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
    }
}