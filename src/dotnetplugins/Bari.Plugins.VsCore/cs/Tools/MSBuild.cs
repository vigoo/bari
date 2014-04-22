using System;
using System.IO;
using Bari.Core.Generic;
using Bari.Core.Tools;
using Bari.Core.UI;
using Bari.Plugins.VsCore.Exceptions;

namespace Bari.Plugins.VsCore.Tools
{
    /// <summary>
    /// Default MSBuild implementation, running the MSBuild command line tool in a separate process
    /// </summary>
    public class MSBuild: ManuallyInstallableExternalTool, IMSBuild
    {
        private readonly IParameters parameters;

        /// <summary>
        /// Constructs the MSBuild runner
        /// </summary>
        /// <param name="parameters">User defined parameters for bari</param>
        public MSBuild(IParameters parameters)
            : base("msbuild40", 
                   Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v4.0.30319\"), 
                   "MSBuild.exe", new Uri("http://www.microsoft.com/en-us/download/details.aspx?id=17851"))
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Runs MSBuild
        /// </summary>
        /// <param name="root">The root directory which will became MSBuild's root directory</param>
        /// <param name="relativePath">Relative path of the solution file (or MSBuild file) to be processed</param>
        public void Run(IFileSystemDirectory root, string relativePath)
        {
            var localRoot = root as LocalFileSystemDirectory;
            if (localRoot != null)
            {
                var absPath = Path.Combine(localRoot.AbsolutePath, relativePath);
                if (!Run(root, (Path.GetFileName(absPath) ?? String.Empty), "/nologo", "/verbosity:" + Verbosity, "/consoleloggerparameters:" + ConsoleLoggerParameters))
                    throw new MSBuildFailedException();
            }
            else
            {
                throw new NotSupportedException("Only local file system is supported for MSBuild!");
            }
        }

        private string ConsoleLoggerParameters
        {
            get { return "NoSummary"; }
        }

        private string Verbosity
        {
            get { return parameters.VerboseOutput ? "normal" : "minimal"; }
        }
    }
}