using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bari.Core.Model;

namespace Bari.Core.Build.Dependencies
{
    public abstract class ProjectParametersDependencies : IDependencies
    {
        private readonly IProjectParameters parameters;

        protected ProjectParametersDependencies(Project project, string name)
        {
            parameters = project.GetParameters<IProjectParameters>(name);
        }

        public IDependencyFingerprint CreateFingerprint()
        {
            return new ObjectPropertiesFingerprint(
                parameters,
                RelevantProperties);
        }

        protected IEnumerable<string> RelevantProperties
        {
            get
            {
                return parameters
                    .GetType()
                    .GetProperties(BindingFlags.Public)
                    .Select(pi => pi.Name);
            }
        }
    }
}