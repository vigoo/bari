using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// A <see cref="IProjectBuilderFactory"/> implementation that creates <see cref="ContentBuilder"/>
    /// instances for projects having a source set named <c>content</c>.
    /// </summary>
    public class ContentProjectBuilderFactory: IProjectBuilderFactory
    {
        private readonly ICoreBuilderFactory coreBuilderFactory;

        public ContentProjectBuilderFactory(ICoreBuilderFactory coreBuilderFactory)
        {
            this.coreBuilderFactory = coreBuilderFactory;
        }

        /// <summary>
        /// Creates a builder (<see cref="IBuilder"/>) which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="projects">Projects to be built</param>
        public IBuilder Create(IEnumerable<Project> projects)
        {
            IBuilder[] builders = projects.Where(prj => prj.HasNonEmptySourceSet("content"))
                                          .Select(prj => (IBuilder)coreBuilderFactory.CreateContentBuilder(prj)).ToArray();

            return coreBuilderFactory.Merge(builders);
        }
    }
}