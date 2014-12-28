using System.Collections.Generic;
using Bari.Core.Commands.Pack;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Nuget.Tools;

namespace Bari.Plugins.Nuget.Packager
{
    public class NugetProductPackager: IProductPackager
    {
        private readonly INuGet nuget;
        private readonly Suite suite;
        private readonly IFileSystemDirectory targetRoot;

        public NugetProductPackager(INuGet nuget, Suite suite, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.nuget = nuget;
            this.suite = suite;
            this.targetRoot = targetRoot;
        }

        public void Pack(Product product, ISet<TargetRelativePath> outputs)
        {
            var parameters = (NugetPackagerParameters)product.Packager.Parameters;
            var nuspec = new NuSpecGenerator(parameters, outputs, suite);

            nuget.CreatePackage(targetRoot, product.Name, nuspec.ToXml());
        }

        public void Publish(Product product)
        {
            throw new System.NotImplementedException();
        }
    }
}