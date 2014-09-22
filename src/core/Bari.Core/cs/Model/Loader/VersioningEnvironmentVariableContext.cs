using System;
using Bari.Core.Generic;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Environment variable resolution context which falls back automatically to '0' for non-existing variables
    /// </summary>
    public class VersioningEnvironmentVariableContext : IEnvironmentVariableContext
    {
        public string GetEnvironmentVariable(string name)
        {
            var result = Environment.GetEnvironmentVariable(name);
            if (String.IsNullOrWhiteSpace(result))
                return "0";
            else
                return result;
        }
    }
}