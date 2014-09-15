using System.Linq;
using Bari.Core.Model;

namespace Bari.Plugins.AddonSupport.Model
{
    /// <summary>
    /// Parameter block defining the startup <see cref="Module"/> or <see cref="Project"/> for a <see cref="Product"/>
    /// </summary>
    public class StartupModuleParameters: IProjectParameters
    {
        private readonly Project project;
        private readonly Module module;

        public Project Project
        {
            get
            {
                if (project != null)
                {
                    return project;
                }
                else if (module != null)
                {
                    return module.Projects.FirstOrDefault(
                        prj => prj.Type == ProjectType.Executable || prj.Type == ProjectType.WindowsExecutable);
                }
                else
                {
                    return null;
                }
            }
        }

        public StartupModuleParameters(Project project)
        {
            this.project = project;
        }

        public StartupModuleParameters(Module module)
        {
            this.module = module;
        }

        public StartupModuleParameters()
        {            
        }
    }
}