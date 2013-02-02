using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio.SolutionName
{
    /// <summary>
    /// Result class for <see cref="ISuiteContentsAnalyzer"/> implementations
    /// </summary>
    public class ModuleMatch
    {
        private readonly Module module;
        private readonly bool partial;
        private readonly bool includingTests;

        /// <summary>
        /// The matched module
        /// </summary>
        public Module Module
        {
            get { return module; }
        }

        /// <summary>
        /// If <c>true</c>, the module has been only partially matched
        /// </summary>
        public bool Partial
        {
            get { return partial; }
        }

        /// <summary>
        /// If <c>true</c>, the module's test projects were also matched
        /// </summary>
        public bool IncludingTests
        {
            get { return includingTests; }
        }

        /// <summary>
        /// Creates the result object
        /// </summary>
        /// <param name="module">Matched module</param>
        /// <param name="partial">Was it partially matched?</param>
        /// <param name="includingTests">Test projects are included?</param>
        public ModuleMatch(Module module, bool partial, bool includingTests)
        {
            this.module = module;
            this.partial = partial;
            this.includingTests = includingTests;
        }
    }
}