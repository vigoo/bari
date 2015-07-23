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

        public override IEnumerable<TestProject> TestProjects
        {
            get
            {
                var testProject = project as TestProject;
                if (testProject != null)
                    return new[] { testProject };
                else
                    return new TestProject[0];
            }
        }
    }
}