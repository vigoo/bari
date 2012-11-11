using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.VisualStudio;
using Ninject.Extensions.ChildKernel;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Builder generating a Visual C# project file from a source code set
    /// 
    /// <para>Uses the <see cref="CsprojGenerator"/> class internally.</para>
    /// </summary>
    public class CsprojBuilder : IBuilder, IEquatable<CsprojBuilder>
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IResolutionRoot root;
        private readonly Project project;
        private readonly IFileSystemDirectory targetDir;
        private ISet<IBuilder> referenceBuilders;

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to use</param>
        /// <param name="root">Path to resolve instances</param>
        /// <param name="project">The project for which the csproj file will be generated</param>
        /// <param name="targetDir">The target directory where the csproj file should be put</param>
        public CsprojBuilder(IProjectGuidManagement projectGuidManagement, IResolutionRoot root, Project project, [TargetRoot] IFileSystemDirectory targetDir)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.root = root;
            this.project = project;
            this.targetDir = targetDir;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get
            {
                if (project.HasNonEmptySourceSet("cs"))
                {
                    return new MultipleDependencies(
                        new IDependencies[]
                            {
                                new SourceSetDependencies(root, project.GetSourceSet("cs")),
                                new ProjectPropertiesDependencies(project, "Name", "Type")
                            }
                            .Concat(
                                from refBuilder in referenceBuilders
                                select new SubtaskDependency(refBuilder)));
                }
                else
                {
                    return new NoDependencies();
                }
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get { return project.Module.Name + "." + project.Name; }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            referenceBuilders = new HashSet<IBuilder>(project.References.Select(CreateReferenceBuilder));

            var childKernel = new ChildKernel(root);
            childKernel.Bind<Project>().ToConstant(project);

            foreach (var refBuilder in referenceBuilders)
                refBuilder.AddToContext(context);

            context.AddBuilder(this, referenceBuilders);
        }


        private IBuilder CreateReferenceBuilder(Reference reference)
        {
            var builder = root.GetReferenceBuilder<IReferenceBuilder>(reference, new ConstructorArgument("project", project, shouldInherit: true));
            if (builder != null)
            {
                return builder;
            }
            else
                throw new InvalidReferenceTypeException(reference.Uri.Scheme);
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in suite relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {            
            var csprojPath = project.Name + ".csproj";
            using (var csproj = targetDir.CreateTextFile(csprojPath))
            {
                var references = new HashSet<TargetRelativePath>();
                foreach (var refBuilder in referenceBuilders)
                {
                    var builderResults = context.GetResults(refBuilder);
                    references.UnionWith(builderResults);
                }

                var generator = new CsprojGenerator(projectGuidManagement, "..", project, references, csproj); // TODO: path to suite root should not be hard coded
                generator.Generate();
            }

            return new HashSet<TargetRelativePath>(
                new[]
                    {
                        new TargetRelativePath(csprojPath)
                    });
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
            return string.Format("[{0}.{1}.csproj]", project.Module.Name, project.Name);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(CsprojBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(project, other.project);
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
            return Equals((CsprojBuilder) obj);
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
            return (project != null ? project.GetHashCode() : 0);
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(CsprojBuilder left, CsprojBuilder right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(CsprojBuilder left, CsprojBuilder right)
        {
            return !Equals(left, right);
        }
    }
}