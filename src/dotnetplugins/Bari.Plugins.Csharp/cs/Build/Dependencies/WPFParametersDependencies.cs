using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.Build.Dependencies
{
    public class WPFParametersDependencies : ProjectParametersDependencies
    {
        WPFParametersDependencies(Project project) : base(project, "wpf")
        {
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("wpf"))
                target.Add(new WPFParametersDependencies(project));
        }
    }
}