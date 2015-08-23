using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;
using Bari.Core.UI;
using System;

namespace Bari.Core.Build.Dependencies
{
    public abstract class ProjectParametersDependencies : DependenciesBase
    {
        private readonly Project project;
        private readonly string blockName;
        private readonly IProjectParameters parameters;

        protected ProjectParametersDependencies(Project project, string name)
        {
            this.project = project;
            blockName = name;
            parameters = project.GetParameters<IProjectParameters>(name);
        }

        protected override IDependencyFingerprint CreateFingerprint()
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
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(pi => pi.Name);
            }
        }

        public override void Dump(IUserOutput output)
        {
            output.Message(String.Format("Project {0}.{1}'s parameter block {2}", project.Module.Name, project.Name, blockName));
        }
    }
}