using System;
using System.IO;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Tools;
using Bari.Core.UI;

namespace Bari.Plugins.InnoSetup.Tools
{
    public class InnoSetupCompiler : DownloadableSelfExtractingExternalTool, IInnoSetupCompiler
    {
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory targetRoot;

        public InnoSetupCompiler([SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot, IParameters parameters) :
            base("InnoSetup",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Inno Setup 5"),
                "iscc.exe",
                new Uri("http://www.jrsoftware.org/download.php/ispack-unicode.exe"),
                false, parameters)
        {
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
        }

        protected override string GetInstallerArguments(string targetDir)
        {
            return String.Format("/verysilent /norestart /dir=\"{0}\"", targetDir);
        }

        public void Compile(SuiteRelativePath scriptPath, TargetRelativePath targetPath, string version, Goal targetGoal)
        {
            var platform = targetGoal.Has("x64") ? "x64" : "x86";

            Run(suiteRoot, 
                "/dVERSION="+version,
                "/dPLATFORM="+platform,
                "/dGOAL="+targetGoal.Name,
                "/o"+Path.GetDirectoryName(Path.Combine(suiteRoot.GetRelativePath(targetRoot), targetPath)),
                scriptPath);
        }
    }
}