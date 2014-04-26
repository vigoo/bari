using System;
using System.Collections.Generic;
using System.Text;
using Bari.Core.Build.Cache;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;

namespace Bari.Core.Build
{
    [ShouldNotCache]
    public class MergingBuilder : IBuilder, IEquatable<MergingBuilder>
    {
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
        public IDependencies Dependencies
        {
            get { return MultipleDependenciesHelper.CreateMultipleDependencies(sourceBuilders); }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
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
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            foreach (var builder in sourceBuilders)
                builder.AddToContext(context);

            context.AddBuilder(this, sourceBuilders);
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var result = new HashSet<TargetRelativePath>();

            foreach (var builder in sourceBuilders)
                result.UnionWith(context.GetResults(builder));

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