using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Generic.Graph;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.Build
{
    public class SolutionBuildContext: IBuildContext
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (SolutionBuildContext));

        private readonly IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory;
        private readonly IBuildContext baseContext;
        private readonly SlnBuilder solutionBuilder;
        private readonly IDictionary<IBuilder, IBuilder> referenceMappings = new Dictionary<IBuilder, IBuilder>();

        public SolutionBuildContext(IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory, IBuildContext baseContext, SlnBuilder solutionBuilder)
        {
            this.baseContext = baseContext;
            this.solutionBuilder = solutionBuilder;
            this.inSolutionReferenceBuilderFactory = inSolutionReferenceBuilderFactory;
        }

        public void AddBuilder(IBuilder builder, IEnumerable<IBuilder> prerequisites)
        {
            IBuilder finalBuilder = builder;
            IEnumerable<IBuilder> finalPrerequisites = prerequisites.Select(ResolveBuilder);

            var moduleReferenceBuilder = builder as ModuleReferenceBuilder;
            if (moduleReferenceBuilder != null)
            {
                if (moduleReferenceBuilder.Reference.Type == ReferenceType.Build &&
                    solutionBuilder.Projects.Contains(moduleReferenceBuilder.ReferencedProject))
                {
                    log.DebugFormat("Transforming module reference builder {0}", moduleReferenceBuilder);

                    finalBuilder = ConvertToInSolutionReference(moduleReferenceBuilder, moduleReferenceBuilder.ReferencedProject);
                    finalPrerequisites = new IBuilder[0];
                }
            }
            else
            {
                var suiteReferenceBuilder = builder as SuiteReferenceBuilder;
                if (suiteReferenceBuilder != null)
                {
                    if (suiteReferenceBuilder.Reference.Type == ReferenceType.Build &&
                        solutionBuilder.Projects.Contains(suiteReferenceBuilder.ReferencedProject))
                    {
                        log.DebugFormat("Transforming module reference builder {0}", suiteReferenceBuilder);

                        finalBuilder = ConvertToInSolutionReference(suiteReferenceBuilder,
                            suiteReferenceBuilder.ReferencedProject);
                        finalPrerequisites = new IBuilder[0];
                    }
                }
            }

            baseContext.AddBuilder(finalBuilder, finalPrerequisites);
        }

        private IBuilder ConvertToInSolutionReference(IReferenceBuilder referenceBuilder, Project referencedProject)
        {
            if (!referenceMappings.ContainsKey(referenceBuilder))
            {
                var inSolutionBuilder = inSolutionReferenceBuilderFactory.CreateInSolutionReferenceBuilder(referencedProject);
                inSolutionBuilder.Reference = referenceBuilder.Reference;

                referenceMappings.Add(referenceBuilder, inSolutionBuilder);
                return inSolutionBuilder;
            }
            else
            {
                return referenceMappings[referenceBuilder];
            }
        }

        public void AddTransformation(Func<ISet<IDirectedGraphEdge<IBuilder>>, bool> transformation)
        {
            baseContext.AddTransformation(transformation);
        }

        public ISet<TargetRelativePath> Run(IBuilder rootBuilder = null)
        {
            return baseContext.Run(ResolveBuilder(rootBuilder));
        }

        public ISet<TargetRelativePath> GetResults(IBuilder builder)
        {
            return baseContext.GetResults(ResolveBuilder(builder));
        }

        public IEnumerable<IBuilder> GetDependencies(IBuilder builder)
        {
            return baseContext.GetDependencies(ResolveBuilder(builder));
        }

        private IBuilder ResolveBuilder(IBuilder builder)
        {
            IBuilder mappedBuilder;
            if (referenceMappings.TryGetValue(builder, out mappedBuilder))
                return mappedBuilder;
            else
                return builder;
        }

        public void Dump(Stream builderGraphStream, IBuilder rootBuilder)
        {
            baseContext.Dump(builderGraphStream, rootBuilder);
        }

        public bool Contains(IBuilder builder)
        {
            return baseContext.Contains(ResolveBuilder(builder));
        }
    }
}