using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Bari.Plugins.PythonScripts.Scripting
{
    public abstract class ScriptRunnerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ScriptRunnerBase));

        private readonly IFileSystemDirectory targetRoot;
        private readonly IReferenceBuilderFactory referenceBuilderFactory;
        private readonly IBuildContextFactory buildContextFactory;

        public IFileSystemDirectory TargetRoot
        {
            get { return targetRoot; }
        }

        protected ScriptRunnerBase(IFileSystemDirectory targetRoot, IReferenceBuilderFactory referenceBuilderFactory, IBuildContextFactory buildContextFactory)
        {
            this.referenceBuilderFactory = referenceBuilderFactory;
            this.buildContextFactory = buildContextFactory;
            this.targetRoot = targetRoot;
        }

        protected ScriptEngine CreateEngine()
        {
            var engine = Python.CreateEngine();

            var libRoot = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lib");
            log.DebugFormat("Python lib root is {0}", libRoot);

            engine.SetSearchPaths(new[] { libRoot });

            return engine;
        }

        protected void AddGetToolToScope(ScriptScope scope, Project project)
        {
            scope.SetVariable("get_tool", (Func<string, string, string>)((uri, fname) => GetTool(uri, fname, project)));
        }

        protected TargetRelativePath GetTargetRelativePath(IFileSystemDirectory innerRoot, string path)
        {
            return new TargetRelativePath(TargetRoot.GetRelativePath(innerRoot), path);
        }

        private string GetTool(string uri, string fileName, Project project)
        {
            var referenceBuilder = referenceBuilderFactory.CreateReferenceBuilder(
                new Reference(new Uri(uri), ReferenceType.Build), project);

            var buildContext = buildContextFactory.CreateBuildContext();
            buildContext.AddBuilder(referenceBuilder, new IBuilder[0]);
            var files = buildContext.Run(referenceBuilder);
            var file = files.FirstOrDefault(f => Path.GetFileName(f).Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
            var localTargetRoot = (LocalFileSystemDirectory)targetRoot;

            if (file != null)
                return Path.Combine(localTargetRoot.AbsolutePath, file);
            else
                return null;
        }
    }
}