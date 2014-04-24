using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using QuickGraph;

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

        private IBuilder ConvertBuilder(IBuilder builder)
        {
            var moduleReferenceBuilder = builder as ModuleReferenceBuilder;
            if (moduleReferenceBuilder != null)
            {
                if (moduleReferenceBuilder.Reference.Type == ReferenceType.Build &&
                    solutionBuilder.Projects.Contains(moduleReferenceBuilder.ReferencedProject))
                {
                    log.DebugFormat("Transforming module reference builder {0}", moduleReferenceBuilder);

                    return ConvertToInSolutionReference(moduleReferenceBuilder, moduleReferenceBuilder.ReferencedProject);
                    
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

                        return ConvertToInSolutionReference(suiteReferenceBuilder, suiteReferenceBuilder.ReferencedProject);
                    }
                }
            }

            return null;
        }

        public void AddBuilder(IBuilder builder, IEnumerable<IBuilder> prerequisites)
        {
            IBuilder finalBuilder = ConvertBuilder(builder);            
            IEnumerable<IBuilder> finalPrerequisites = null;
            if (finalBuilder != null)
                finalPrerequisites = new IBuilder[0];
            else
                finalBuilder = builder;

            if (finalPrerequisites == null)
                finalPrerequisites = prerequisites.Select(ResolveBuilder);

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

        public void AddTransformation(Func<ISet<EquatableEdge<IBuilder>>, bool> transformation)
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
            var resolved = ResolveBuilder(builder);
            var converted = ConvertBuilder(resolved);

            return baseContext.GetDependencies(converted ?? resolved);
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
            var resolved = ResolveBuilder(builder);
            var converted = ConvertBuilder(resolved);

            return baseContext.Contains(converted ?? resolved);
        }
    }
}