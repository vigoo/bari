using System.Collections.Generic;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Test
{
    /// <summary>
    /// General interface for test runners
    /// </summary>
    public interface ITestRunner
    {
        /// <summary>
        /// Runs the tests for the given test projects (<see cref="TestProject"/>)
        /// </summary>
        /// <param name="projects">Test projects to be ran</param>
        /// <param name="buildOutputs">Build outputs for these test projects</param>
        void Run(IEnumerable<TestProject> projects, IEnumerable<TargetRelativePath> buildOutputs);
    }
}