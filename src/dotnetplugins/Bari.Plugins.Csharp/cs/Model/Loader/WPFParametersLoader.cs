using System;
using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.Csharp.Model.Loader
{
    public class WPFParametersLoader : YamlProjectParametersLoaderBase<WPFParameters>
    {
        public WPFParametersLoader(IUserOutput output) : base(output)
        {
        }

        protected override string BlockName
        {
            get { return "wpf"; }
        }

        protected override WPFParameters CreateNewParameters(Suite suite)
        {
            return new WPFParameters();
        }

        protected override Dictionary<string, Action> GetActions(WPFParameters target, YamlNode value, YamlParser parser)
        {
             return new Dictionary<string, Action>
                {
                    {"application-definition", () => { target.ApplicationDefinition = ParseString(value); }}
                };
        }
    }
}