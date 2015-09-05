using System;
using Bari.Core.Model.Loader;
using Bari.Core.Model.Parameters;
using Bari.Core.UI;
using Bari.Core.Model;
using YamlDotNet.RepresentationModel;
using System.Collections.Generic;

namespace Bari.Plugins.VCpp.Model.Loader
{
    public class VCppToolchainParametersLoader : YamlProjectParametersLoaderBase<VCppProjectToolchainParameters>
    {
        public VCppToolchainParametersLoader(IUserOutput output) : base(output)
        {
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "toolchain"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="suite">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override VCppProjectToolchainParameters CreateNewParameters(Suite suite)
        {
            return new VCppProjectToolchainParameters();
        }

        public override IProjectParameters Load(Suite suite, string name, YamlNode value, YamlParser parser)
        {
            Suite = suite;

            var result = CreateNewParameters(suite);
            result.PlatformToolSet = ParseEnum<PlatformToolSet>(value, "C++ platform tool set");

            return result;
        }

        protected override Dictionary<string, Action> GetActions(VCppProjectToolchainParameters target, YamlNode value, YamlParser parser)
        {
            throw new NotImplementedException();
        }
    }
}

