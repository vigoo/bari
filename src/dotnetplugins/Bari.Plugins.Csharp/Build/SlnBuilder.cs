using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Generic.Graph;
using Bari.Core.Model;
using Bari.Plugins.Csharp.VisualStudio;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Builder for generating Visual Studio solution files from a set of projects.
    /// </summary>
    public class SlnBuilder : IBuilder, IEquatable<SlnBuilder>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (SlnBuilder));

        private readonly IResolutionRoot root;
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IFileSystemDirectory targetDir;
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IList<Project> projects;
        private ISet<IBuilder> projectBuilders;
        private IDependencies projectDependencies;

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to use</param>
        /// <param name="projects">The projects to be included to the solution</param>
        /// <param name="suiteRoot">Suite's root directory </param>
        /// <param name="targetDir">The target directory where the sln file should be put</param>
        /// <param name="root">Interface for creating new builder instances</param>
        public SlnBuilder(IProjectGuidManagement projectGuidManagement, IEnumerable<Project> projects, [SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetDir, IResolutionRoot root)
        {
            Contract.Requires(projectGuidManagement != null);
            Contract.Requires(projects != null);
            Contract.Requires(targetDir != null);
            Contract.Requires(suiteRoot != null);
            Contract.Requires(root != null);

            this.projectGuidManagement = projectGuidManagement;
            this.suiteRoot = suiteRoot;
            this.projects = projects.ToList();
            this.targetDir = targetDir;
            this.root = root;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return projectDependencies; }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get
            {
                return MD5.Encode(string.Join(",",
                                   from project in projects
                                   let module = project.Module
                                   let fullName = module + "." + project.Name
                                   select fullName));
            }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            projectBuilders = new HashSet<IBuilder>(
                from project in projects
                select CreateProjectBuilder(context, project)
                    into builder
                    where builder != null
                    select builder
                );

            context.AddBuilder(this, projectBuilders);
            context.AddTransformation(TransformRedundantSolutionBuilds);

            if (projectBuilders.Count == 1)
            {
                projectDependencies = new SubtaskDependency(projectBuilders.First());
            }
            else
            {
                projectDependencies = new MultipleDependencies(
                    from builder in projectBuilders
                    select new SubtaskDependency(builder));
            }
        }

        private IBuilder CreateProjectBuilder(IBuildContext context, Project project)
        {            if (project.HasNonEmptySourceSet("cs"))
            {
                var childKernel = new ChildKernel(root);
                childKernel.Bind<Project>().ToConstant(project);
                childKernel.Rebind<IResolutionRoot>().ToConstant(childKernel);

                var csprojBuilder = childKernel.Get<CsprojBuilder>();
                csprojBuilder.AddToContext(context);
                return csprojBuilder;
            }
            else
            {
                return null;
            }
        } 

        private bool TransformRedundantSolutionBuilds(List<IDirectedGraphEdge<IBuilder>> builders)
        {
            var subNodes = builders.BuildNodes(this).DirectedBreadthFirstTraversal((builder, bs) => { bs.Add(builder); return bs; }, new List<IBuilder>());
            foreach (var builderNode in subNodes)
            {
                var moduleReferenceBuilder = builderNode as ModuleReferenceBuilder;
                if (moduleReferenceBuilder != null)
                {
                    if (projects.Contains(moduleReferenceBuilder.ReferencedProject))
                    {
                        log.DebugFormat("Transforming module reference builder {0}", moduleReferenceBuilder);

                        ConvertToInSolutionReference(builders, moduleReferenceBuilder, moduleReferenceBuilder.ReferencedProject);
                    }
                }

                var suiteReferenceBuilder = builderNode as SuiteReferenceBuilder;
                if (suiteReferenceBuilder != null)
                {
                    if (projects.Contains(suiteReferenceBuilder.ReferencedProject))
                    {
                        log.DebugFormat("Transforming module reference builder {0}", suiteReferenceBuilder);

                        ConvertToInSolutionReference(builders, suiteReferenceBuilder, suiteReferenceBuilder.ReferencedProject);
                    }
                }
            }

            return true;
        }

        private void ConvertToInSolutionReference(List<IDirectedGraphEdge<IBuilder>> builders, IReferenceBuilder moduleReferenceBuilder, Project referencedProject)
        {
            // finding edges X -> RB 
            var edges = builders.Where(edge => Equals(edge.Target, moduleReferenceBuilder)).ToList();

            // removing these edges
            foreach (var edge in edges)
                builders.Remove(edge);
            var sourceNodes = edges.Select(edge => edge.Source);

            // creating new, in-solution reference builder node (ISB)
            var inSolutionBuilder = root.Get<InSolutionReferenceBuilder>(
                new ConstructorArgument("project", referencedProject));
            inSolutionBuilder.Reference = moduleReferenceBuilder.Reference;

            // creating new edges: X -> ISB                        
            builders.AddRange(
                sourceNodes.Select(
                    sourceNode => new SimpleDirectedGraphEdge<IBuilder>(sourceNode, inSolutionBuilder)));

            // removing every edge containing RB
            var edgesToRemove = builders.Where(edge => Equals(edge.Target, moduleReferenceBuilder) || Equals(edge.Source, moduleReferenceBuilder)).ToList();
            foreach (var edge in edgesToRemove)
                builders.Remove(edge);
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            string slnPath = Uid + ".sln";

            using (var sln = targetDir.CreateTextFile(slnPath))
            {
                var generator = new SlnGenerator(projectGuidManagement, projects, sln, suiteRoot, targetDir);
                generator.Generate();
            }

            return new HashSet<TargetRelativePath> { new TargetRelativePath(slnPath) };
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("[{0}.sln]", Uid);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(SlnBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return projects.SequenceEqual(other.projects);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SlnBuilder) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (projects != null ? projects.GetHashCode() : 0);
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(SlnBuilder left, SlnBuilder right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(SlnBuilder left, SlnBuilder right)
        {
            return !Equals(left, right);
        }
    }
}