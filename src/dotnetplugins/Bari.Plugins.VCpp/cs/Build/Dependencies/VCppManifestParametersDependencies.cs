using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Build.Dependencies
{
    public class VCppManifestParametersDependencies: ProjectParametersDependencies
    {
        VCppManifestParametersDependencies(Project project)
            : base(project, "manifest")
        {            
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("manifest"))
                target.Add(new VCppManifestParametersDependencies(project));
        }
    }
}