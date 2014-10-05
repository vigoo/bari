using System;
using System.Collections.Generic;
using System.Linq;

namespace Bari.Core.Commands.Clean
{
    public class SoftCleanPredicates: ISoftCleanPredicates
    {
        private readonly ISet<Func<string, bool>> keepPredicates = new HashSet<Func<string, bool>>();

        public void Add(Func<string, bool> keepPredicate)
        {
            keepPredicates.Add(keepPredicate);
        }

        public bool ShouldDelete(string relativePath)
        {
            return !keepPredicates.Any(p => p(relativePath));
        }
    }
}