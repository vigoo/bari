using System;
using System.Collections.Generic;

namespace Bari.Core.Generic
{
    /// <summary>
    /// Default implementation, uses the system environment and also 
    /// extensible at runtime
    /// </summary>
    public class DefaultEnvironmentVariableContext: IEnvironmentVariableContext
    {
        private readonly IDictionary<string, string> runtimeVariables = new Dictionary<string, string>();

        public string GetEnvironmentVariable(string name)
        {
            string value;
            if (!runtimeVariables.TryGetValue(name, out value))
                return Environment.GetEnvironmentVariable(name);
            else
                return value;
        }

        public void Define(string name, string value)
        {
            runtimeVariables.Add(name, value);
        }

        public void Undefine(string name)
        {
            runtimeVariables.Remove(name);
        }
    }
}