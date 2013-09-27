using System;
using System.Collections.Generic;
using System.IO;
using Bari.Core.Build;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.FSRepository.Build.Dependencies;
using Bari.Plugins.FSRepository.Model;

namespace Bari.Plugins.FSRepository.Build
{
    public class FSRepositoryReferenceBuilder : IReferenceBuilder, IEquatable<FSRepositoryReferenceBuilder>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (FSRepositoryReferenceBuilder));

        private readonly Suite suite;
        private readonly IFSRepositoryFingerprintFactory fingerprintFactory;
        private readonly IFileSystemRepositoryAccess repository;
        private readonly IFileSystemDirectory targetRoot;

        private Reference reference;
        private string resolvedPath;
        private IPatternResolutionContext resolutionContext;

        public FSRepositoryReferenceBuilder(Suite suite, IFSRepositoryFingerprintFactory fingerprintFactory, IFileSystemRepositoryAccess repository, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.suite = suite;
            this.fingerprintFactory = fingerprintFactory;
            this.repository = repository;
            this.targetRoot = targetRoot;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get
            {
                return new FSRepositoryReferenceDependencies(fingerprintFactory, repository, resolvedPath);
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get { return reference.Uri.ToString().Replace('/', '.'); }
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
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            log.DebugFormat("Resolving reference {0} using {1}", reference.Uri, resolvedPath);

            var depsRoot = targetRoot.CreateDirectory("deps");
            var depDir = depsRoot.CreateDirectory(resolutionContext.DependencyName);

            repository.Copy(resolvedPath, depDir);

            return new HashSet<TargetRelativePath>(new[]
                {
                    new TargetRelativePath(Path.Combine(targetRoot.GetRelativePath(depDir), Path.GetFileName(resolvedPath)))
                });
        }

        /// <summary>
        /// Gets or sets the reference to be resolved
        /// </summary>
        public Reference Reference
        {
            get { return reference; }
            set 
            { 
                reference = value;
                if (reference != null)
                    ResolveReference();
            }
        }

        private void ResolveReference()
        {
            var uri = reference.Uri;
            var repositories = suite.GetFSRepositories();
            resolutionContext = new UriBasedPatternResolutionContext(uri);
            resolvedPath = repositories.Resolve(resolutionContext);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(FSRepositoryReferenceBuilder other)
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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FSRepositoryReferenceBuilder) obj);
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
            return (reference != null ? reference.GetHashCode() : 0);
        }

        public static bool operator ==(FSRepositoryReferenceBuilder left, FSRepositoryReferenceBuilder right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FSRepositoryReferenceBuilder left, FSRepositoryReferenceBuilder right)
        {
            return !Equals(left, right);
        }
    }
}