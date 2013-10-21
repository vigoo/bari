using System.Collections.Generic;
using System.Linq;

namespace Bari.Core.Build.Dependencies
{
    public static class MultipleDependenciesHelper
    {
        public static IDependencies CreateMultipleDependencies(ISet<IDependencies> subdeps)
        {
            if (subdeps.Count == 0)
                return new NoDependencies();
            else if (subdeps.Count == 1)
                return subdeps.First();
            else
                return new MultipleDependencies(subdeps);
        }

         public static IDependencies CreateMultipleDependencies(ISet<IBuilder> subtasks)
         {
             if (subtasks.Count == 0)
                 return new NoDependencies();
             else if (subtasks.Count == 1)
                 return new SubtaskDependency(subtasks.First());
             else
                 return new MultipleDependencies(subtasks.Select(subtask => new SubtaskDependency(subtask)));
         }
    }
}