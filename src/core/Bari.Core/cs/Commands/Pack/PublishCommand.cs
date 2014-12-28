using System;
using Bari.Core.Exceptions;
using Bari.Core.Model;

namespace Bari.Core.Commands.Pack
{
    /// <summary>
    /// Implements the `publish` command which publishes the `pack` commands output
    /// </summary>
    public class PublishCommand: ICommand
    {
        private readonly IProductPackagerFactory productPackagerFactory;

        public PublishCommand(IProductPackagerFactory productPackagerFactory)
        {
            this.productPackagerFactory = productPackagerFactory;
        }

        public string Name
        {
            get { return "publish"; }
        }

        public string Description
        {
            get { return "publishes a packed product"; }
        }

        public string Help
        {
            get
            {
                return
@"=Publish command=

Publishes a product package *previously created* with the `pack` command.
Example: `bari publish ExampleProduct`

The product must be built and packaged separately!
";
            }
        }

        public bool NeedsExplicitTargetGoal
        {
            get { return true; }
        }

        public bool Run(Suite suite, string[] parameters)
        {
            if (parameters.Length == 1)
                PublishProduct(suite, parameters[0]);
            else
                throw new InvalidCommandParameterException("publish",
                                                           "The 'publish' command must be called with a product name!");

            return true;
        }

        private void PublishProduct(Suite suite, string productName)
        {
            if (suite.HasProduct(productName))
            {
                var product = suite.GetProduct(productName);
                if (product.Packager != null)
                {
                    var packager = productPackagerFactory.CreateProductPackager(product.Packager.PackagerType);
                    packager.Publish(product);
                }
                else
                {
                    throw new InvalidCommandParameterException("publish", "The product has no packager definition!");
                }
            }
            else
            {
                throw new InvalidCommandParameterException("publish", String.Format("Product {0} is not defined!", productName));
            }
        }
    }
}