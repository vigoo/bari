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
            var suite = project.Module.Suite;
            if (project.HasNonEmptySourceSet("cpp"))
                return suite.ActiveGoal.Has("x64") ? "x64" : "Win32";
            else
                return baseImpl.GetDefaultPlatform(project);
        }
    }
}