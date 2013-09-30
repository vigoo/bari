using System;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    public class ReferenceLoader
    {
        public Reference LoadReference(YamlNode node)
        {
            var scalar = node as YamlScalarNode;
            if (scalar != null)
            {
                return new Reference(new Uri(scalar.Value), ReferenceType.Build);
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

                return new Reference(new Uri(uri), type);
            }

            return null;
        }
    }
}