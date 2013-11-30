using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Nuget.Tools;

namespace Bari.Plugins.Nuget.Build
{
    /// <summary>
    /// A <see cref="IReferenceBuilder"/> implementation downloading references with the NuGet package manager
    /// 
    /// <para>
    /// The reference URIs are interpreted in the following way:
    /// 
    /// <example>nuget://Ninject/Version</example>
    /// means that the latest version of the Ninject package should be downloaded and added as a reference.
    /// The version part is optional.
    /// </para>
    /// </summary>
    public class NugetReferenceBuilder: IReferenceBuilder, IEquatable<NugetReferenceBuilder>
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (NugetReferenceBuilder));

        private readonly INuGet nuget;
        private readonly IFileSystemDirectory targetRoot;

        private Reference reference;

        /// <summary>
        /// Constructs the builder
        /// </summary>
        /// <param name="nuget">Interface to the NuGet package manager</param>
        /// <param name="targetRoot">Target root directory</param>
        public NugetReferenceBuilder(INuGet nuget, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.nuget = nuget;
            this.targetRoot = targetRoot;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return new NoDependencies(); }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get { return reference.Uri.Host; }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            log.DebugFormat("Resolving reference {0}", reference.Uri);

            string pkgName = reference.Uri.Host;
            string pkgVersion = reference.Uri.AbsolutePath.TrimStart('/');            

            var depsRoot = targetRoot.CreateDirectory("deps");
            var depDir = depsRoot.CreateDirectory(pkgName);

            var files = nuget.InstallPackage(pkgName, pkgVersion, depDir, "", dllsOnly: reference.Type == ReferenceType.Build);
            var relativeRoot = Path.Combine(targetRoot.GetRelativePath(depDir), files.Item1);
            return new HashSet<TargetRelativePath>(
                from path in files.Item2
                let relativePath = path.Substring(files.Item1.Length).TrimStart(Path.DirectorySeparatorChar)
                select new TargetRelativePath(relativeRoot, relativePath));
        }

        /// <summary>
        /// Gets or sets the reference to be resolved
        /// </summary>
        public Reference Reference
        {
            get { return reference; }
            set { reference = value; }
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
            return string.Format("[{0}]", reference.Uri);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(NugetReferenceBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(reference, other.reference);
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
            return Equals((NugetReferenceBuilder) obj);
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
            return reference != null ? reference.GetHashCode() : 0;
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(NugetReferenceBuilder left, NugetReferenceBuilder right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(NugetReferenceBuilder left, NugetReferenceBuilder right)
        {
            return !Equals(left, right);
        }
    }
}