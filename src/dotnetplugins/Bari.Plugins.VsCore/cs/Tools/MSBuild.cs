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
    public abstract class MSBuild: ManuallyInstallableExternalTool, IMSBuild
    {
        private readonly IParameters parameters;

        /// <summary>
        /// Constructs the MSBuild runner
        /// </summary>
        /// <param name="parameters">User defined parameters for bari</param>
        /// <param name="path">Path to MSBuild.exe</param>
        protected MSBuild(IParameters parameters, string path)
            : base("msbuild", path, 
                   "MSBuild.exe", new Uri("http://www.microsoft.com/en-us/download/details.aspx?id=17851"), false, parameters)
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
                if (!Run(root, (Path.GetFileName(absPath) ?? String.Empty), "/m", "/nologo", "/verbosity:" + Verbosity, "/consoleloggerparameters:" + ConsoleLoggerParameters))
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