using System;
using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.VCpp.Model.Loader
{
    /// <summary>
    /// Loads <see cref="VCppProjectATLParameters"/> parameter block from YAML files
    /// </summary>
    public class VCppATLParametersLoader: YamlProjectParametersLoaderBase<VCppProjectATLParameters>
    {
        public VCppATLParametersLoader(Suite suite) : base(suite)
        {
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "atl"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="suite">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override VCppProjectATLParameters CreateNewParameters(Suite suite)
        {
            return new VCppProjectATLParameters();
        }

        public override IProjectParameters Load(string name, YamlNode value, YamlParser parser)
        {
            var result = CreateNewParameters(Suite);
            result.UseOfATL = ParseEnum<UseOfATL>(value, "ATL usage mode");

            return result;
        }

        protected override Dictionary<string, Action> GetActions(VCppProjectATLParameters target, YamlNode value, YamlParser parser)
        {
            throw new NotImplementedException();
        }
    }
}