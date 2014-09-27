using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;

namespace Bari.Plugins.Fsharp.Build.Dependencies
{
    public class FileOrderDependencies: ProjectParametersDependencies
    {
        FileOrderDependencies(Project project)
            : base(project, "order")
        {
        }

        public static void Add(Project project, ICollection<IDependencies> target)
        {
            if (project.HasParameters("order"))
                target.Add(new FileOrderDependencies(project));
        }
    }
}