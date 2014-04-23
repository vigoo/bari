using System.IO;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace Bari.Core.Generic
{
    public sealed class FileDotEngine : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            string output = outputFileName;
            File.WriteAllText(output, dot);
            return output;
        }
    }
}