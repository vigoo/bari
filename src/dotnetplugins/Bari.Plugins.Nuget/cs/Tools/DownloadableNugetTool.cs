using System;
using System.IO;
using System.Net;
using Bari.Core.Generic;
using Bari.Core.Tools;
using Bari.Core.UI;

namespace Bari.Plugins.Nuget.Tools
{
    /// <summary>
    /// A more generally usable version of <see cref="DownloadableExternalTool"/> where tool is downloaded
    /// using NuGet.
    /// </summary>
    public abstract class DownloadableNugetTool : DownloadableExternalTool
    {
        private readonly INuGet nuget;
        private readonly string packageName;
        private readonly string packageVersion;
        private readonly string bariInstallLocation;
        private readonly string defaultInstallLocation;
        private readonly string executableName;

        private String packageDirectory = "";

        /// <summary>
        /// Gets the path to the executable of the external tool
        /// </summary>
        protected override string ToolPath
        {
            get
            {
                var downloadedExe = Path.Combine(bariInstallLocation, packageDirectory, executableName);

                if (File.Exists(downloadedExe))
                    return downloadedExe;

                var defaultExe = Path.Combine(defaultInstallLocation, executableName);
                if (File.Exists(defaultExe))
                    return defaultExe;

                throw new InvalidOperationException(
                    String.Format("Tool not found at {0} or {1}", downloadedExe, defaultExe));
            }
        }


        /// <summary>
        /// Defines a tool which can be downloaded if missing from an URL
        /// </summary>
        /// <param name="nuget">NuGet implementation</param>
        /// <param name="name">Unique name of this tool</param>
        /// <param name="defaultInstallLocation">Default installation location where the tool can be found</param>
        /// <param name="executableName">File name of the executable to be ran</param>
        /// <param name="packageName">The NuGet package name</param>
        /// <param name="packageVersion">The NuGEt package version</param>
        /// <param name="parameters">Application parameters</param>
        protected DownloadableNugetTool(INuGet nuget, string name, string defaultInstallLocation, string executableName, string packageName, string packageVersion, IParameters parameters) 
            : base(name, defaultInstallLocation, executableName, new Uri("file:///"), true, parameters)
        {
            this.nuget = nuget;
            this.packageName = packageName;
            this.packageVersion = packageVersion;
            this.defaultInstallLocation = defaultInstallLocation;
            this.executableName = executableName;

            bariInstallLocation =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                             "bari", "tools", name);
        }

        protected override void DownloadAndDeploy(string target)
        {
            var results = nuget.InstallPackage(packageName, packageVersion, new LocalFileSystemDirectory(bariInstallLocation), "", false, NugetLibraryProfile.Net45);
            packageDirectory = results.Item1;
        }
    }
}