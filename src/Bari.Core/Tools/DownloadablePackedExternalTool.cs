using System;
using System.IO;
using System.Net;
using Ionic.Zip;

namespace Bari.Core.Tools
{
    /// <summary>
    /// A more generally usable version of <see cref="DownloadableExternalTool"/> where the downloaded file
    /// is not the external tool executable itself but a zip archive to be extracted first
    /// </summary>
    public class DownloadablePackedExternalTool: DownloadableExternalTool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (DownloadablePackedExternalTool));

        /// <summary>
        /// Defines a tool which can be downloaded if missing from an URL
        /// </summary>
        /// <param name="name">Unique name of this tool</param>
        /// <param name="defaultInstallLocation">Default installation location where the tool can be found</param>
        /// <param name="executableName">File name of the executable to be ran</param>
        /// <param name="url">The URL where the tool archive can be downloaded from </param>
        public DownloadablePackedExternalTool(string name, string defaultInstallLocation, string executableName, Uri url) 
            : base(name, defaultInstallLocation, executableName, url)
        {
        }

        protected override void DownloadAndDeploy(string target)
        {
            var tempZip = Path.GetTempFileName();

            var client = new WebClient();
            client.DownloadFile(Url, tempZip);

            log.DebugFormat("Extracting downloaded archive to {0}", target);
            using (var zip = new ZipFile(tempZip))
            {
                zip.ExtractAll(target);
            }

            log.DebugFormat("Extracting completed");
        }
    }
}