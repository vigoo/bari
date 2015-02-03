using System;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Ninject.Modules;

namespace Bari.Core
{
    public class DefaultPluginLoader: IPluginLoader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (DefaultPluginLoader));

        private readonly IReferenceBuilderFactory referenceBuilderFactory;
        private readonly IBuildContextFactory buildContextFactory;
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory targetRoot;

        public DefaultPluginLoader(IReferenceBuilderFactory referenceBuilderFactory, IBuildContextFactory buildContextFactory, [SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.referenceBuilderFactory = referenceBuilderFactory;
            this.buildContextFactory = buildContextFactory;
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
        }

        public void Load(Uri referenceUri)
        {
            log.InfoFormat("Trying to load plugin {0}", referenceUri);

            var path = LoadReference(referenceUri);

            if (path != null)
            {
                Load(path);
            }
            else
            {
                throw new InvalidReferenceException(String.Format("Referenced plugin could not be resolved: {0}", referenceUri));
            }
        }

        public void Load(string path)
        {
            Kernel.Root.Load(new[] {path});
        }

        public void Load(INinjectModule module)
        {
            Kernel.Root.Load(new[] {module});
        }

        private string LoadReference(Uri referenceUri)
        {
            var dummyProject = new Project("dummy", new Module("dummy", new Suite(suiteRoot)));
            var referenceBuilder =
                referenceBuilderFactory.CreateReferenceBuilder(new Reference(referenceUri, ReferenceType.Build), dummyProject);

            var buildContext = buildContextFactory.CreateBuildContext();
            buildContext.AddBuilder(referenceBuilder, new IBuilder[0]);
            var files = buildContext.Run(referenceBuilder);
            var file = files.FirstOrDefault(f => Path.GetExtension(f).ToLowerInvariant() == ".dll");
            var localTargetRoot = (LocalFileSystemDirectory) targetRoot;

            if (file != null)
                return Path.Combine(localTargetRoot.AbsolutePath, file);
            else
                return null;
        }
    }
}