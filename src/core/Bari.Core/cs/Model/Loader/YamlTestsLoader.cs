using System;
using System.Collections.Generic;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    public class YamlTestsLoader : YamlProjectParametersLoaderBase<Tests>
    {
        public YamlTestsLoader(IUserOutput output) : base(output)
        {
        }

        protected override string BlockName
        {
            get { return "test"; }
        }

        protected override Tests CreateNewParameters(Suite suite)
        {
            return new Tests();
        }

        protected override Dictionary<string, Action> GetActions(Tests target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>
            {
                {"enabled-runners", () => target.EnableTestRunners(ParseStringArray(parser, value))}
            };
        }
    }
}