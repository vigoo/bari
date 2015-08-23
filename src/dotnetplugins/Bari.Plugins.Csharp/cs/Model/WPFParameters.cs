using Bari.Core.Model.Parameters;

namespace Bari.Plugins.Csharp.Model
{
    public class WPFParametersDef : ProjectParametersPropertyDefs
    {
        public WPFParametersDef()
        {
            Define<string>("ApplicationDefinition");
        }
    }

    public class WPFParameters : InheritableProjectParameters<WPFParametersDef>
    {
         public string ApplicationDefinition 
         {
             get { return Get<string>("ApplicationDefinition"); }
             set { Set("ApplicationDefinition", value); }
         }
    }
}