using System;
using System.Collections.Generic;
using System.IO;
using Bari.Core.Build.Cache;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;

namespace Bari.Core.Build
{
    [ShouldNotCache]
    [AggressiveCacheRestore]
    public class CopyResultBuilder : BuilderBase<CopyResultBuilder>, IEquatable<CopyResultBuilder>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ContentBuilder));

        private readonly IBuilder sourceBuilder;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IFileSystemDirectory targetDirectory;

        public CopyResultBuilder(IBuilder sourceBuilder, [TargetRoot] IFileSystemDirectory targetRoot, IFileSystemDirectory targetDirectory)
        {
            this.sourceBuilder = sourceBuilder;
            this.targetRoot = targetRoot;
            this.targetDirectory = targetDirectory;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public override IDependencies Dependencies
        {
            get
            {
                return new SubtaskDependency(sourceBuilder);
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get { return sourceBuilder.Uid + "__" + targetRoot.GetRelativePath(targetDirectory).Replace(Path.DirectorySeparatorChar, '_'); }
        }

        /// <summary>
        /// Get the builders to be executed before this builder
        /// </summary>
        public override IEnumerable<IBuilder> Prerequisites
        {
            get { return new[] {sourceBuilder}; }
        }

        public override void AddPrerequisite(IBuilder target)
        {
            if (target != sourceBuilder)
                throw new Exception(String.Format("Unexpected override of copy result builder's source from {0} to {1}", sourceBuilder, target));
        }

        public override void RemovePrerequisite(IBuilder target)
        {
            if (target == sourceBuilder)
                throw new Exception(String.Format("Unexpected removal of copy result builder's source: {0}", sourceBuilder));
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var files = context.GetResults(sourceBuilder);

            var result = new HashSet<TargetRelativePath>();
            var relativeTargetDirectory = targetRoot.GetRelativePath(targetDirectory);
            foreach (var sourcePath in files)
            {
                log.DebugFormat("Copying result {0}...", sourcePath);

                var relativePath = sourcePath.RelativePath;
                targetRoot.CopyFile(sourcePath, targetDirectory, relativePath);

                result.Add(new TargetRelativePath(relativeTargetDirectory, relativePath));
            }

            return result;
        }

        public override BuilderName Name
        {
            get
            {
                return new BuilderName(String.Format("copy-result:{0}:{1}", sourceBuilder.Name.AsFull, targetDirectory)); 
            }
        }

        public override string ToString()
        {
            return String.Format("[result of {0} to {1}]", sourceBuilder, targetRoot.GetRelativePath(targetDirectory));
        }

        public bool Equals(CopyResultBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return sourceBuilder.Equals(other.sourceBuilder) &&
                   targetDirectory.Equals(other.targetDirectory);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CopyResultBuilder) obj);
        }

        public override int GetHashCode()
        {
            return sourceBuilder.GetHashCode() ^ targetDirectory.GetHashCode();
        }

        public static bool operator ==(CopyResultBuilder left, CopyResultBuilder right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CopyResultBuilder left, CopyResultBuilder right)
        {
            return !Equals(left, right);
        }
    }
}