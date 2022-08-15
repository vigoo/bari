using System;
using System.IO;
using System.Net;
using Bari.Core.UI;

namespace Bari.Core.Tools
{
    /// <summary>
    /// Represents an external tool which can be automatically downloaded if missing
    /// </summary>
    public class DownloadableExternalTool: ExternalTool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (DownloadableExternalTool));

        private readonly string defaultInstallLocation;
        private readonly string bariInstallLocation;
        private readonly string executableName;
        private readonly Uri url;
        private readonly bool isDotNetProcess;

        /// <summary>
        /// Gets the download URI for the tool
        /// </summary>
        public Uri Url
        {
            get { return url; }
        }

        /// <summary>
        /// Defines a tool which can be downloaded if missing from an URL
        /// </summary>
        /// <param name="name">Unique name of this tool</param>
        /// <param name="defaultInstallLocation">Default installation location where the tool can be found</param>
        /// <param name="executableName">File name of the executable to be ran</param>
        /// <param name="url">The URL where the tool can be downloaded from </param>
        /// <param name="isDotNETProcess">If <c>true</c> the process will be executed with mono when not running on MS CLR</param>
        /// <param name="parameters">Application parameters</param>
        public DownloadableExternalTool(string name, string defaultInstallLocation, string executableName, Uri url, bool isDotNETProcess, IParameters parameters) 
            : base(name, parameters)
        {
            this.defaultInstallLocation = defaultInstallLocation;
            this.executableName = executableName;
            this.url = url;
            isDotNetProcess = isDotNETProcess;

            bariInstallLocation =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                             "bari", "tools", name);
        }

        /// <summary>
        /// Checks if the tool is available and download, copy, install etc. it if possible
        /// 
        /// <para>If the tool cannot be acquired then it throws an exception.</para>
        /// </summary>
        protected override void EnsureToolAvailable()
        {
            if (!File.Exists(Path.Combine(bariInstallLocation, executableName)) &&
                !File.Exists(Path.Combine(defaultInstallLocation, executableName)))
            {
                DownloadTool();
            }
        }        

        /// <summary>
        /// Gets the path to the executable of the external tool
        /// </summary>
        protected override string ToolPath
        {
            get
            {
                var downloadedExe = Path.Combine(bariInstallLocation, executableName);

                if (File.Exists(downloadedExe))
                    return downloadedExe;

                var defaultExe = Path.Combine(defaultInstallLocation, executableName);
                if (File.Exists(defaultExe))
                    return defaultExe;

                throw new InvalidOperationException(
                    String.Format("Tool not found at {0} or {1}", downloadedExe, defaultExe));
            }
        }

        protected override bool IsDotNETProcess
        {
            get { return isDotNetProcess; }
        }

        protected void DownloadTool()
        {
            log.InfoFormat("Downloading tool {0} from {1} to {2}", Name, url, bariInstallLocation);

            Directory.CreateDirectory(bariInstallLocation);
            DownloadAndDeploy(bariInstallLocation);

            log.DebugFormat("Download completed");
        }

        /// <summary>
        /// Downloads the tool to the given target path
        /// </summary>
        /// <param name="target">Target directory</param>
        protected virtual void DownloadAndDeploy(string target)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            using (var client = new WebClient())
            {
                client.DownloadFile(url, Path.Combine(target, executableName));
            }
        }
    }
}