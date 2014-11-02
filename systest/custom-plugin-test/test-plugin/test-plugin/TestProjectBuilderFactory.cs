using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using TestPlugin;

namespace test_plugin
{
    class TestProjectBuilderFactory : IProjectBuilderFactory
    {
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory targetRoot;

        public TestProjectBuilderFactory([SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
        }

        public IBuilder AddToContext(IBuildContext context, IEnumerable<Project> projects)
        {
            var builders = new List<IBuilder>();

            foreach (var project in projects)
            {
                if (project.HasNonEmptySourceSet("scr"))
                {
                    var builder = new TestBuilder(project, suiteRoot, targetRoot);
                    builder.AddToContext(context);
                    builders.Add(builder);
                }
            }

            return builders.ToArray().Merge();
        }
    }
}
