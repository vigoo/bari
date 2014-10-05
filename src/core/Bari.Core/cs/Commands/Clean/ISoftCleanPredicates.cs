using System;

namespace Bari.Core.Commands.Clean
{
    public interface ISoftCleanPredicates
    {
        void Add(Func<string, bool> keepPredicate);
        bool ShouldDelete(string relativePath);
    }
}