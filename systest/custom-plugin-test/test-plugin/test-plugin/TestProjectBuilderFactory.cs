using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.MergingTag;
using Bari.Core.Generic;
using Bari.Core.Model;
using TestPlugin;

namespace test_plugin
{
    class TestProjectBuilderFactory : IProjectBuilderFactory
    {
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory targetRoot;
        private readonly ICoreBuilderFactory coreBuilderFactory;

        public TestProjectBuilderFactory([SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot, ICoreBuilderFactory coreBuilderFactory)
        {
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
            this.coreBuilderFactory = coreBuilderFactory;
        }

        public IBuilder Create(IEnumerable<Project> projects)
        {
            var builders = new List<IBuilder>();
            var prjs = projects.ToList();

            foreach (var project in prjs)
            {
                if (project.HasNonEmptySourceSet("scr"))
                {
                    var builder = new TestBuilder(project, suiteRoot, targetRoot);
                    builders.Add(builder);
                }
            }

            return coreBuilderFactory.Merge(builders.ToArray(), new ProjectBuilderTag(string.Format("Test builders of {0}", string.Join(", ", prjs.Select(p => p.Name))), prjs));
        }
    }
}
