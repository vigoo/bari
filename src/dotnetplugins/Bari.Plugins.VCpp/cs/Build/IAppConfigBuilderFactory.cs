using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.UI;

namespace Bari.Plugins.VCpp.Build
{
    public interface IAppConfigBuilderFactory
    {
        AppConfigBuilder CreateAppConfigBuilder(Project project);
    }
}
