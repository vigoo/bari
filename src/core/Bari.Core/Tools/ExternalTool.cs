using System;
using System.Diagnostics;
using Bari.Core.Generic;

namespace Bari.Core.Tools
{    
    /// <summary>
    /// Base class for running and acquiring external tools 
    /// </summary>
    public abstract class ExternalTool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (ExternalTool));
        private readonly string name;

        /// <summary>
        /// Gets the tool's name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Initializes the tool runner
        /// </summary>
        /// <param name="name">Name of the external tool, for debugging and error handling purposes</param>
        protected ExternalTool(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Checks if the tool is available and download, copy, install etc. it if possible
        /// 
        /// <para>If the tool cannot be acquired then it throws an exception.</para>
        /// </summary>
        protected abstract void EnsureToolAvailable();

        /// <summary>
        /// Gets the path to the executable of the external tool
        /// </summary>
        protected abstract string ToolPath { get; }

        /// <summary>
        /// Runs the external tool with the given parameters
        /// </summary>
        /// <param name="root">Working directory</param>
        /// <param name="args">Process parameters</param>
        /// <returns>Returns <c>true</c> if the process' exit code was 0</returns>
        public bool Run(IFileSystemDirectory root, params string[] args)
        {
            EnsureToolAvailable();

            var localRoot = root as LocalFileSystemDirectory;
            if (localRoot != null)
            {
                string path = ToolPath;

                var psi = new ProcessStartInfo
                {
                    FileName = path,
                    WorkingDirectory = localRoot.AbsolutePath,
                    Arguments = String.Join(" ", args),
                    UseShellExecute = false
                };

                log.DebugFormat("Executing {0} with arguments {1}", path, psi.Arguments);

                using (var process = System.Diagnostics.Process.Start(psi))
                {
                    process.WaitForExit();
                    
                    log.DebugFormat("Exit code: {0}", process.ExitCode);
                    return process.ExitCode == 0;
                }
            }
            else
            {
                throw new NotSupportedException("Only local file system is supported for " + name + "!");
            }
        }
    }
}