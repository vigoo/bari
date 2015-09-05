using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Model;

namespace Bari.Plugins.VCpp.Build.Dependencies
{
    public class VCppCompilerParametersDependencies : InheritableProjectParametersDependencies<VCppProjectCompilerParameters, VCppProjectCompilerParametersDef>
    {
        VCppCompilerParametersDependencies(Project project)
            : base(project, "cpp-compiler")
        {            
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("cpp-compiler"))
                target.Add(new VCppCompilerParametersDependencies(project));
        }
    }
}