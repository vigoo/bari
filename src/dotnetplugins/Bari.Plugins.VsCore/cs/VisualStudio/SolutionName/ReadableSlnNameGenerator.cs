using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio.SolutionName
{
    /// <summary>
    /// Advanced SLN name generator which tries to give human readable names to special cases of set of projects.
    /// </summary>
    public class ReadableSlnNameGenerator : ISlnNameGenerator
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReadableSlnNameGenerator));

        private const int MaxModuleCount = 4;

        private readonly ISlnNameGenerator fallbackGenerator;
        private readonly ISuiteContentsAnalyzer analyzer;

        /// <summary>
        /// Creates the name generator
        /// </summary>
        /// <param name="fallbackGenerator">Fallback name generator to be used when the set of projects is not a special case
        /// handled by this name generator.</param>
        /// <param name="analyzer">Solution analyzer to be used to find special cases.</param>
        public ReadableSlnNameGenerator(ISlnNameGenerator fallbackGenerator, ISuiteContentsAnalyzer analyzer)
        {
            Contract.Requires(fallbackGenerator != null);
            Contract.Requires(analyzer != null);

            this.fallbackGenerator = fallbackGenerator;
            this.analyzer = analyzer;
        }

        /// <summary>
        /// Generates a file name for a VS solution file which will contain the given set of projects.
        /// </summary>
        /// <param name="projects">Set of projects to be included in the SLN file</param>
        /// <returns>Returns a valid file name without extension</returns>
        public string GetName(IEnumerable<Project> projects)
        {
            var prjs = projects.ToList();

            if (prjs.Count == 1)
            {
                // Single project
                return GetNameForSingleProject(prjs[0]);
            }
            else
            {
                var matches = analyzer.GetCoveredModules(prjs).ToList();
                if (matches.Count > 0 && matches.All(m => !m.Partial))
                {
                    bool? allTests = AllHasTests(matches);

                    if (allTests.HasValue)
                    {
                        var productName = analyzer.GetProductName(matches.Select(m => m.Module));
                        if (productName != null)
                        {
                            // this covers a product
                            return GetNameBasedOnProduct(productName, allTests.Value);
                        }
                        else if (matches.Count <= MaxModuleCount)
                        {
                            return GetNameBasedOnMultipleModules(matches.Select(m => m.Module), allTests.Value);
                        }
                        // otherwise: many modules -> fallback
                    }
                    // otherwise: contains modules both with tests included and without included -> fallback
                }
                // otherwise: there are partial matches -> fallback

                return fallbackGenerator.GetName(prjs);
            }
        }

        private string GetNameBasedOnMultipleModules(IEnumerable<Module> modules, bool allHasTests)
        {
            var result = string.Join("_", modules.Select(m => m.Name)) + GetPostfix(allHasTests);
            log.DebugFormat("Returning sln name based on multiple modules: {0}", result);
            return result;
        }

        private string GetNameBasedOnProduct(string productName, bool allHasTests)
        {
            var result = productName + GetPostfix(allHasTests);
            log.DebugFormat("Returning sln name based on product: {0}", result);
            return result;
        }

        private string GetNameForSingleProject(Project project)
        {
            var result = (project.Module + "." + project.Name).Replace(' ', '_');
            log.DebugFormat("Returning sln name for a single project: {0}", result);
            return result;
        }

        private string GetPostfix(bool allHasTests)
        {
            if (allHasTests)
                return "-withtests";
            else
                return string.Empty;
        }

        private bool? AllHasTests(IEnumerable<ModuleMatch> matches)
        {
            bool allTrue = true;
            bool allFalse = true;
            bool hadAnyTests = false;
            foreach (var moduleMatch in matches)
            {
                if (moduleMatch.Module.TestProjects.Any())
                {
                    if (moduleMatch.IncludingTests)
                        allFalse = false;
                    else
                        allTrue = false;

                    hadAnyTests = true;
                }
            }

            if (allTrue && hadAnyTests)
                return true;
            else if (allFalse || !hadAnyTests)
                return false;
            else
                return null;
        }
    }
}