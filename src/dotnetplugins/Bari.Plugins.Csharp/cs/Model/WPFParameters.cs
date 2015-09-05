using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.Csharp.Model
{
    public class WPFParametersDef : ProjectParametersPropertyDefs<WPFParameters>
    {
        public WPFParametersDef()
        {
            Define<string>("ApplicationDefinition");
        }

        public override WPFParameters CreateDefault(Suite suite, WPFParameters parent)
        {
            return new WPFParameters(parent);
        }
    }

    public class WPFParameters : InheritableProjectParameters<WPFParameters, WPFParametersDef>
    {
        public WPFParameters(WPFParameters parent = null) 
            : base(parent)
        {
        }

        public string ApplicationDefinition 
         {
             get { return Get<string>("ApplicationDefinition"); }
             set { Set("ApplicationDefinition", value); }
         }
    }
}