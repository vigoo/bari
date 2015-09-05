using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.InnoSetup.Packager
{
    public class InnoSetupPackagerParameters: IPackagerParameters
    {
        private SuiteRelativePath scriptPath;

        public SuiteRelativePath ScriptPath
        {
            get { return scriptPath; }
            set { scriptPath = value; }
        }
    }
}