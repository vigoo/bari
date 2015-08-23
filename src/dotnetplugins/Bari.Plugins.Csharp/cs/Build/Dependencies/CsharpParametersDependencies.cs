using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;
using Bari.Plugins.Csharp.Model;

namespace Bari.Plugins.Csharp.Build.Dependencies
{
    public class CsharpParametersDependencies : InheritableProjectParametersDependencies<CsharpProjectParametersDef>
    {
        CsharpParametersDependencies(Project project) : base(project, "csharp")
        {            
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("csharp"))
                target.Add(new CsharpParametersDependencies(project));
        }
    }
}