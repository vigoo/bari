using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Bari.Core.Model.Discovery
{
    /// <summary>
    /// Runs a set of suite explorers to add each discovery to a suite model
    /// </summary>
    public class ExplorerRunner
    {
        private readonly IEnumerable<ISuiteExplorer> explorers;

        /// <summary>
        /// Constructs the runner with the set of available explorers
        /// </summary>
        /// <param name="explorers">The set of explorers to be used</param>
        public ExplorerRunner(IEnumerable<ISuiteExplorer> explorers)
        {
            Contract.Requires(explorers != null);
            Contract.Requires(Contract.ForAll(explorers, explorer => explorer != null));

            this.explorers = explorers;
        }

        /// <summary>
        /// Runs all the available explorers to add their discoveris
        /// to the suite model.
        /// </summary>
        /// <param name="suite">The suite model to be extended.</param>
        public void RunAll(Suite suite)
        {
            Contract.Requires(suite != null);

            foreach (var suiteExplorer in explorers)
            {
                suiteExplorer.ExtendWithDiscoveries(suite);
            }
        }
    }
}