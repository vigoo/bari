using System;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    public class ReferenceLoader
    {
        public Reference[] LoadReference(YamlNode node)
        {
            var scalar = node as YamlScalarNode;
            if (scalar != null)
            {
                var refUri = new Uri(scalar.Value);

                if (refUri.Scheme == "alias")
                {
                    // Reference aliases are a core functionality and have special support here:                
                    return new[]
                    {
                        new Reference(refUri, ReferenceType.Build),
                        new Reference(refUri, ReferenceType.Runtime)
                    };
                }
                else
                {
                    return new[] {new Reference(refUri, ReferenceType.Build)};
                }
            }

            var mapping = node as YamlMappingNode;
            if (mapping != null)
            {
                var uri = ((YamlScalarNode)mapping.Children[new YamlScalarNode("uri")]).Value;
                var type = ReferenceType.Build;

                if (mapping.Children.ContainsKey(new YamlScalarNode("type")))
                {
                    Enum.TryParse(((YamlScalarNode) mapping.Children[new YamlScalarNode("type")]).Value, out type);
                }                

                return new[] {new Reference(new Uri(uri), type)};
            }

            return new Reference[0];
        }
    }
}