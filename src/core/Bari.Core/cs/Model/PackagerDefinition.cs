using Bari.Core.Model.Parameters;

namespace Bari.Core.Model
{
    public class PackagerDefinition
    {
        private readonly PackagerId packagerType;
        private readonly IPackagerParameters parameters;

        public PackagerId PackagerType
        {
            get { return packagerType; }
        }

        public IPackagerParameters Parameters
        {
            get { return parameters; }
        }

        public PackagerDefinition(PackagerId packagerType, IPackagerParameters parameters)
        {
            this.packagerType = packagerType;
            this.parameters = parameters;
        }
    }
}