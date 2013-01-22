using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;

namespace Bari.Core.Commands.Helper
{
    public class ProductTarget: CommandTarget
    {
        private readonly Product product;

        public ProductTarget(Product product)
        {
            this.product = product;
        }

        public override IEnumerable<Project> Projects
        {
            get
            {
                return from module in product.Modules
                       from project in module.Projects
                       select project;
            }
        }

        public override IEnumerable<Project> TestProjects
        {
            get
            {
                return from module in product.Modules
                       from project in module.TestProjects
                       select project;
            }
        }
    }
}