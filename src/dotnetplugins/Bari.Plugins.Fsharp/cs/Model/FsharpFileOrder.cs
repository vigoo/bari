using System.Collections.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Fsharp.Model
{
    public class FsharpFileOrder : IProjectParameters
    {
        private readonly IList<string> orderedFiles;

        public IEnumerable<string> OrderedFiles
        {
            get { return orderedFiles; }
        }

        public FsharpFileOrder()
        {
            orderedFiles = new List<string>();
        }

        public void Clear()
        {
            orderedFiles.Clear();
        }

        public void Add(string file)
        {
            orderedFiles.Add(file);
        }
    }
}