using System;
using System.IO;
using Bari.Core.Generic;
using Bari.Core.Tools;

namespace Bari.Plugins.InnoSetup.Tools
{
    public class InnoSetupCompiler : DownloadableSelfExtractingExternalTool, IInnoSetupCompiler
    {
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory targetRoot;

        public InnoSetupCompiler([SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot) :
            base("InnoSetup",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Inno Setup 5"),
                "iscc.exe",
                new Uri("http://www.jrsoftware.org/download.php/ispack-unicode.exe"))
        {
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
        }

        protected override string GetInstallerArguments(string targetDir)
        {
            return String.Format("/verysilent /norestart /dir=\"{0}\"", targetDir);
        }

        public void Compile(SuiteRelativePath scriptPath, TargetRelativePath targetPath, string version)
        {
            Run(suiteRoot, 
                "/dVERSION="+version,
                "/o"+Path.GetDirectoryName(Path.Combine(suiteRoot.GetRelativePath(targetRoot), targetPath)),
                "/f"+Path.GetFileName(targetPath),
                scriptPath);
        }
    }
}