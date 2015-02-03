using System;
using Bari.Core.Generic;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Environment variable resolution context which falls back automatically to '0' for non-existing variables
    /// </summary>
    public class VersioningEnvironmentVariableContext: IEnvironmentVariableContext
    {    
        private readonly IEnvironmentVariableContext baseImpl;

        public VersioningEnvironmentVariableContext(IEnvironmentVariableContext baseImpl)
        {
            this.baseImpl = baseImpl;
        }

        public string GetEnvironmentVariable(string name)
        {
            var result = baseImpl.GetEnvironmentVariable(name);
            if (String.IsNullOrWhiteSpace(result))
                return "0";
            else
                return result;
        }

        public void Define(string name, string value)
        {
            baseImpl.Define(name, value);
        }

        public void Undefine(string name)
        {
            baseImpl.Undefine(name);
        }
    }
}