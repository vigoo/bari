using System;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Model;
using Ninject.Extensions.ChildKernel;
using Ninject.Syntax;

namespace Bari.Plugins.Csharp.Build
{
    public class CsprojBuilderFactory
    {
        private readonly IResolutionRoot root;

        public CsprojBuilderFactory(IResolutionRoot root)
        {
            this.root = root;
        }

        public IBuilder AddToContext(IBuildContext context, Project project)
        {
            var refBuilders = project.References.Select(CreateReferenceBuilder).ToList();

            var childKernel = new ChildKernel(root);
            childKernel.Bind<Project>().ToConstant(project);
            var csprojBuilder = childKernel.GetBuilder<CsprojBuilder>();

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
                throw new NotSupportedException(string.Format("Reference type {0} is not supported", reference.Uri.Scheme));
        }
    }
}