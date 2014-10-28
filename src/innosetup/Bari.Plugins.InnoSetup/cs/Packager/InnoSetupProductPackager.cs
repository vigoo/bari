using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Commands.Pack;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.InnoSetup.Tools;

namespace Bari.Plugins.InnoSetup.Packager
{
    public class InnoSetupProductPackager: IProductPackager
    {
        private readonly IInnoSetupCompiler compiler;
        private readonly Suite suite;

        public InnoSetupProductPackager(IInnoSetupCompiler compiler, Suite suite)
        {
            Contract.Requires(compiler != null);
            Contract.Requires(suite != null);

            this.compiler = compiler;
            this.suite = suite;
        }

        public void Pack(Product product, ISet<TargetRelativePath> outputs)
        {
            var parameters = (InnoSetupPackagerParameters) product.Packager.Parameters;

            compiler.Compile(
                parameters.ScriptPath, 
                new TargetRelativePath("", String.Format("{0}-{1}.exe", product.Name, suite.Version)), 
                suite.Version, suite.ActiveGoal);
        }
    }
}