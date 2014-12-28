using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Commands.Pack;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.UI;
using Bari.Plugins.InnoSetup.Tools;

namespace Bari.Plugins.InnoSetup.Packager
{
    public class InnoSetupProductPackager: IProductPackager
    {
        private readonly IInnoSetupCompiler compiler;
        private readonly Suite suite;
        private readonly IUserOutput output;

        public InnoSetupProductPackager(IInnoSetupCompiler compiler, Suite suite, IUserOutput output)
        {
            Contract.Requires(compiler != null);
            Contract.Requires(suite != null);

            this.compiler = compiler;
            this.suite = suite;
            this.output = output;
        }

        public void Pack(Product product, ISet<TargetRelativePath> outputs)
        {
            var parameters = (InnoSetupPackagerParameters) product.Packager.Parameters;

            compiler.Compile(
                parameters.ScriptPath, 
                new TargetRelativePath("", String.Format("{0}-{1}", product.Name, suite.Version)), 
                suite.Version, suite.ActiveGoal);
        }

        public void Publish(Product product)
        {
            output.Error("InnoSetup packager does not support publishing.");
        }
    }
}
