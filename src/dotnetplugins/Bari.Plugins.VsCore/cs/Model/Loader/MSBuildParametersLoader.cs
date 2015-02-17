using System;
using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.VsCore.Model.Loader
{
    public class MSBuildParametersLoader : YamlProjectParametersLoaderBase<MSBuildParameters>
    {
        public MSBuildParametersLoader(IUserOutput output) : base(output)
        {
        }

        protected override string BlockName
        {
            get { return "msbuild"; }
        }

        protected override MSBuildParameters CreateNewParameters(Suite suite)
        {
            return new MSBuildParameters();
        }

        protected override Dictionary<string, Action> GetActions(MSBuildParameters target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>
                {
                    {"version", () => { target.Version = ParseEnum<MSBuildVersion>(value, "MSBuild version"); }}
                };
        }
    }
}