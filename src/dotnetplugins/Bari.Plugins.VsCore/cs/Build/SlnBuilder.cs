using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Generic.Graph;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.SolutionName;

namespace Bari.Plugins.VsCore.Build
{
    /// <summary>
    /// Builder for generating Visual Studio solution files from a set of projects.
    /// </summary>
    public class SlnBuilder : IBuilder, IEquatable<SlnBuilder>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (SlnBuilder));

        private readonly IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory;
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IProjectPlatformManagement projectPlatformManagement;
        private readonly IFileSystemDirectory targetDir;
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IList<Project> projects;
        private readonly ISlnNameGenerator slnNameGenerator;
        private readonly IDictionary<Project, ISet<Project>> inSolutionReferences;
        private readonly IEnumerable<ISlnProject> supportedSlnProjects;
        private ISet<IBuilder> projectBuilders;
        private IDependencies projectDependencies;

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to use</param>
        /// <param name="projectPlatformManagement">Interface for getting default project configuration names</param>
        /// <param name="supportedSlnProjects">Supported project types</param>
        /// <param name="projects">The projects to be included to the solution</param>
        /// <param name="suiteRoot">Suite's root directory </param>
        /// <param name="targetDir">The target directory where the sln file should be put</param>
        /// <param name="slnNameGenerator">Name generator implementation for the sln file </param>
        /// <param name="inSolutionReferenceBuilderFactory">Interface to create new in-solution reference builder instances</param>
        public SlnBuilder(IProjectGuidManagement projectGuidManagement, IProjectPlatformManagement projectPlatformManagement, IEnumerable<ISlnProject> supportedSlnProjects, IEnumerable<Project> projects, [SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetDir, ISlnNameGenerator slnNameGenerator, IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory)
        {
            Contract.Requires(projectGuidManagement != null);
            Contract.Requires(projectPlatformManagement != null);
            Contract.Requires(supportedSlnProjects != null);
            Contract.Requires(projects != null);
            Contract.Requires(targetDir != null);
            Contract.Requires(suiteRoot != null);
            Contract.Requires(slnNameGenerator != null);
            Contract.Requires(inSolutionReferenceBuilderFactory != null);

            this.projectGuidManagement = projectGuidManagement;
            this.supportedSlnProjects = supportedSlnProjects;
            this.suiteRoot = suiteRoot;
            this.projects = projects.ToList();
            this.targetDir = targetDir;
            this.slnNameGenerator = slnNameGenerator;
            this.inSolutionReferenceBuilderFactory = inSolutionReferenceBuilderFactory;
            this.projectPlatformManagement = projectPlatformManagement;

            inSolutionReferences = new Dictionary<Project, ISet<Project>>();
        }

        /// <summary>
        /// Gets the projects added to the solution built by this builder
        /// </summary>
        public IEnumerable<Project> Projects
        {
            get { return projects; }
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
            get { return slnNameGenerator.GetName(projects); }
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

            projectDependencies = MultipleDependenciesHelper.CreateMultipleDependencies(projectBuilders);            
        }

        private IBuilder CreateProjectBuilder(IBuildContext context, Project project)
        {
            var slnProject = supportedSlnProjects.FirstOrDefault(prj => prj.SupportsProject(project));
            if (slnProject != null)
            {
                var projectBuilder = slnProject.CreateBuilder(project);
                projectBuilder.AddToContext(context);
                return projectBuilder;
            }
            else
            {
                return null;
            }
        }

        private bool TransformRedundantSolutionBuilds(ISet<IDirectedGraphEdge<IBuilder>> builders)
        {
            var subNodes = builders.BuildNodes(this).DirectedBreadthFirstTraversal((builder, bs) => { bs.Add(builder); return bs; }, new List<IBuilder>());
            foreach (var builderNode in subNodes)
            {
                var moduleReferenceBuilder = builderNode as ModuleReferenceBuilder;
                if (moduleReferenceBuilder != null)
                {
                    if (moduleReferenceBuilder.Reference.Type == ReferenceType.Build &&
                        projects.Contains(moduleReferenceBuilder.ReferencedProject))
                    {
                        log.DebugFormat("Transforming module reference builder {0}", moduleReferenceBuilder);

                        ConvertToInSolutionReference(builders, moduleReferenceBuilder, moduleReferenceBuilder.ReferencedProject);
                    }
                }

                var suiteReferenceBuilder = builderNode as SuiteReferenceBuilder;
                if (suiteReferenceBuilder != null)
                {
                    if (suiteReferenceBuilder.Reference.Type == ReferenceType.Build &&
                        projects.Contains(suiteReferenceBuilder.ReferencedProject))
                    {
                        log.DebugFormat("Transforming module reference builder {0}", suiteReferenceBuilder);

                        ConvertToInSolutionReference(builders, suiteReferenceBuilder, suiteReferenceBuilder.ReferencedProject);                        
                    }
                }
            }

            return true;
        }

        private void RegisterInSolutionProjectReference(Project project, Project dependency)
        {
            ISet<Project> deps;
            if (!inSolutionReferences.TryGetValue(project, out deps))
            {
                deps = new HashSet<Project>();
                inSolutionReferences.Add(project, deps);
            }

            deps.Add(dependency);
        }

        private void ConvertToInSolutionReference(ISet<IDirectedGraphEdge<IBuilder>> builders, IReferenceBuilder moduleReferenceBuilder, Project referencedProject)
        {
            // finding edges X -> RB 
            var edges = builders.Where(edge => Equals(edge.Target, moduleReferenceBuilder)).ToList();

            // removing these edges
            foreach (var edge in edges)
                builders.Remove(edge);
            var sourceNodes = edges.Where(edge => !Equals(edge.Source, edge.Target)).Select(edge => edge.Source).ToList();

            // creating new, in-solution reference builder node (ISB)
            var inSolutionBuilder = inSolutionReferenceBuilderFactory.CreateInSolutionReferenceBuilder(referencedProject);
            inSolutionBuilder.Reference = moduleReferenceBuilder.Reference;

            // creating new edges: X -> ISB                        
            foreach (var newEdge in sourceNodes.Select(sourceNode => new SimpleDirectedGraphEdge<IBuilder>(sourceNode, inSolutionBuilder)))
                builders.Add(newEdge);

            // removing every edge containing RB
            var edgesToRemove = builders.Where(edge => Equals(edge.Target, moduleReferenceBuilder) || Equals(edge.Source, moduleReferenceBuilder)).ToList();
            foreach (var edge in edgesToRemove)
                builders.Remove(edge);

            // registering these project references for later use (in sln generator)
            foreach (var sourceNode in sourceNodes.OfType<IProjectBuilder>())
            {
                RegisterInSolutionProjectReference(sourceNode.Project, referencedProject);
            }
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
                var generator = new SlnGenerator(projectGuidManagement, projectPlatformManagement, supportedSlnProjects, projects, sln, suiteRoot, targetDir, GetInSolutionReferences);
                generator.Generate();
            }

            return new HashSet<TargetRelativePath> { new TargetRelativePath(String.Empty, slnPath) };
        }

        private IEnumerable<Project> GetInSolutionReferences(Project project)
        {
            ISet<Project> result;
            if (inSolutionReferences.TryGetValue(project, out result))
                return result;
            else
                return new Project[0];
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