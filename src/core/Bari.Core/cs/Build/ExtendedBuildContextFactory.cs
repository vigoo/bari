using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;
using QuickGraph;

namespace Bari.Core.Build
{
    public class ExtendedBuildContextFactory: IBuildContextFactory
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ExtendedBuildContextFactory));
        
        private readonly IBuildContextFactory wrappedFactory;
        
        public ExtendedBuildContextFactory(IBuildContextFactory wrappedFactory)
        {
            this.wrappedFactory = wrappedFactory;
        }

        public IBuildContext CreateBuildContext(Suite suite)
        {
            var context = wrappedFactory.CreateBuildContext(suite);
            if (suite != null)
            {
                // NOTE: suite can be null for plugin loader contexts, for example
                context.AddTransformation(graph => AddForcedOrdering(suite, graph));
            }
            return context;
        }
        
        
        private bool AddForcedOrdering(Suite suite, ISet<EquatableEdge<IBuilder>> graph)
        {
            var builders = graph.Select(edge => edge.Source).ToArray();
            
            foreach (var module in suite.Modules)
            {
                foreach (var project in module.Projects.Concat(module.TestProjects))
                {
                    var order = project.ProjectLevelForcedCompilationOrder.ToArray();
                    if (order.Length > 1)
                    {
                        for (int i = 0; i < order.Length - 1; i++)
                        {
                            var sourceBuilder = builders.FirstOrDefault(b => b.Name.MatchesProjectRelativeName(project, order[i]));
                            var targetBuilder = builders.FirstOrDefault(b => b.Name.MatchesProjectRelativeName(project, order[i+1]));
                            
                            if (targetBuilder != null && sourceBuilder != null)
                            {
                                log.DebugFormat("Adding forced dependency: {0} -> {1}", targetBuilder, sourceBuilder);
                                graph.Add(new EquatableEdge<IBuilder>(targetBuilder, sourceBuilder));
                                targetBuilder.AddPrerequisite(sourceBuilder);
                            }
                            else
                            {
                                if (sourceBuilder == null)
                                    log.ErrorFormat("Could not find builder for forced ordering in project {0}: {1}", project, order[i]);
                                if (targetBuilder == null)
                                    log.ErrorFormat("Could not find builder for forced ordering in project {0}: {1}", project, order[i+1]);
                            }
                        }
                    }
                }
            }
            
            return true;
        }
    }
}   