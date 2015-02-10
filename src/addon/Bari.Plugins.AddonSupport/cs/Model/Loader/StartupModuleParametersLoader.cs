using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.AddonSupport.Model.Loader
{
    public class StartupModuleParametersLoader : YamlProjectParametersLoaderBase<StartupModuleParameters>
    {
        public StartupModuleParametersLoader(IUserOutput output) : base(output)
        {
        }

        protected override string BlockName
        {
            get { return "startup"; }
        }

        protected override StartupModuleParameters CreateNewParameters(Suite suite)
        {
               return new StartupModuleParameters();
        }

        public override IProjectParameters Load(Suite suite, string name, YamlNode value, YamlParser parser)
        {
            var startupName = ParseString(value);

            if (suite.HasModule(startupName))
                return new StartupModuleParameters(suite.GetModule(startupName));
            else
            {
                var project = (from module in suite.Modules
                               where module.HasProject(startupName)
                               select module.GetProject(startupName)).FirstOrDefault();

                if (project != null)
                    return new StartupModuleParameters(project);
                else
                    return new StartupModuleParameters();
            }
        }

        protected override Dictionary<string, Action> GetActions(StartupModuleParameters target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>();
        }
    }
}