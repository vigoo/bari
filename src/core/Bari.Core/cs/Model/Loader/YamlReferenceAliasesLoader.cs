using System;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Loads <see cref="ReferenceAliases"/> parameter block from YAML files
    /// </summary>
    public class YamlReferenceAliasesLoader : YamlProjectParametersLoaderBase<ReferenceAliases>
    {
        private readonly ReferenceLoader referenceLoader = new ReferenceLoader();

        /// <summary>
        /// Gets the name of the yaml block the loader supports
        /// </summary>
        protected override string BlockName
        {
            get { return "aliases"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter model type 
        /// </summary>
        /// <param name="s">Current suite</param>
        /// <returns>Returns the new instance to be filled with loaded data</returns>
        protected override ReferenceAliases CreateNewParameters(Suite s)
        {
            return new ReferenceAliases();
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
        protected override Dictionary<string, Action> GetActions(ReferenceAliases target, YamlNode value, YamlParser parser)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applies a parameter definition to the given parameters model.
        /// 
        /// <para>This only has to be overridden to provide a custom parameter parsing logic. Otherwise
        /// just override <see cref="YamlProjectParametersLoaderBase{TParamType}.GetActions"/></para>
        /// </summary>
        /// <param name="target">Target model object to be filled</param>
        /// <param name="name">Option name</param>
        /// <param name="value">Option value to be parsed</param>
        /// <param name="parser">Parser to be used</param>
        protected override void TryAddParameter(ReferenceAliases target, string name, YamlNode value, YamlParser parser)
        {
            var references = new HashSet<Reference>();

            var seq = value as YamlSequenceNode;
            if (seq != null)
            {
                foreach (var referenceNode in parser.EnumerateNodesOf(seq))
                {
                    foreach (var reference in referenceLoader.LoadReference(referenceNode))
                        references.Add(reference);
                }
            }
            else
            {
                foreach (var reference in referenceLoader.LoadReference(value))
                    references.Add(reference);
            }

            target.Add(name, references);
        }       
    }
}