using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.Tools;

namespace Bari.Plugins.VsCore.Build
{
    /// <summary>
    /// Builder for running MSBuild on a Visual Studio solution file.
    /// </summary>
    public class MSBuildRunner: BuilderBase<MSBuildRunner>, IBuilder, IEquatable<MSBuildRunner>
    {
        private readonly SlnBuilder slnBuilder;
        private readonly TargetRelativePath slnPath;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IMSBuild msbuild;

        /// <summary>
        /// Constructs the builder
        /// </summary>
        /// <param name="slnBuilder">Sub task building the solution file, used as a dependency</param>
        /// <param name="slnPath">Path of the generated solution file</param>
        /// <param name="targetRoot">Target directory</param>
        /// <param name="msbuild">The MSBuild implementation to use</param>
        public MSBuildRunner(SlnBuilder slnBuilder, TargetRelativePath slnPath,
                             [TargetRoot] IFileSystemDirectory targetRoot, IMSBuild msbuild)
        {
            this.slnBuilder = slnBuilder;
            this.slnPath = slnPath;
            this.targetRoot = targetRoot;
            this.msbuild = msbuild;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public override IDependencies Dependencies
        {
            get
            {
                return MultipleDependenciesHelper.CreateMultipleDependencies(
                    new HashSet<IDependencies>(new[]
                    {
                        new SubtaskDependency(slnBuilder),
                        slnBuilder.FullSourceDependencies
                    }));
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get { return slnBuilder.Uid; }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public override void AddToContext(IBuildContext context)
        {
            context.AddBuilder(this, new[] { slnBuilder });
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            msbuild.Run(targetRoot, slnPath);

            // Collecting all the files in 'targetdir/modulename' directories as results
            var modules = new HashSet<Module>(from proj in slnBuilder.Projects select proj.Module);
            var outputs = new HashSet<TargetRelativePath>();
            foreach (var module in modules)
            {
                var moduleTargetDir = targetRoot.GetChildDirectory(module.Name);
                if (moduleTargetDir != null)
                    foreach (var fileName in moduleTargetDir.Files)
                        outputs.Add(new TargetRelativePath(module.Name, fileName));
            }

            return outputs;
        }

        public override bool CanRun()
        {
            return true;
        }

        public override Type BuilderType
		{
			get
			{
				return typeof(MSBuildRunner);
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
            return String.Format("[msbuild:{0}]", slnPath);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(MSBuildRunner other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return slnBuilder.Equals(other.slnBuilder);
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
            return Equals((MSBuildRunner) obj);
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
            return slnBuilder.GetHashCode();
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(MSBuildRunner left, MSBuildRunner right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(MSBuildRunner left, MSBuildRunner right)
        {
            return !Equals(left, right);
        }
    }
}