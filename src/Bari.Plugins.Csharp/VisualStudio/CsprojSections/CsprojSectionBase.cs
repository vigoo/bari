using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio.CsprojSections
{
    /// <summary>
    /// Base class for <see cref="ICsprojSection"/> implementations
    /// </summary>
    public abstract class CsprojSectionBase: ICsprojSection
    {
        private readonly Suite suite;

        /// <summary>
        /// Gets the active suite
        /// </summary>
        protected Suite Suite
        {
            get { return suite; }
        }

        /// <summary>
        /// Initializes the .csproj section writer
        /// </summary>
        /// <param name="suite">Active suite</param>
        protected CsprojSectionBase(Suite suite)
        {
            this.suite = suite;
        }

        /// <summary>
        /// Writes the section using an XML writer
        /// </summary>
        /// <param name="writer">XML writer to use</param>
        /// <param name="project">The project to generate .csproj for</param>
        /// <param name="context">Current .csproj generation context</param>
        public abstract void Write(XmlWriter writer, Project project, ICsprojGeneratorContext context);

        /// <summary>
        /// Converts a suite relative path to project relative
        /// </summary>
        /// <param name="project">Project to be relative to</param>
        /// <param name="suiteRelativePath">Suite relative path</param>
        /// <returns>Returns the path relative to the project's root directory</returns>
        protected string ToProjectRelativePath(Project project, string suiteRelativePath)
        {
            return suite.SuiteRoot.GetRelativePathFrom(project.RootDirectory, suiteRelativePath);
        }
    }
}