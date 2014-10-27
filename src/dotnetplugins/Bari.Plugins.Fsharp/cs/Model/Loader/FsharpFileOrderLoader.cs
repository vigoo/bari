using System;
using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Loader;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Plugins.Fsharp.Model.Loader
{
    public class FsharpFileOrderLoader : YamlProjectParametersLoaderBase<FsharpFileOrder>
    {
        public FsharpFileOrderLoader(IUserOutput output) : base(output)
        {
        }

        protected override string BlockName
        {
            get { return "order"; }
        }

        protected override FsharpFileOrder CreateNewParameters(Suite suite)
        {
            return new FsharpFileOrder();
        }

        protected override Dictionary<string, Action> GetActions(FsharpFileOrder target, YamlNode value, YamlParser parser)
        {
            throw new NotSupportedException();
        }

        public override IProjectParameters Load(Suite suite, string name, YamlNode value, YamlParser parser)
        {
            Suite = suite;

            var target = CreateNewParameters(suite);

            var seq = value as YamlSequenceNode;
            if (seq != null)
            {
                foreach (var fileNameNode in seq.Children)
                {
                    string fileName = ParseString(fileNameNode);
                    target.Add(fileName);
                }
            }

            return target;
        }
    }
}