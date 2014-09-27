using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Build.Dependencies
{
    public class VCppCompilerParametersDependencies: ProjectParametersDependencies
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