using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Build;
using Bari.Plugins.VsCore.Build;
using Bari.Plugins.VsCore.VisualStudio;

namespace Bari.Plugins.VCpp.VisualStudio
{
    /// <summary>
    /// The <see cref="ISlnProject"/> implementation for Visual C++ projects
    /// </summary>
    public class CppSlnProject: SlnProjectBase
    {
        private readonly IVcxprojBuilderFactory builderFactory;

        public CppSlnProject([SuiteRoot] IFileSystemDirectory suiteRoot, IVcxprojBuilderFactory builderFactory) 
            : base(suiteRoot)
        {
            this.builderFactory = builderFactory;
        }

        protected override string SourceSetName
        {
            get { return "cpp"; }
        }

        protected override string ProjectFileExtension
        {
            get { return ".vcxproj"; }
        }

        public override string ProjectTypeGuid
        {
            get { return "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}"; }
        }

        public override ISlnProjectBuilder CreateBuilder(Project project)
        {
            return builderFactory.CreateVcxprojBuilder(project);
        }
    }
}