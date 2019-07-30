using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Tools;
using Bari.Core.UI;
using Bari.Plugins.Nuget.Tools;

namespace Bari.Plugins.NUnit.Tools
{
    public class NUnit : DownloadableNugetTool, INUnit
    {
        private readonly IFileSystemDirectory targetDir;
        private const string executableName = "nunit3-console.exe";

        public NUnit(INuGet nuget, [TargetRoot] IFileSystemDirectory targetDir, IParameters parameters)
            : base(nuget, "NUnit", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "NUnit.org", "nunit-console"),
                executableName, "NUnit.Runners", "3.10.0", parameters)
        {
            this.targetDir = targetDir;
        }

        private string BasePath
        {
            get { return Path.Combine(BariInstallLocation, "NUnit.ConsoleRunner.3.10.0", "tools", executableName); }
        }

        protected override string ToolPath
        {
            get
            {
                var downloadedExe = BasePath;

                if (File.Exists(downloadedExe))
                    return downloadedExe;

                var defaultExe = Path.Combine(DefaultInstallLocation, executableName);
                if (File.Exists(defaultExe))
                    return defaultExe;

                throw new InvalidOperationException(
                    String.Format("Tool not found at {0} or {1}", downloadedExe, defaultExe));
            }
        }

        /// <summary>
        /// Checks if the tool is available and download, copy, install etc. it if possible
        /// 
        /// <para>If the tool cannot be acquired then it throws an exception.</para>
        /// </summary>
        protected override void EnsureToolAvailable()
        {
            if (!File.Exists(BasePath) &&
                !File.Exists(Path.Combine(DefaultInstallLocation, executableName)))
            {
                DownloadTool();
            }
        }

        public bool RunTests(IEnumerable<TargetRelativePath> testAssemblies)
        {
            List<string> ps = testAssemblies.Select(p => (string)p).ToList();
            ps.Add("-result=test-report.xml");
            return Run(targetDir, ps.ToArray());
        }
    }
}