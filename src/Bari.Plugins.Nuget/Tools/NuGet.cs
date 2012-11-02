using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bari.Core.Generic;

namespace Bari.Plugins.Nuget.Tools
{
    public class NuGet : INuGet
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (NuGet));

        public IEnumerable<TargetRelativePath> InstallPackage(string name, IFileSystemDirectory root, string relativeTargetDirectory)
        {
            Run(root, "install", name, "-o", "\""+relativeTargetDirectory+"\"");

            return new TargetRelativePath[0]; // TODO
        }

        private void Run(IFileSystemDirectory root, params string[] args)
        {
            var localRoot = root as LocalFileSystemDirectory;
            if (localRoot != null)
            {
                const string path = @"c:\programs\NuGet.exe"; // TODO: this must not be hard-coded and could be downloaded dynamically from nuget site (http://nuget.codeplex.com/downloads/get/412077)

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
                throw new NotSupportedException("Only local file system is supported for NuGet!");
            }
        }
    }
}