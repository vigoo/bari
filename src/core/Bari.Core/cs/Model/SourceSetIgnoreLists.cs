using System.Collections.Generic;

namespace Bari.Core.Model
{
    public class SourceSetIgnoreLists
    {
        private readonly IDictionary<SourceSetType, SourceSetIgnoreList> ignoreLists =
            new Dictionary<SourceSetType, SourceSetIgnoreList>();

        public SourceSetIgnoreList Get(SourceSetType type)
        {
            SourceSetIgnoreList ignoreList;
            if (!ignoreLists.TryGetValue(type, out ignoreList))
            {
                ignoreList = new SourceSetIgnoreList();
                ignoreLists.Add(type, ignoreList);
            }

            return ignoreList;
        }
    }
}