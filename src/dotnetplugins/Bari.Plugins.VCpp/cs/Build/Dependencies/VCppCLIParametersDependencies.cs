using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Model;

namespace Bari.Plugins.VCpp.Build.Dependencies
{
    public class VCppCLIParametersDependencies : InheritableProjectParametersDependencies<VCppProjectCLIParameters, VCppProjectCLIParametersDef>
    {
        VCppCLIParametersDependencies(Project project)
            : base(project, "cli")
        {            
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("cli"))
                target.Add(new VCppCLIParametersDependencies(project));
        }
    }
}