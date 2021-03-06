﻿using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Model;

namespace Bari.Plugins.VCpp.Build.Dependencies
{
    public class VCppMIDLParametersDependencies : InheritableProjectParametersDependencies<VCppProjectMIDLParameters, VCppProjectMIDLParametersDef>
    {
        VCppMIDLParametersDependencies(Project project)
            : base(project, "midl")
        {            
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("midl"))
                target.Add(new VCppMIDLParametersDependencies(project));
        }
    }
}