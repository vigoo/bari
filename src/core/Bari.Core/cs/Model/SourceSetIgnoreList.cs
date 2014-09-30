using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bari.Core.Generic;

namespace Bari.Core.Model
{
    public class SourceSetIgnoreList
    {
        private readonly ISet<Regex> ignoreExpressions = new HashSet<Regex>();

        public IEnumerable<string> Expressions
        {
            get { return ignoreExpressions.Select(r => r.ToString()); }
        }

        public bool IsIgnored(SuiteRelativePath path)
        {
            return ignoreExpressions.Any(expression => expression.IsMatch(path));
        }

        public void Add(string expression)
        {
            ignoreExpressions.Add(new Regex(expression, RegexOptions.Compiled));
        }
    }
}