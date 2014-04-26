using System;
using System.Collections.Generic;
using Bari.Core.Build.Cache;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;

namespace Bari.Core.Build
{
    [ShouldNotCache]
    public class CopyResultBuilder : IBuilder, IEquatable<CopyResultBuilder>
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
        public IDependencies Dependencies
        {
            get
            {
                return new SubtaskDependency(sourceBuilder);
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get { return sourceBuilder.Uid + "__" + targetRoot.GetRelativePath(targetDirectory).Replace('\\', '_'); }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            context.AddBuilder(this, new[] { sourceBuilder });
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var files = context.GetResults(sourceBuilder);

            var result = new HashSet<TargetRelativePath>();

            foreach (var sourcePath in files)
            {
                log.DebugFormat("Copying result {0}...", sourcePath);

                var relativePath = sourcePath.RelativePath;
                Copy(sourcePath, relativePath);

                result.Add(new TargetRelativePath(targetRoot.GetRelativePath(targetDirectory), relativePath));
            }

            return result;
        }

        private void Copy(TargetRelativePath sourcePath, string relativePath)
        {
            using (var source = targetRoot.ReadBinaryFile(sourcePath))
            using (var target = targetDirectory.CreateBinaryFileWithDirectories(relativePath))
                StreamOperations.Copy(source, target);
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
            if (obj.GetType() != this.GetType()) return false;
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