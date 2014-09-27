using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Build.Dependencies
{
    public class VCppATLParametersDependencies: ProjectParametersDependencies
    {
        VCppATLParametersDependencies(Project project)
            : base(project, "atl")
        {            
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("atl"))
                target.Add(new VCppATLParametersDependencies(project));
        }         
    }
}