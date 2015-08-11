using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Fsharp.Build.Dependencies;
using Bari.Plugins.Fsharp.VisualStudio;
using Bari.Plugins.VsCore.Build;

namespace Bari.Plugins.Fsharp.Build
{
    /// <summary>
    /// Builder generating Visual F# project file from a source code set
    /// 
    /// <para>Uses the <see cref="FsprojGenerator"/> class internally.</para>
    /// </summary>
    public class FsprojBuilder: BuilderBase<FsprojBuilder>, ISlnProjectBuilder, IEquatable<FsprojBuilder>
    {
        private readonly IReferenceBuilderFactory referenceBuilderFactory;
        private readonly ISourceSetDependencyFactory sourceSetDependencyFactory;

        private readonly Project project;
        private readonly Suite suite;
        private readonly IFileSystemDirectory targetDir;
        private readonly FsprojGenerator generator;
        private ISet<IBuilder> referenceBuilders;

        /// <summary>
        /// Gets the project this builder is working on
        /// </summary>
        public Project Project
        {
            get { return project; }
        }

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="referenceBuilderFactory">Interface to create new reference builder instances</param>
        /// <param name="sourceSetDependencyFactory">Interface to create new source set dependencies</param>
        /// <param name="project">The project for which the fsproj file will be generated</param>
        /// <param name="suite">The suite the project belongs to </param>
        /// <param name="targetDir">The build target directory </param>
        /// <param name="generator">The fsproj generator class to be used</param>
        public FsprojBuilder(IReferenceBuilderFactory referenceBuilderFactory, ISourceSetDependencyFactory sourceSetDependencyFactory, 
                             Project project, Suite suite, [TargetRoot] IFileSystemDirectory targetDir, FsprojGenerator generator)
        {
            this.referenceBuilderFactory = referenceBuilderFactory;
            this.sourceSetDependencyFactory = sourceSetDependencyFactory;
            this.project = project;
            this.suite = suite;
            this.targetDir = targetDir;
            this.generator = generator;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public override IDependencies Dependencies
        {
            get
            {
                var deps = new List<IDependencies>();

                if (project.HasNonEmptySourceSet("fs"))
                    deps.Add(sourceSetDependencyFactory.CreateSourceSetStructureDependency(project.GetSourceSet("fs"),
                        fn => fn.EndsWith(".fsproj", StringComparison.InvariantCultureIgnoreCase) ||
                              fn.EndsWith(".fsproj.user", StringComparison.InvariantCultureIgnoreCase)));

                deps.Add(new ProjectPropertiesDependencies(project, "Name", "Type", "EffectiveVersion"));

                FsharpParametersDependencies.Add(project, deps);
                FileOrderDependencies.Add(project, deps);

                if (referenceBuilders != null)
                    deps.AddRange(referenceBuilders.OfType<IReferenceBuilder>().Select(CreateReferenceDependency));

                return MultipleDependenciesHelper.CreateMultipleDependencies(new HashSet<IDependencies>(deps));
            }
        }

        /// <summary>
        /// Gets the builder's full source code dependencies
        /// </summary>
        public IDependencies FullSourceDependencies
        {
            get
            {
                var deps = new List<IDependencies>();

                if (project.HasNonEmptySourceSet("fs"))
                    deps.Add(sourceSetDependencyFactory.CreateSourceSetDependencies(project.GetSourceSet("fs"),
                        fn => fn.EndsWith(".fsproj", StringComparison.InvariantCultureIgnoreCase) ||
                              fn.EndsWith(".fsproj.user", StringComparison.InvariantCultureIgnoreCase)));

                return MultipleDependenciesHelper.CreateMultipleDependencies(new HashSet<IDependencies>(deps));
            }
        }

        private IDependencies CreateReferenceDependency(IReferenceBuilder refBuilder)
        {
            return new MultipleDependencies(
                new SubtaskDependency(refBuilder),
                new ReferenceDependency(refBuilder.Reference));
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get { return project.Module.Name + "." + project.Name; }
        }

        /// <summary>
        /// Get the builders to be executed before this builder
        /// </summary>
        public override IEnumerable<IBuilder> Prerequisites
        {
            get
            {
                if (referenceBuilders == null)
                {
                    referenceBuilders = new HashSet<IBuilder>(project.References.Select(CreateReferenceBuilder));
                }

                return referenceBuilders;
            }
        }

        private IBuilder CreateReferenceBuilder(Reference reference)
        {
            var builder = referenceBuilderFactory.CreateReferenceBuilder(reference, project);
            if (builder != null)
            {
                return builder;
            }
            else
                throw new InvalidReferenceTypeException(reference.Uri.Scheme);
        }

        public void ReplaceReferenceBuilder(IReferenceBuilder original, IReferenceBuilder replacement)
        {
            if (referenceBuilders != null)
            {
                if (referenceBuilders.Contains(original))
                {
                    referenceBuilders.Remove(original);
                    referenceBuilders.Add(replacement);
                }
            }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in suite relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var fsprojPath = project.Name + ".fsproj";
            const string fsversionPath = "version.fs";

            using (var fsproj = project.RootDirectory.GetChildDirectory("fs").CreateTextFile(fsprojPath))
            using (var fsversion = project.RootDirectory.CreateTextFile(fsversionPath))
            {
                var references = new HashSet<TargetRelativePath>();
                foreach (var refBuilder in context.GetDependencies(this).OfType<IReferenceBuilder>().Where(r => r.Reference.Type == ReferenceType.Build))
                {
                    var builderResults = context.GetResults(refBuilder);
                    references.UnionWith(builderResults);
                }

                generator.Generate(project, references, fsproj, fsversion, fsversionPath);
            }

            return new HashSet<TargetRelativePath>(
                new[]
                    {
                        new TargetRelativePath(String.Empty,
                            suite.SuiteRoot.GetRelativePathFrom(targetDir, 
                                Path.Combine(suite.SuiteRoot.GetRelativePath(project.RootDirectory), "fs", fsprojPath))),
                        new TargetRelativePath(String.Empty,
                            suite.SuiteRoot.GetRelativePathFrom(targetDir, 
                                Path.Combine(suite.SuiteRoot.GetRelativePath(project.RootDirectory), fsversionPath)))
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
            return string.Format("[{0}.{1}.fsproj]", project.Module.Name, project.Name);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(FsprojBuilder other)
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
            if (obj.GetType() != GetType()) return false;
            return Equals((FsprojBuilder)obj);
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
        public static bool operator ==(FsprojBuilder left, FsprojBuilder right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(FsprojBuilder left, FsprojBuilder right)
        {
            return !Equals(left, right);
        }        
    }
}