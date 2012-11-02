using System;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Exceptions;
using Bari.Core.Model;
using Ninject.Extensions.ChildKernel;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Factory class for the <see cref="CsprojBuilder"/> builder, creating additional builders for resolving
    /// project references with the correct dependency information.
    /// </summary>
    public class CsprojBuilderFactory
    {
        private readonly IResolutionRoot root;

        /// <summary>
        /// Constructs the factory
        /// </summary>
        /// <param name="root">Interface for creating new objects</param>
        public CsprojBuilderFactory(IResolutionRoot root)
        {
            this.root = root;
        }

        /// <summary>
        /// Adds a csproj builder to the context with all the necessary dependencies
        /// </summary>
        /// <param name="context">The build context to extend</param>
        /// <param name="project">Project to build project file for</param>
        /// <returns>Returns the created <see cref="CsprojBuilder"/> object.</returns>
        public IBuilder AddToContext(IBuildContext context, Project project)
        {
            var refBuilders = project.References.Select(CreateReferenceBuilder).ToList();

            var childKernel = new ChildKernel(root);
            childKernel.Bind<Project>().ToConstant(project);
            var csprojBuilder = childKernel.GetBuilder<CsprojBuilder>(
                new ConstructorArgument("referenceBuilders", refBuilders));

            foreach (var refBuilder in refBuilders)            
                context.AddBuilder(refBuilder, new IBuilder[0]);

            context.AddBuilder(csprojBuilder, refBuilders);

            return csprojBuilder;
        }

        private IBuilder CreateReferenceBuilder(Reference reference)
        {
            var builder = root.GetReferenceBuilder<IReferenceBuilder>(reference);
            if (builder != null)
                return builder;
            else
                throw new InvalidReferenceTypeException(reference.Uri.Scheme);
        }
    }
}