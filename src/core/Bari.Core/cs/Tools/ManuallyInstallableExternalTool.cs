using System;
using System.IO;
using System.Linq;
using Bari.Core.Exceptions;

namespace Bari.Core.Tools
{
    /// <summary>
    /// Represents an external tool which cannot be automatically installed
    /// </summary>
    public class ManuallyInstallableExternalTool: ExternalTool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (ManuallyInstallableExternalTool));

        private readonly string defaultInstallLocation;
        private readonly string exeName;
        private readonly Uri manualUri;

        /// <summary>
        /// Defines the external tool
        /// </summary>
        /// <param name="name">Name of the tool</param>
        /// <param name="defaultInstallLocation">Default install location where the external tool can be found</param>
        /// <param name="exeName">Executable file name</param>
        /// <param name="manualUri">URI where the user can start solving the problem if the tool is missing</param>
        public ManuallyInstallableExternalTool(string name, string defaultInstallLocation, string exeName, Uri manualUri) : base(name)
        {
            this.defaultInstallLocation = defaultInstallLocation;
            this.exeName = exeName;
            this.manualUri = manualUri;
        }

        /// <summary>
        /// Checks if the tool is available and download, copy, install etc. it if possible
        /// 
        /// <para>If the tool cannot be acquired then it throws an exception.</para>
        /// </summary>
        protected override void EnsureToolAvailable()
        {
            bool exists;
            if (!Path.IsPathRooted(ToolPath))
            {
                var paths = Environment.GetEnvironmentVariable("PATH").Split(';');
                exists = paths.Any(path =>
                {
                    var possibility = Path.Combine(path, ToolPath);
                    log.DebugFormat("Trying {0}", possibility);
                    return File.Exists(possibility);
                });
            }
            else
            {
                exists = File.Exists(ToolPath);
            }

            if (!exists)
            {
                throw new ToolMustBeInstalledManually(Name, manualUri);
            }
        }

        /// <summary>
        /// Gets the path to the executable of the external tool
        /// </summary>
        protected override string ToolPath
        {
            get { return Path.Combine(defaultInstallLocation, exeName); }
        }
    }
}