using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio
{
    /// <summary>
    /// Default implementation
    /// 
    /// <para>Always returns the platform name <c>Bari</c></para>
    /// </summary>
    public class DefaultProjectPlatformManagement: IProjectPlatformManagement
    {
        public string GetDefaultPlatform(Project project)
        {
            return "Bari";
        }
    }
}