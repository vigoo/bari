using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.SolutionItems;
using Bari.Plugins.VsCore.VisualStudio.SolutionName;

namespace Bari.Plugins.VsCore.Build
{
    /// <summary>
    /// Builder for generating Visual Studio solution files from a set of projects.
    /// </summary>
    public class SlnBuilder : IBuilder, IEquatable<SlnBuilder>
    {
        private readonly IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory;
        private readonly IEnumerable<ISolutionItemProvider> solutionItemProviders;
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IProjectPlatformManagement projectPlatformManagement;
        private readonly IFileSystemDirectory targetDir;
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IList<Project> projects;
        private readonly ISlnNameGenerator slnNameGenerator;
        private readonly IEnumerable<ISlnProject> supportedSlnProjects;
        private ISet<ISlnProjectBuilder> projectBuilders;
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
        /// <param name="solutionItemProviders">List of registered solution item providers</param>
        public SlnBuilder(IProjectGuidManagement projectGuidManagement, IProjectPlatformManagement projectPlatformManagement, IEnumerable<ISlnProject> supportedSlnProjects, IEnumerable<Project> projects, [SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetDir, ISlnNameGenerator slnNameGenerator, IInSolutionReferenceBuilderFactory inSolutionReferenceBuilderFactory, IEnumerable<ISolutionItemProvider> solutionItemProviders)
        {
            Contract.Requires(projectGuidManagement != null);
            Contract.Requires(projectPlatformManagement != null);
            Contract.Requires(supportedSlnProjects != null);
            Contract.Requires(projects != null);
            Contract.Requires(targetDir != null);
            Contract.Requires(suiteRoot != null);
            Contract.Requires(slnNameGenerator != null);
            Contract.Requires(inSolutionReferenceBuilderFactory != null);
            Contract.Requires(solutionItemProviders != null);

            this.projectGuidManagement = projectGuidManagement;
            this.supportedSlnProjects = supportedSlnProjects;
            this.suiteRoot = suiteRoot;
            this.projects = projects.ToList();
            this.targetDir = targetDir;
            this.slnNameGenerator = slnNameGenerator;
            this.inSolutionReferenceBuilderFactory = inSolutionReferenceBuilderFactory;
            this.solutionItemProviders = solutionItemProviders;
            this.projectPlatformManagement = projectPlatformManagement;
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

        public IDependencies FullSourceDependencies
        {
            get
            {
                return
                    MultipleDependenciesHelper.CreateMultipleDependencies(
                        new HashSet<IDependencies>(projectBuilders.Select(b => b.FullSourceDependencies)));
            }
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
            if (!context.Contains(this))
            {
                var solutionBuildContext = new SolutionBuildContext(inSolutionReferenceBuilderFactory, context, this);

                projectBuilders = new HashSet<ISlnProjectBuilder>(
                    from project in projects
                    select CreateProjectBuilder(solutionBuildContext, project)
                    into builder
                    where builder != null
                    select builder
                    );

                solutionBuildContext.AddBuilder(this, projectBuilders);
            }
            else
            {
                projectBuilders = new HashSet<ISlnProjectBuilder>(context.GetDependencies(this).Cast<ISlnProjectBuilder>());
            }

            projectDependencies = MultipleDependenciesHelper.CreateMultipleDependencies(new HashSet<IBuilder>(projectBuilders));
        }

        private ISlnProjectBuilder CreateProjectBuilder(IBuildContext context, Project project)
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
                var generator = new SlnGenerator(projectGuidManagement, projectPlatformManagement, supportedSlnProjects, projects, sln, suiteRoot, targetDir, GetInSolutionReferences, solutionItemProviders, Uid);
                generator.Generate();
            }

            return new HashSet<TargetRelativePath> { new TargetRelativePath(String.Empty, slnPath) };
        }

        private IEnumerable<Project> GetInSolutionReferences(Project project)
        {
            foreach (var reference in project.References)
            {
                if (reference.Type == ReferenceType.Build)
                {
                    if (reference.Uri.Scheme == "module")
                    {
                        var projectName = reference.Uri.Host;
                        foreach (var dependency in projects.Where(other => 
                            other.Module == project.Module && String.Equals(other.Name, projectName, StringComparison.InvariantCultureIgnoreCase)))
                            yield return dependency;
                    }
                    else if (reference.Uri.Scheme == "suite")
                    {
                        var moduleName = reference.Uri.Host;
                        var projectName = reference.Uri.AbsolutePath.TrimStart('/');

                        foreach (var dependency in projects.Where(other => 
                            String.Equals(other.Module.Name, moduleName, StringComparison.InvariantCultureIgnoreCase) && 
                            String.Equals(other.Name, projectName, StringComparison.InvariantCultureIgnoreCase)))
                            yield return dependency;
                    }
                }
            }
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
            return new HashSet<Project>(projects).SetEquals(new HashSet<Project>(other.projects));
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
            var result = 11;
            if (projects != null)
                result = projects.Aggregate(result, (current, project) => current ^ project.GetHashCode());

            return result;
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