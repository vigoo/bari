using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Build.Dependencies
{
    public class VCppLinkerParametersDependencies: ProjectParametersDependencies
    {
        VCppLinkerParametersDependencies(Project project)
            : base(project, "cpp-linker")
        {            
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("cpp-linker"))
                target.Add(new VCppLinkerParametersDependencies(project));
        }
    }
}