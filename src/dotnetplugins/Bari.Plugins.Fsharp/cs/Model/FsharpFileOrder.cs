using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.Fsharp.Model
{
    public class FsharpFileOrder : IProjectParameters
    {
        private readonly IList<string> orderedFiles;

        public string[] OrderedFiles
        {
            get { return orderedFiles.ToArray(); }
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