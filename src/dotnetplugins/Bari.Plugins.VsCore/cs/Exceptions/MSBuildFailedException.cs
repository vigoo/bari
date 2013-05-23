using System;

namespace Bari.Plugins.VsCore.Exceptions
{
    /// <summary>
    /// Exception thrown if MSBuild returns with an error
    /// </summary>
    [Serializable]
    public class MSBuildFailedException: Exception
    {
        /// <summary>
        /// Constructs the exception
        /// </summary>
        public MSBuildFailedException() 
            : base("MSBuild returned with an error")
        {
        }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/></PermissionSet>
        public override string ToString()
        {
            return Message;
        }
    }
}