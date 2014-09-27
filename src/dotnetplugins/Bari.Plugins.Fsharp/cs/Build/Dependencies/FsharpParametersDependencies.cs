using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;

namespace Bari.Plugins.Fsharp.Build.Dependencies
{
    public class FsharpParametersDependencies : ProjectParametersDependencies
    {
        FsharpParametersDependencies(Project project)
            : base(project, "fsharp")
        {
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("fsharp"))
                target.Add(new FsharpParametersDependencies(project));
        }
    }
}