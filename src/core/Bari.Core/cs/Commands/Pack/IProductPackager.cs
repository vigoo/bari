using System.Collections.Generic;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Pack
{
    public interface IProductPackager
    {
        void Pack(Product product, ISet<TargetRelativePath> outputs);
    }
}