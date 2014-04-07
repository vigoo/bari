using System;
using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.Csharp.Model.Loader
{
    public class WPFParametersLoader : YamlProjectParametersLoaderBase<WPFParameters>
    {
        public WPFParametersLoader(Suite suite) : base(suite)
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