using System.Runtime.InteropServices;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Model
{
    /// <summary>
    /// Parameters for the C++ compiler (cl.exe, ClCompile MSBuild task) 
    /// </summary>
    public class VCppProjectCompilerParameters : IProjectParameters
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
        EnhancedInstructionSet EnableEnhancedInstructionSet { get; set; }

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
    }
}