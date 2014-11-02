using System;
using System.Collections.Generic;
using System.IO;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace TestPlugin
{
    public class TestBuilder: IBuilder
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (TestBuilder));
        private readonly Project project;
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory targetRoot;

        public TestBuilder(Project project, [SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.project = project;
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
        }

        public void AddToContext(IBuildContext context)
        {
            context.AddBuilder(this, new IBuilder[0]);
        }

        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var targetDir = targetRoot.CreateDirectory(project.RelativeTargetPath);
            if (targetDir == null)
                targetDir = targetRoot.CreateDirectory(project.RelativeTargetPath);

            var sourceSet = project.GetSourceSet("scr");
            var result = new HashSet<TargetRelativePath>();
            foreach (var file in sourceSet.Files)
            {
                log.DebugFormat("Processing {0}", file);

                using (var reader = suiteRoot.ReadTextFile(file))
                {
                    var contents = reader.ReadToEnd();
                    var msg = String.Format("Hello {0}!!!", contents);
                    var fileName = Path.GetFileName(file);

                    using (var writer = targetDir.CreateTextFile(fileName + ".txt"))
                        writer.WriteLine(msg);

                    result.Add(new TargetRelativePath(project.RelativeTargetPath, fileName + ".txt"));
                }
            }

            return result;
        }

        public IDependencies Dependencies
        {
            get { return new NoDependencies(); }
        }

        public string Uid
        {
            get { return String.Format("test-{0}", project.Name); }
        }
    }
}
