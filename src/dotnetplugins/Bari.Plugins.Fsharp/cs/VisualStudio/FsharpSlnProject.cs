using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Fsharp.Build;
using Bari.Plugins.VsCore.VisualStudio;

namespace Bari.Plugins.Fsharp.VisualStudio
{
    /// <summary>
    /// The <see cref="ISlnProject"/> implementation for F# projects
    /// </summary>
    public class FsharpSlnProject : SlnProjectBase
    {
        private readonly IFsprojBuilderFactory builderFactory;

        public FsharpSlnProject([SuiteRoot] IFileSystemDirectory suiteRoot, IFsprojBuilderFactory builderFactory)
            : base(suiteRoot)
        {
            this.builderFactory = builderFactory;
        }

        protected override string SourceSetName
        {
            get { return "fs"; }
        }

        protected override string ProjectFileExtension
        {
            get { return ".fsproj"; }
        }

        public override string ProjectTypeGuid
        {
            get { return "{F2A71F9B-5D33-465A-A702-920D77279786}"; }
        }

        public override IBuilder CreateBuilder(Project project)
        {
            return builderFactory.CreateFsprojBuilder(project);
        }
    }
}