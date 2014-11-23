using System;

namespace Bari.Core.Exceptions
{
    public class BuilderCantRunException: Exception
    {
        public BuilderCantRunException(string builderUid)
            : base("Could not execute builder " + builderUid)
        {            
        }
    }
}