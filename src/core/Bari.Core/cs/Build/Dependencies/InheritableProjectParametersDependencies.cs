using System;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;
using Bari.Core.UI;

namespace Bari.Core.Build.Dependencies
{
    public class InheritableProjectParametersDependencies<TParams, TDef>: DependenciesBase 
        where TDef : ProjectParametersPropertyDefs<TParams>, new() 
        where TParams : InheritableProjectParameters<TParams, TDef>
    {
        private readonly Project project;
        private readonly string blockName;
        private readonly InheritableProjectParameters<TParams, TDef> parameters;

        protected InheritableProjectParametersDependencies(Project project, string name)
        {
            this.project = project;
            blockName = name;
            parameters = project.GetParameters<InheritableProjectParameters<TParams, TDef>>(name);
        }


        protected override IDependencyFingerprint CreateFingerprint()
        {
            return new InheritablePropertiesFingerprint<TParams, TDef>(parameters);
        }

        public override void Dump(IUserOutput output)
        {
            output.Message(String.Format("Project {0}.{1}'s parameter block {2}", project.Module.Name, project.Name, blockName));
        }
    }
}