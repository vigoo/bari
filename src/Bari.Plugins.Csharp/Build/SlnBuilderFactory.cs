using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Model;
using Ninject.Extensions.ChildKernel;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Factory class for <see cref="SlnBuilder"/> builder, creating the dependent <see cref="CsprojBuilder"/> objects
    /// and adding them to a <see cref="IBuildContext"/> with the correct dependency information.
    /// </summary>
    public class SlnBuilderFactory
    {
        private readonly IResolutionRoot root;

        /// <summary>
        /// Constructs the factory
        /// </summary>
        /// <param name="root">Interface for creating new objects</param>
        public SlnBuilderFactory(IResolutionRoot root)
        {
            this.root = root;
        }

        /// <summary>
        /// Creates a solution builder and adds it to the given build context together with its
        /// dependencies.
        /// </summary>
        /// <param name="context">The current build context</param>
        /// <param name="projects">Set of projects to be included in the solution file</param>
        /// <returns>Returns the <see cref="SlnBuilder"/> object.</returns>
        public IBuilder AddToContext(IBuildContext context, IEnumerable<Project> projects)
        {
            var projectBuilders = new HashSet<IBuilder>(
                from project in projects
                select CreateProjectBuilder(project)
                    into builder
                    where builder != null
                    select builder
                );

            var slnBuilder = root.GetBuilder<SlnBuilder>(
                new ConstructorArgument("projects", projects),
                new ConstructorArgument("projectBuilders", projectBuilders));

            foreach (var projectBuilder in projectBuilders)            
                context.AddBuilder(projectBuilder, new IBuilder[0]);
            
            context.AddBuilder(slnBuilder, projectBuilders);

            return slnBuilder;
        }

        private IBuilder CreateProjectBuilder(Project project)
        {
            if (project.HasNonEmptySourceSet("cs"))
            {
                var childKernel = new ChildKernel(root);
                childKernel.Bind<Project>().ToConstant(project);

                return childKernel.GetBuilder<CsprojBuilder>();
            }
            else
            {
                return null;
            }
        } 
    }
}