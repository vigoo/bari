using System;

namespace Bari.Plugins.Csharp.Exceptions
{
    public class MSBuildFailedException: Exception
    {
        public MSBuildFailedException()
        {
        }

        public override string ToString()
        {
            return "MSBuild returned with error";
        }
    }
}