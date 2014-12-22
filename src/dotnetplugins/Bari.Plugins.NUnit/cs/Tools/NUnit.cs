using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Tools;
using Bari.Core.UI;

namespace Bari.Plugins.NUnit.Tools
{
    public class NUnit: DownloadablePackedExternalTool, INUnit
    {
        private readonly IFileSystemDirectory targetDir;

        public NUnit([TargetRoot] IFileSystemDirectory targetDir, IParameters parameters)
            : base("NUnit", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NUnit 2.6.4"),
                   Path.Combine("NUnit-2.6.4", "bin", "nunit-console.exe"), new Uri("http://github.com/nunit/nunitv2/releases/download/2.6.4/NUnit-2.6.4.zip"), true, parameters)
        {
            this.targetDir = targetDir;
        }

        public bool RunTests(IEnumerable<TargetRelativePath> testAssemblies)
        {
            List<string> ps = testAssemblies.Select(p => (string)p).ToList();
            ps.Add("/result:test-report.xml");
            return Run(targetDir, ps.ToArray());
        }
    }
}