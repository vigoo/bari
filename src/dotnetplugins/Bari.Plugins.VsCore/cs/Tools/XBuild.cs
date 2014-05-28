using System;
using System.IO;
using Bari.Core.Generic;
using Bari.Core.Tools;
using Bari.Core.UI;
using Bari.Plugins.VsCore.Exceptions;

namespace Bari.Plugins.VsCore.Tools
{
    /// <summary>
    /// Mono's MSBuild implementation
    /// </summary>
    public class XBuild : ManuallyInstallableExternalTool, IMSBuild
      {
        private readonly IParameters parameters;

        /// <summary>
        /// Constructs the MSBuild runner
        /// </summary>
        /// <param name="parameters">User defined parameters for bari</param>
        public XBuild(IParameters parameters)
            : base("xbuild", 
                   "",
                   "xbuild.bat", new Uri("http://www.go-mono.com/mono-downloads/download.html"))
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