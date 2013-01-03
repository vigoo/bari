using System;
using System.IO;
using Bari.Core.Exceptions;

namespace Bari.Core.Tools
{
    /// <summary>
    /// Represents an external tool which cannot be automatically installed
    /// </summary>
    public class ManuallyInstallableExternalTool: ExternalTool
    {
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
            if (!File.Exists(ToolPath))
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