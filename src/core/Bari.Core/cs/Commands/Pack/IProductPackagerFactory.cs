using Bari.Core.Model;

namespace Bari.Core.Commands.Pack
{
    /// <summary>
    /// Factory interface for <see cref="IProductPackager"/> implementations
    /// </summary>
    public interface IProductPackagerFactory
    {
        IProductPackager CreateProductPackager(PackagerId type);
    }
}