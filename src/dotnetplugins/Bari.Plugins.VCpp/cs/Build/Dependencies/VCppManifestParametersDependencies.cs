using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Model;

namespace Bari.Plugins.VCpp.Build.Dependencies
{
    public class VCppManifestParametersDependencies : InheritableProjectParametersDependencies<VCppProjectManifestParameters, VCppProjectManifestParametersDef>
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