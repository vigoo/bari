using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bari.Core.Generic;

namespace Bari.Plugins.Gallio.Tools
{
    public class Gallio: IGallio
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (Gallio));
        private readonly IFileSystemDirectory targetDir;

        public Gallio([TargetRoot] IFileSystemDirectory targetDir)
        {
            this.targetDir = targetDir;
        }

        public void RunTests(IEnumerable<TargetRelativePath> testAssemblies)
        {
            Run(targetDir, testAssemblies.Select(p => (string)p).ToArray());
        }

        private void Run(IFileSystemDirectory root, params string[] args)
        {
            var localRoot = root as LocalFileSystemDirectory;
            if (localRoot != null)
            {
                const string path = @"c:\program files\gallio\bin\Gallio.Echo.exe"; // TODO: this must not be hard-coded and could be downloaded dynamically from nuget site (http://nuget.codeplex.com/downloads/get/412077)

                var psi = new ProcessStartInfo
                {
                    FileName = path,
                    WorkingDirectory = localRoot.AbsolutePath,
                    Arguments = String.Join(" ", args),
                    UseShellExecute = false
                };

                log.DebugFormat("Executing {0} with arguments {1}", path, psi.Arguments);

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                }
            }
            else
            {
                throw new NotSupportedException("Only local file system is supported for Gallio!");
            }
        }        
    }
}