using System;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;
using Bari.Core.UI;

namespace Bari.Core.Build.Dependencies
{
    public class InheritableProjectParametersDependencies<TDef>: DependenciesBase 
        where TDef : ProjectParametersPropertyDefs, new()
    {
        private readonly Project project;
        private readonly string blockName;
        private readonly InheritableProjectParameters<TDef> parameters;

        protected InheritableProjectParametersDependencies(Project project, string name)
        {
            this.project = project;
            blockName = name;
            parameters = project.GetParameters<InheritableProjectParameters<TDef>>(name);
        }


        protected override IDependencyFingerprint CreateFingerprint()
        {
            return new InheritablePropertiesFingerprint<TDef>(parameters);
        }

        public override void Dump(IUserOutput output)
        {
            output.Message(String.Format("Project {0}.{1}'s parameter block {2}", project.Module.Name, project.Name, blockName));
        }
    }
}