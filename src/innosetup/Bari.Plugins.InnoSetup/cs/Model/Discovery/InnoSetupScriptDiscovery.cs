using System;
using System.IO;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.Model.Discovery;
using Bari.Plugins.InnoSetup.Packager;

namespace Bari.Plugins.InnoSetup.Model.Discovery
{
    /// <summary>
    /// Finds InnoSetup scripts in the <c>setup</c> directory with <c>productname.iss</c> names
    /// </summary>
    public class InnoSetupScriptDiscovery : ISuiteExplorer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InnoSetupScriptDiscovery));

        public void ExtendWithDiscoveries(Suite suite)
        {
            var setupDir = suite.SuiteRoot.GetChildDirectory("setup");
            if (setupDir != null)
            {
                foreach (var product in suite.Products)
                {
                    var scriptName = String.Format("{0}.iss", product.Name);

                    if (setupDir.Exists(scriptName))
                        SetupInnoSetupPackagerFor(product, new SuiteRelativePath(Path.Combine("setup", scriptName)));
                }
            }
        }

        private void SetupInnoSetupPackagerFor(Product product, SuiteRelativePath scriptPath)
        {
            log.InfoFormat("Found InnoSetup script for product {0} at {1}", product.Name, scriptPath);

            if (product.Packager == null)
            {
                product.Packager = new PackagerDefinition(new PackagerId("innosetup"),
                    new InnoSetupPackagerParameters {ScriptPath = scriptPath});
            }
            else
            {
                if (product.Packager.PackagerType == new PackagerId("innosetup"))
                {
                    var parameters = product.Packager.Parameters as InnoSetupPackagerParameters;
                    if (parameters != null)
                        parameters.ScriptPath = scriptPath;
                }
            }
        }
    }
}