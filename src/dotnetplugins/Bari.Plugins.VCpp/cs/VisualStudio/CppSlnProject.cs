using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio;

namespace Bari.Plugins.VCpp.VisualStudio
{
    /// <summary>
    /// The <see cref="ISlnProject"/> implementation for Visual C++ projects
    /// </summary>
    public class CppSlnProject: SlnProjectBase
    {
        public CppSlnProject([SuiteRoot] IFileSystemDirectory suiteRoot) 
            : base(suiteRoot)
        {
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

        public override IBuilder CreateBuilder(Project project)
        {
            throw new System.NotImplementedException();
        }
    }
}