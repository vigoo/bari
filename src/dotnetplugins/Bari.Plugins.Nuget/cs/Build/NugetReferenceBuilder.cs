using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Cache;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.UI;
using Bari.Plugins.Csharp.Model;
using Bari.Plugins.Nuget.Tools;
using Bari.Plugins.VsCore.Model;

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
    [PersistentReference]
    [FallbackToCache]
    [AggressiveCacheRestore]
    public class NugetReferenceBuilder: ReferenceBuilderBase<NugetReferenceBuilder>, IEquatable<NugetReferenceBuilder>
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (NugetReferenceBuilder));

        private readonly INuGet nuget;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IUserOutput output;
        private readonly Project project;

        private Reference reference;

        /// <summary>
        /// Constructs the builder
        /// </summary>
        /// <param name="nuget">Interface to the NuGet package manager</param>
        /// <param name="targetRoot">Target root directory</param>
        /// <param name="output">User output interface</param>
        /// <param name="project">The project his reference belong sto</param>
        public NugetReferenceBuilder(INuGet nuget, [TargetRoot] IFileSystemDirectory targetRoot, IUserOutput output, Project project)
        {
            this.nuget = nuget;
            this.targetRoot = targetRoot;
            this.output = output;
            this.project = project;
        }
        
        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get { return reference.Uri.Host + "-" + reference.Uri.AbsolutePath.TrimStart('/'); }
        }


        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            if (output != null)
                output.Message(String.Format("Resolving reference {0}", reference.Uri));

            string pkgName = reference.Uri.Host;
            string pkgVersion = reference.Uri.AbsolutePath.TrimStart('/');            

            var depsRoot = targetRoot.CreateDirectory("deps");
            var depDir = depsRoot.CreateDirectory(pkgName);

            var files = nuget.InstallPackage(pkgName, pkgVersion, depDir, "", dllsOnly: reference.Type == ReferenceType.Build, maxProfile: GetMaxProfile());
            var relativeRoot = Path.Combine(targetRoot.GetRelativePath(depDir), files.Item1);
            return new HashSet<TargetRelativePath>(
                from path in files.Item2
                let relativePath = path.Substring(files.Item1.Length).TrimStart(Path.DirectorySeparatorChar)
                select reference.Type == ReferenceType.Build ?
                    new TargetRelativePath(relativeRoot, relativePath) :
                    new TargetRelativePath(
                        Path.GetDirectoryName(Path.Combine(relativeRoot, relativePath)),
                        Path.GetFileName(Path.Combine(relativeRoot, relativePath))));
        }

        private NugetLibraryProfile GetMaxProfile()
        {
            if (project != null)
            {
                var csharpParams = project.GetInheritableParameters<CsharpProjectParameters, CsharpProjectParametersDef>("csharp");
                var frameworkVersion = csharpParams.IsTargetFrameworkVersionSpecified
                    ? csharpParams.TargetFrameworkVersion
                    : FrameworkVersion.v4;
                var frameworkProfile = csharpParams.IsTargetFrameworkProfileSpecified
                    ? csharpParams.TargetFrameworkProfile
                    : FrameworkProfile.Default;

                switch (frameworkVersion)
                {
                    case FrameworkVersion.v20: return NugetLibraryProfile.Net2;
                    case FrameworkVersion.v30: return NugetLibraryProfile.Net3;
                    case FrameworkVersion.v35:
                        return frameworkProfile == FrameworkProfile.Default
                            ? NugetLibraryProfile.Net35
                            : NugetLibraryProfile.Net35Client;
                    case FrameworkVersion.v4:
                        return frameworkProfile == FrameworkProfile.Default
                            ? NugetLibraryProfile.Net4
                            : NugetLibraryProfile.Net4Client;
                    case FrameworkVersion.v45:
                        return NugetLibraryProfile.Net45;
                    case FrameworkVersion.v451:
                        return NugetLibraryProfile.Net451;
                    case FrameworkVersion.v452:
                        return NugetLibraryProfile.Net452;
                    case FrameworkVersion.v46:
                        return NugetLibraryProfile.Net46;
                    case FrameworkVersion.v461:
                        return NugetLibraryProfile.Net461;
                    case FrameworkVersion.v462:
                        return NugetLibraryProfile.Net462;
                    case FrameworkVersion.v47:
                        return NugetLibraryProfile.Net47;
                    case FrameworkVersion.v471:
                        return NugetLibraryProfile.Net471;
                    case FrameworkVersion.v472:
                        return NugetLibraryProfile.Net472;
                    case FrameworkVersion.v48:
                        return NugetLibraryProfile.Net48;
                }
            }

            return NugetLibraryProfile.Net4;
        }

        /// <summary>
        /// Gets or sets the reference to be resolved
        /// </summary>
        public override Reference Reference
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