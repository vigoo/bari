using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio;

namespace Bari.Plugins.VCpp.VisualStudio
{
    public class CppProjectPlatformManagement: IProjectPlatformManagement
    {
        private readonly IProjectPlatformManagement baseImpl;

        public CppProjectPlatformManagement(IProjectPlatformManagement baseImpl)
        {
            this.baseImpl = baseImpl;
        }

        public string GetDefaultPlatform(Project project)
        {
            if (project.HasNonEmptySourceSet("cpp"))
                return "Win32"; // TODO: 64 bit support
            else
                return baseImpl.GetDefaultPlatform(project);
        }
    }
}