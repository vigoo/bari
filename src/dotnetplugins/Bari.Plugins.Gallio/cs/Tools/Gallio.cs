using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Tools;
using Bari.Core.UI;
using Bari.Plugins.Nuget.Tools;

namespace Bari.Plugins.Gallio.Tools
{
    public class Gallio: DownloadableNugetTool, IGallio
    {
        private readonly IFileSystemDirectory targetDir;

        public Gallio(INuGet nuget, [TargetRoot] IFileSystemDirectory targetDir, IParameters parameters)
            : base(nuget, "Gallio", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "gallio"),
                   Path.Combine("bin", "Gallio.Echo.exe"), "GallioBundle", "3.4.14", parameters)
        {
            this.targetDir = targetDir;
        }

        public bool RunTests(IEnumerable<TargetRelativePath> testAssemblies)
        {
            List<string> ps = testAssemblies.Select(p => (string)p).ToList();
            ps.Add("/report-type:Xml");
            ps.Add("/report-directory:.");
            ps.Add("/report-formatter-property:AttachmentContentDisposition=Absent");
            ps.Add("/report-name-format:test-report");
            return Run(targetDir, ps.ToArray());
        }
    }
}