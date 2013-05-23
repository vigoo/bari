using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.Build;
using Bari.Plugins.VsCore.VisualStudio;

namespace Bari.Plugins.Csharp.VisualStudio
{
    /// <summary>
    /// The <see cref="ISlnProject"/> implementation for C# projects
    /// </summary>
    public class CsharpSlnProject: SlnProjectBase
    {
        private readonly ICsprojBuilderFactory builderFactory;
       
        public CsharpSlnProject([SuiteRoot] IFileSystemDirectory suiteRoot, ICsprojBuilderFactory builderFactory)
            : base(suiteRoot)
        {
            this.builderFactory = builderFactory;
        }

        protected override string SourceSetName
        {
            get { return "cs"; }
        }

        protected override string ProjectFileExtension
        {
            get { return ".csproj"; }
        }

        public override string ProjectTypeGuid
        {
            get { return "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"; }
        }

        public override IBuilder CreateBuilder(Project project)
        {
            return builderFactory.CreateCsprojBuilder(project);
        }
    }
}