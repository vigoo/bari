using System;
using System.Linq;
using Bari.Core.Commands;
using Bari.Core.Model;
using Bari.Core.UI;
using Newtonsoft.Json;

namespace Bari.Plugins.AddonSupport.Commands 
{
    public class AddonInfoCommand : ICommand
    {
        private readonly IUserOutput output;
        
        public string Description
        {
            get
            {
                return "provides info about the suite in JSON format for editor addons"; 
            }
        }

        public string Help
        {
            get
            {
                return
@"= addon-info command =

Prints a JSON describing the possible *goals* and *targets* of the suite.
";
            }
        }

        public string Name
        {
            get
            {
                return "addon-info";
            }
        }

        public bool NeedsExplicitTargetGoal
        {
            get
            {
                return false;
            }
        }
        
        public AddonInfoCommand(IUserOutput output)
        {
            this.output = output;
        }

        public bool Run(Suite suite, string[] parameters)
        {
            var data = new {
                products = suite.Products.Select(p => p.Name).ToArray(),
                goals = suite.Goals.Select(g => g.Name).ToArray()                
            };
            output.Message(JsonConvert.SerializeObject(data));
            
            return true;
        }
    }
}