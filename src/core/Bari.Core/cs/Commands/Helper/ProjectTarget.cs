using System.Collections.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Helper
{
    public class ProjectTarget: CommandTarget
    {
        private readonly Project project;

        public ProjectTarget(Project project)
        {
            this.project = project;
        }

        public override IEnumerable<Project> Projects
        {
            get
            {
                if (project is TestProject)
                    return new Project[0];
                else
                    return new[] {project};
            }
        }

        public override IEnumerable<Project> TestProjects
        {
            get
            {
                if (project is TestProject)
                    return new[] { project };
                else
                    return new Project[0];
            }
        }
    }
}