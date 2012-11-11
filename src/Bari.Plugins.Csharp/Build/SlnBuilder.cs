using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.VisualStudio;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Syntax;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Builder for generating Visual Studio solution files from a set of projects.
    /// </summary>
    public class SlnBuilder : IBuilder, IEquatable<SlnBuilder>
    {
        private readonly IResolutionRoot root;
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IFileSystemDirectory targetDir;
        private readonly IList<Project> projects;
        private ISet<IBuilder> projectBuilders;
        private IDependencies projectDependencies;

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to use</param>
        /// <param name="projects">The projects to be included to the solution</param>
        /// <param name="targetDir">The target directory where the sln file should be put</param>
        /// <param name="root">Interface for creating new builder instances</param>
        public SlnBuilder(IProjectGuidManagement projectGuidManagement, IEnumerable<Project> projects, [TargetRoot] IFileSystemDirectory targetDir, IResolutionRoot root)
        {
            Contract.Requires(projectGuidManagement != null);
            Contract.Requires(projects != null);
            Contract.Requires(targetDir != null);

            this.projectGuidManagement = projectGuidManagement;
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
        {
            if (project.HasNonEmptySourceSet("cs"))
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
                var generator = new SlnGenerator(projectGuidManagement, projects, sln);
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
            if (obj.GetType() != this.GetType()) return false;
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