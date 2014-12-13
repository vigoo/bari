using System.Collections.Generic;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Plugins.VsCore.VisualStudio;

namespace Bari.Plugins.VsCore.Model.Discovery
{    
    public class ProjectPathExplorer: ISuiteExplorer
    {
        private readonly List<ISlnProject> supportedSlnProjects;
        private readonly IProjectPathManagement pathManagement;
        public ProjectPathExplorer(IEnumerable<ISlnProject> supportedSlnProjects, IProjectPathManagement pathManagement)
        {
            this.supportedSlnProjects = supportedSlnProjects.ToList();
            this.pathManagement = pathManagement;
        }

        public void ExtendWithDiscoveries(Suite suite)
        {
            foreach (var module in suite.Modules)
            {
                foreach (var project in module.Projects.Concat(module.TestProjects))
                {
                    foreach (var slnProject in supportedSlnProjects)
                    {
                        if (slnProject.SupportsProject(project))
                        {
                            pathManagement.RegisterProjectFile(project, new SuiteRelativePath(slnProject.GetSuiteRelativeProjectFilePath(project)));
                        }
                    }
                }
            }
        }
    }
}