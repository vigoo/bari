using System.IO;
using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio
{
    public abstract class SlnProjectBase : ISlnProject
    {
        private readonly IFileSystemDirectory suiteRoot;

        protected SlnProjectBase(IFileSystemDirectory suiteRoot)
        {
            this.suiteRoot = suiteRoot;
        }

        /// <summary>
        /// Checks if the given project is supported by this implementation
        /// </summary>
        /// <param name="project">Project to check</param>
        /// <returns>Returns <c>true</c> if the project is supported</returns>
        public virtual bool SupportsProject(Project project)
        {
            return project.HasNonEmptySourceSet(SourceSetName);
        }

        protected abstract string SourceSetName { get; }
        protected abstract string ProjectFileExtension { get; }

        public virtual string GetSuiteRelativeProjectFilePath(Project project)
        {
            string projectFileName = project.Name + ProjectFileExtension;
            return Path.Combine(suiteRoot.GetRelativePath(project.RootDirectory.GetChildDirectory(SourceSetName)), projectFileName);
        }

        public abstract string ProjectTypeGuid { get; }

        public abstract IBuilder CreateBuilder(Project project);
    }
}