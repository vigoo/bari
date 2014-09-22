using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Bari.Core.Generic
{
    public static class EnvironmentVariables
    {
        private static readonly Regex envVarsRegex = new Regex(@"\$([a-zA-Z0-9]+)", RegexOptions.Singleline);

        public static bool ResolveEnvironmentVariables(IEnvironmentVariableContext context, StringBuilder resultBuilder, Action<string> failLog = null)
        {
            Match match;
            do
            {
                match = envVarsRegex.Match(resultBuilder.ToString());

                if (match.Success)
                {
                    string value = context.GetEnvironmentVariable(match.Groups[1].Captures[0].Value);
                    if (value == null)
                    {
                        if (failLog != null)
                            failLog(match.Value);                        
                        return false;
                    }
                    else
                    {
                        resultBuilder.Remove(match.Index, match.Length);
                        resultBuilder.Insert(match.Index, value);
                    }
                }
            } while (match.Success);

            return true;
        }
    }
}