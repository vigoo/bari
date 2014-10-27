using System;
using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.VCpp.Model.Loader
{
    /// <summary>
    /// Loader for <see cref="VCppProjectManifestParameters"/> parameter block from YAML files
    /// </summary>
    public class VCppManifestParametersLoader : YamlProjectParametersLoaderBase<VCppProjectManifestParameters>
    {
        public VCppManifestParametersLoader(IUserOutput output) : base(output)
        {
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "manifest"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="suite">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override VCppProjectManifestParameters CreateNewParameters(Suite suite)
        {
            return new VCppProjectManifestParameters(suite);
        }

        protected override Dictionary<string, Action> GetActions(VCppProjectManifestParameters target, YamlNode value, YamlParser parser)
        {
            return new Dictionary<string, Action>
            {
                { "generate-manifest", () => target.GenerateManifest = ParseBool(value) },
                { "embed-manifest", () => target.EmbedManifest = ParseBool(value) }
            };
        }
    }
}