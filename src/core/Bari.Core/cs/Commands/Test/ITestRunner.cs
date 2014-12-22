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
        /// Gets the name which can be used to refere to this test runner in the suite configuration
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Runs the tests for the given test projects (<see cref="TestProject"/>)
        /// </summary>
        /// <param name="projects">Test projects to be ran</param>
        /// <param name="buildOutputs">Build outputs for these test projects</param>
        /// <returns>Returns <c>true</c> if all the tests has passed</returns>
        bool Run(IEnumerable<TestProject> projects, IEnumerable<TargetRelativePath> buildOutputs);
    }
}