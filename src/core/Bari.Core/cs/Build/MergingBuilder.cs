using System;
using System.Collections.Generic;
using System.Text;
using Bari.Core.Build.Cache;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;

namespace Bari.Core.Build
{
    [ShouldNotCache]
    [AggressiveCacheRestore]
    public class MergingBuilder : BuilderBase<MergingBuilder>, IEquatable<MergingBuilder>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MergingBuilder));
        private readonly ISet<IBuilder> sourceBuilders;

        // Debug ID used only in ToString to help debugging
        private static int lastDebugId = 0;
        private readonly int debugId = lastDebugId++;

        public MergingBuilder(IEnumerable<IBuilder> sourceBuilders)
        {
            this.sourceBuilders = new HashSet<IBuilder>(sourceBuilders);
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public override IDependencies Dependencies
        {
            get { return MultipleDependenciesHelper.CreateMultipleDependencies(sourceBuilders); }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get
            {
                var uids = new StringBuilder();
                foreach (var builder in sourceBuilders)
                    uids.AppendLine(builder.Uid);

                return MD5.Encode(uids.ToString());
            }
        }

        /// <summary>
        /// Get the builders to be executed before this builder
        /// </summary>
        public override IEnumerable<IBuilder> Prerequisites
        {
            get { return sourceBuilders; }
        }

        public override void AddPrerequisite(IBuilder target)
        {
            if (sourceBuilders != null)
            {
                sourceBuilders.Add(target);
            }

            base.AddPrerequisite(target);
        }

        public override void RemovePrerequisite(IBuilder target)
        {
            if (sourceBuilders != null)
            {
                sourceBuilders.Remove(target);
            }

            base.RemovePrerequisite(target);
        }


        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var result = new HashSet<TargetRelativePath>();

            foreach (var builder in sourceBuilders)
            {
                log.DebugFormat("..merging {0}", builder);
                result.UnionWith(context.GetResults(builder));
            }

            return result;
        }

        public override string ToString()
        {
            return String.Format("[Merge #{0}]", debugId);
        }

        public bool Equals(MergingBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return sourceBuilders.SetEquals(other.sourceBuilders);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MergingBuilder)obj);
        }

        public override int GetHashCode()
        {
            return (sourceBuilders != null ? sourceBuilders.GetHashCode() : 0);
        }

        public static bool operator ==(MergingBuilder left, MergingBuilder right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MergingBuilder left, MergingBuilder right)
        {
            return !Equals(left, right);
        }
    }
}