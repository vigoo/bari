using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bari.Core.Build.Cache
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AggressiveCacheRestoreAttribute: Attribute
    {
        private string[] exceptions = new string[0];
        private Regex[] exceptionExpressions = new Regex[0];

        public string[] Exceptions
        {
            get { return exceptions; }
            set
            {
                if (exceptions != value)
                {
                    exceptions = value;
                    BuildRegularExpressions();
                }
            }
        }

        public Regex[] ExceptionExpressions
        {
            get { return exceptionExpressions; }
        }

        private void BuildRegularExpressions()
        {
            exceptionExpressions =
                exceptions.Select(
                e => new Regex(e, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline))
                    .ToArray();
        }
    }
}