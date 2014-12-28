using System;
using System.IO;
using System.Net;
using Bari.Core.UI;

namespace Bari.Core.Tools
{
    /// <summary>
    /// A more generally usable version of <see cref="DownloadableExternalTool"/> where the downloaded file
    /// is not the external tool executable itself but an installer/self extracting package
    /// </summary>
    public abstract class DownloadableSelfExtractingExternalTool : DownloadableExternalTool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DownloadableSelfExtractingExternalTool));

        /// <summary>
        /// Defines a tool which can be downloaded if missing from an URL
        /// </summary>
        /// <param name="name">Unique name of this tool</param>
        /// <param name="defaultInstallLocation">Default installation location where the tool can be found</param>
        /// <param name="executableName">File name of the executable to be ran</param>
        /// <param name="url">The URL where the tool archive can be downloaded from </param>
        /// <param name="isDotNETProcess">If <c>true</c> the process will be executed with mono when not running on MS CLR</param>
        /// <param name="parameters">Application parameters</param>
        protected DownloadableSelfExtractingExternalTool(string name, string defaultInstallLocation, string executableName, Uri url, bool isDotNETProcess, IParameters parameters) 
            : base(name, defaultInstallLocation, executableName, url, isDotNETProcess, parameters)
        {
        }

        /// <summary>
        /// Downloads the tool to the given target path
        /// </summary>
        /// <param name="target">Target directory</param>
        protected override void DownloadAndDeploy(string target)
        {
            var tempInstaller = Path.GetTempFileName() + ".exe";

            using (var client = new WebClient())
            {
                client.DownloadFile(Url, tempInstaller);

                log.DebugFormat("Installing downloaded package to {0}", target);

                var process = System.Diagnostics.Process.Start(tempInstaller, GetInstallerArguments(target));
                if (process == null)
                    throw new InvalidOperationException("Could not start tool installer");

                process.WaitForExit();

                log.DebugFormat("Installation completed");
            }
        }

        /// <summary>
        /// Returns the command line arguments to be passed to the downloaded installer
        /// </summary>
        /// <param name="targetDir">Target directory where the tool should be installed</param>
        /// <returns>Returns command line arguments</returns>
        protected abstract string GetInstallerArguments(string targetDir);
    }
}