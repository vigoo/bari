using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Build.Dependencies;
using Bari.Plugins.VCpp.Model;
using Bari.Plugins.VCpp.VisualStudio;
using Bari.Plugins.VsCore.Build;

namespace Bari.Plugins.VCpp.Build
{
    /// <summary>
    /// Builder generating Visual C++ project file from a source code set
    /// 
    /// <para>Uses the <see cref="VcxprojGenerator"/> class internally.</para>
    /// </summary>
    public class VcxprojBuilder: BuilderBase<VcxprojBuilder>, ISlnProjectBuilder, IEquatable<VcxprojBuilder>
    {
        private readonly IReferenceBuilderFactory referenceBuilderFactory;
        private readonly ISourceSetDependencyFactory sourceSetDependencyFactory;

        private readonly Project project;
        private readonly Suite suite;
        private readonly IFileSystemDirectory targetDir;
        private readonly VcxprojGenerator generator;
        private ISet<IBuilder> referenceBuilders;
        private IDependencies dependencies;
        private IDependencies fullSourceDependencies;
        
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
        /// <param name="project">The project for which the vcxproj file will be generated</param>
        /// <param name="suite">The suite the project belongs to </param>
        /// <param name="targetDir">The build target directory </param>
        /// <param name="generator">The vcxproj generator class to be used</param>
        public VcxprojBuilder(IReferenceBuilderFactory referenceBuilderFactory, ISourceSetDependencyFactory sourceSetDependencyFactory, 
                             Project project, Suite suite, [TargetRoot] IFileSystemDirectory targetDir, VcxprojGenerator generator)
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
                if (dependencies == null)
                    dependencies = CreateDependencies();
                return dependencies;
            }
        }

        private IDependencies CreateDependencies()
        {
            var deps = new List<IDependencies>();

            if (project.HasNonEmptySourceSet("cpp"))
            {
                var filteredSourceSet = project.GetSourceSet("cpp").FilterCppSourceSet(
                    project.RootDirectory.GetChildDirectory("cpp"), suite.SuiteRoot);

                deps.Add(sourceSetDependencyFactory.CreateSourceSetStructureDependency(filteredSourceSet,
                    fn => fn.EndsWith(".vcxproj", StringComparison.InvariantCultureIgnoreCase) ||
                          fn.EndsWith(".vcxproj.user", StringComparison.InvariantCultureIgnoreCase)));
            }

            deps.Add(new ProjectPropertiesDependencies(project, "Name", "Type", "EffectiveVersion"));

            VCppATLParametersDependencies.Add(project, deps);
            VCppCLIParametersDependencies.Add(project, deps);
            VCppCompilerParametersDependencies.Add(project, deps);
            VCppLinkerParametersDependencies.Add(project, deps);
            VCppManifestParametersDependencies.Add(project, deps);
            VCppMIDLParametersDependencies.Add(project, deps);

            if (referenceBuilders != null)
                deps.AddRange(referenceBuilders.OfType<IReferenceBuilder>().Select(CreateReferenceDependency));

            //if (appConfigBuilder != null && appConfigBuilder.HasConfigFile)
            //    deps.Add(new SubtaskDependency(appConfigBuilder));

            return MultipleDependenciesHelper.CreateMultipleDependencies(new HashSet<IDependencies>(deps));
        }

        /// <summary>
        /// Gets the builder's full source code dependencies
        /// </summary>
        public IDependencies FullSourceDependencies
        {
            get 
            {
                if (fullSourceDependencies == null)
                    fullSourceDependencies = CreateFullSourceDependencies();
                return fullSourceDependencies;
            }
        }
        
        private IDependencies CreateFullSourceDependencies()
        {
            var deps = new List<IDependencies>();

            if (project.HasNonEmptySourceSet("cpp"))
            {
                var filteredSourceSet = project.GetSourceSet("cpp").FilterCppSourceSet(
                    project.RootDirectory.GetChildDirectory("cpp"), suite.SuiteRoot);

                deps.Add(sourceSetDependencyFactory.CreateSourceSetDependencies(filteredSourceSet,
                    fn => fn.EndsWith(".vcxproj", StringComparison.InvariantCultureIgnoreCase) ||
                          fn.EndsWith(".vcxproj.user", StringComparison.InvariantCultureIgnoreCase)));
            }

            return MultipleDependenciesHelper.CreateMultipleDependencies(new HashSet<IDependencies>(deps));
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

        public override void AddPrerequisite(IBuilder target)
        {
            if (referenceBuilders != null)
            {
                referenceBuilders.Add((IReferenceBuilder)target);
            }

            base.AddPrerequisite(target);
        }

        public override void RemovePrerequisite(IBuilder target)
        {
            if (referenceBuilders != null)
            {
                if (referenceBuilders.Contains(target))
                {
                    referenceBuilders.Remove((IReferenceBuilder)target);
                }
            }

            base.RemovePrerequisite(target);
        }


        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in suite relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var vcxprojPath = project.Name + ".vcxproj";
            const string csversionPath = "version.cpp";

            var csversion = project.GetVersionSupport() ? project.RootDirectory.CreateTextFile(csversionPath) : null;
            using (var fsproj = project.RootDirectory.GetChildDirectory("cpp").CreateTextFile(vcxprojPath))
            {

                var references = new HashSet<TargetRelativePath>();
                foreach (var refBuilder in context.GetDependencies(this).OfType<IReferenceBuilder>().Where(r => r.Reference.Type == ReferenceType.Build))
                {
                    var builderResults = context.GetResults(refBuilder);
                    references.UnionWith(builderResults);
                }

                generator.Generate(project, references, fsproj, csversion, csversionPath);
            }

            if (csversion != null)
            {
                csversion.Close();
                csversion.Dispose();
            }

            return new HashSet<TargetRelativePath>(
                new[]
                    {
                        new TargetRelativePath(String.Empty,
                            suite.SuiteRoot.GetRelativePathFrom(targetDir, 
                                Path.Combine(suite.SuiteRoot.GetRelativePath(project.RootDirectory), "cpp", vcxprojPath))),
                    });
        }

        /// <summary>
        /// Gets the target relative path to the outmost directory in which the <see cref="Run"/> method generates
        /// any files.
        /// </summary>
        public TargetRelativePath TargetRoot
        {
            get
            {
                return new TargetRelativePath(String.Empty,
                    suite.SuiteRoot.GetRelativePathFrom(targetDir,
                        Path.Combine(suite.SuiteRoot.GetRelativePath(project.RootDirectory))));
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
            return string.Format("[{0}.{1}.vcxproj]", project.Module.Name, project.Name);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(VcxprojBuilder other)
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
            return Equals((VcxprojBuilder)obj);
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
        public static bool operator ==(VcxprojBuilder left, VcxprojBuilder right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(VcxprojBuilder left, VcxprojBuilder right)
        {
            return !Equals(left, right);
        }
    }
}