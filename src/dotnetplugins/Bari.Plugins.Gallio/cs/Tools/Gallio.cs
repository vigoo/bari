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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (Gallio));
        private readonly IFileSystemDirectory targetDir;

        public Gallio([TargetRoot] IFileSystemDirectory targetDir)
            : base("Gallio", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "gallio"),
                   @"bin\Gallio.Echo.exe", new Uri("http://mb-unit.googlecode.com/files/GallioBundle-3.4.14.0.zip"))
        {
            this.targetDir = targetDir;
        }

        public void RunTests(IEnumerable<TargetRelativePath> testAssemblies)
        {
            List<string> ps = testAssemblies.Select(p => (string)p).ToList();
            Run(targetDir, ps.ToArray());
        }
    }
}