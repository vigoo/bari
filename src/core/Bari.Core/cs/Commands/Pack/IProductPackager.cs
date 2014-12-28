using System.Collections.Generic;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Pack
{
    /// <summary>
    /// Packager/publisher interface for products
    /// </summary>
    public interface IProductPackager
    {
        /// <summary>
        /// Creates a product package
        /// </summary>
        /// <param name="product">Product to be packaged</param>
        /// <param name="outputs">Build outputs to pack</param>
        void Pack(Product product, ISet<TargetRelativePath> outputs);
        
        /// <summary>
        /// Publishes a packed product
        /// </summary>
        /// <param name="product">Produc to be published</param>
        void Publish(Product product);
    }
}