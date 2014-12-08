using System;
using System.Collections.Generic;
using System.IO;
using Bari.Core.Build;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.UI;
using Bari.Plugins.FSRepository.Build.Dependencies;
using Bari.Plugins.FSRepository.Model;

namespace Bari.Plugins.FSRepository.Build
{
    [PersistentReference]
    [FallbackToCache]
    public class FSRepositoryReferenceBuilder : IReferenceBuilder, IEquatable<FSRepositoryReferenceBuilder>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (FSRepositoryReferenceBuilder));

        private readonly Suite suite;
        private readonly IFSRepositoryFingerprintFactory fingerprintFactory;
        private readonly IFileSystemRepositoryAccess repository;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IUserOutput output;

        private Reference reference;
        private string resolvedPath;
        private IPatternResolutionContext resolutionContext;

        public FSRepositoryReferenceBuilder(Suite suite, IFSRepositoryFingerprintFactory fingerprintFactory, IFileSystemRepositoryAccess repository, [TargetRoot] IFileSystemDirectory targetRoot, IUserOutput output)
        {
            this.suite = suite;
            this.fingerprintFactory = fingerprintFactory;
            this.repository = repository;
            this.targetRoot = targetRoot;
            this.output = output;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get
            {
                if (reference != null && resolutionContext == null)
                    ResolveReference();

                return new FSRepositoryReferenceDependencies(fingerprintFactory, repository, resolvedPath);
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get
            {
                return String.Format("{0}.{1}", reference.Uri.Host, reference.Uri.PathAndQuery.Replace('/', '.'))
                    .Replace("*.*", "___allfiles___");
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
            context.AddBuilder(this, new IBuilder[0]);
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            if (reference != null && resolutionContext == null)
                ResolveReference();

            if (output != null)
                output.Message(String.Format("Resolving reference {0}", reference.Uri));

            log.DebugFormat("Resolving reference {0} using {1}", reference.Uri, resolvedPath);

            var depsRoot = targetRoot.CreateDirectory("deps");
            var depDir = depsRoot.CreateDirectory(resolutionContext.DependencyName);

            string fileName = resolutionContext.FileName + "." + resolutionContext.Extension;

            if (fileName == "*.*")
                return DeployDirectoryContents(depDir);
            else
                return DeploySingleFile(depDir, fileName);
        }

        public bool CanRun()
        {
            if (reference != null && resolutionContext == null)
                ResolveReference();

            string fileName = resolutionContext.FileName + "." + resolutionContext.Extension;

            if (fileName == "*.*")
                return Directory.Exists(Path.GetDirectoryName(resolvedPath));
            else
                return File.Exists(resolvedPath);
        }

        private ISet<TargetRelativePath> DeployDirectoryContents(IFileSystemDirectory depDir)
        {
            var result = new HashSet<TargetRelativePath>();
            foreach (var file in repository.ListFiles(Path.GetDirectoryName(resolvedPath)))
            {
                var fileName = Path.GetFileName(file);
                repository.Copy(file, depDir, fileName);
                result.Add(new TargetRelativePath(targetRoot.GetRelativePath(depDir), fileName));
            }

            return result;
        }

        private ISet<TargetRelativePath> DeploySingleFile(IFileSystemDirectory depDir, string fileName)
        {
            repository.Copy(resolvedPath, depDir, fileName);

            return new HashSet<TargetRelativePath>(new[]
                {
                    new TargetRelativePath(targetRoot.GetRelativePath(depDir), fileName)
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
                resolutionContext = null;
                resolvedPath = null;                
            }
        }

        /// <summary>
        /// If <c>false</c>, the reference builder can be ignored as an optimization
        /// </summary>
        public bool IsEffective
        {
            get { return true; }
        }

        private void ResolveReference()
        {
            var uri = reference.Uri;
            var repositories = suite.GetFSRepositories();
            resolutionContext = new UriBasedPatternResolutionContext(uri);
            resolvedPath = repositories.Resolve(resolutionContext);

            if (resolvedPath == null)
                throw new InvalidReferenceException("Could not resolve FS repository dependency: " + uri);
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
    }
}