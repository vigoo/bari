using System;
using System.Collections.Generic;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.InnoSetup.Packager.Loader
{
    public class InnoSetupPackagerParametersLoader : YamlProjectParametersLoaderBase<InnoSetupPackagerParameters>
    {
        public InnoSetupPackagerParametersLoader(IUserOutput output) : base(output)
        {
        }

        protected override string BlockName
        {
            get { return "innosetup"; }
        }

        protected override InnoSetupPackagerParameters CreateNewParameters(Suite suite)
        {
            return new InnoSetupPackagerParameters();
        }

        protected override Dictionary<string, Action> GetActions(InnoSetupPackagerParameters target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>
            {
                { "script", () => target.ScriptPath = new SuiteRelativePath(ParseString(value)) }
            };
        }
    }
}