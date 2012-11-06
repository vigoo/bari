using System;
using System.Diagnostics;
using System.IO;
using Bari.Core.Generic;
using Bari.Core.UI;

namespace Bari.Plugins.Csharp.Tools
{
    /// <summary>
    /// Default MSBuild implementation, running the MSBuild command line tool in a separate process
    /// </summary>
    public class MSBuild: IMSBuild
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (MSBuild));
        private readonly IParameters parameters;

        /// <summary>
        /// Constructs the MSBuild runner
        /// </summary>
        /// <param name="parameters">User defined parameters for bari</param>
        public MSBuild(IParameters parameters)
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
                const string path = @"c:\windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"; // TODO: this must not be hard-coded

                var absPath = Path.Combine(localRoot.AbsolutePath, relativePath);
                var psi = new ProcessStartInfo
                    {
                        FileName = path,
                        WorkingDirectory = Path.GetDirectoryName(absPath) ?? String.Empty,
                        Arguments = (Path.GetFileName(absPath) ?? String.Empty) + " /nologo /verbosity:"+Verbosity,
                        UseShellExecute = false
                    };
                
                log.DebugFormat("Executing {0} with arguments {1}", path, psi.Arguments);

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();

                    log.DebugFormat("Exit code: {0}", process.ExitCode);
                }
            }
            else
            {
                throw new NotSupportedException("Only local file system is supported for MSBuild!");
            }
        }

        private string Verbosity
        {
            get { return parameters.VerboseOutput ? "normal" : "quiet"; }
        }
    }
}