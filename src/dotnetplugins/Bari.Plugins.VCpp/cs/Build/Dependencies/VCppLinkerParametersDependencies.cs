using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Model;

namespace Bari.Plugins.VCpp.Build.Dependencies
{
    public class VCppLinkerParametersDependencies : InheritableProjectParametersDependencies<VCppProjectLinkerParameters, VCppProjectLinkerParametersDef>
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