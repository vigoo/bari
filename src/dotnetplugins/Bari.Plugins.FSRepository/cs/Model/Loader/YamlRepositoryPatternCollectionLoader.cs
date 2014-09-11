using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.FSRepository.Model.Loader
{
    /// <summary>
    /// Loads <see cref="RepositoryPatternCollection"/> parameter block from YAML suite definitions
    /// </summary>
    public class YamlRepositoryPatternCollectionLoader : YamlProjectParametersLoaderBase<RepositoryPatternCollection>
    {
        private readonly IFileSystemRepositoryAccess fsAccess;

        public YamlRepositoryPatternCollectionLoader(IFileSystemRepositoryAccess fsAccess)
        {
            this.fsAccess = fsAccess;
        }

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "fs-repositories"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="s">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override RepositoryPatternCollection CreateNewParameters(Suite s)
        {
            return new RepositoryPatternCollection(fsAccess);
        }

        /// <summary>
        /// Gets the mapping table
        /// 
        /// <para>The table contains the action to be performed for each supported option key</para>
        /// </summary>
        /// <param name="target">Target model object to be filled</param>
        /// <param name="value">Value to be parsed</param>
        /// <param name="parser">Parser to be used</param>
        /// <returns>Returns the mapping</returns>
        protected override Dictionary<string, Action> GetActions(RepositoryPatternCollection target, YamlNode value, YamlParser parser)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the YAML block
        /// </summary>
        /// <param name="name">Name of the block (same that was passed to <see cref="IYamlProjectParametersLoader.Supports"/>)</param>
        /// <param name="value">The YAML node representing the value</param>
        /// <param name="parser">The YAML parser to be used</param>
        /// <returns>Returns the loaded node</returns>
        public override IProjectParameters Load(Suite suite, string name, YamlNode value, YamlParser parser)
        {
            Suite = suite;

            var target = CreateNewParameters(suite);

            var seq = value as YamlSequenceNode;
            if (seq != null)
            {
                foreach (var node in parser.EnumerateNodesOf(seq).OfType<YamlScalarNode>())
                {                    
                    target.AddPattern(new RepositoryPattern(ParseString(node)));
                }
            }

            return target;
        }
    }
}
