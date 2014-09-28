using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Tools;

namespace Bari.Plugins.Gallio.Tools
{
    public class Gallio: DownloadablePackedExternalTool, IGallio
    {
        private readonly IFileSystemDirectory targetDir;

        public Gallio([TargetRoot] IFileSystemDirectory targetDir)
            : base("Gallio", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "gallio"),
                   @"bin\Gallio.Echo.exe", new Uri("http://mb-unit.googlecode.com/files/GallioBundle-3.4.14.0.zip"))
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