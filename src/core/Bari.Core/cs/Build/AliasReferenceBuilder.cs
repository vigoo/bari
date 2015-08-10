using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Build.Cache;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// A reference builder implementation which is a placeholder for a set of real references
    /// given by a name.
    /// </summary>
	[AggressiveCacheRestore]
    public class AliasReferenceBuilder: ReferenceBuilderBase<AliasReferenceBuilder>, IEquatable<AliasReferenceBuilder>
    {
        private readonly Suite suite;
        private readonly Project project;
        private readonly IReferenceBuilderFactory referenceBuilderFactory;
        private readonly IList<IBuilder> builders = new List<IBuilder>();
        private Reference reference;
        private ReferenceAlias alias;
        private bool buildersReady;

        public AliasReferenceBuilder(Suite suite, Project project, IReferenceBuilderFactory referenceBuilderFactory)
        {
            Contract.Requires(suite != null);
            Contract.Requires(project != null);
            Contract.Requires(referenceBuilderFactory != null);

            this.suite = suite;
            this.project = project;
            this.referenceBuilderFactory = referenceBuilderFactory;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public override IDependencies Dependencies
        {
            get
            {
                if (alias != null)
                {
                    return MultipleDependenciesHelper.CreateMultipleDependencies(
                            new HashSet<IDependencies>(alias.References.Select(rf => new ReferenceDependency(rf))));
                }
                else
                {
                    return new NoDependencies();
                }
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get { return string.Format("alias.{0}__{1}", reference.Uri.Host, reference.Type); }
        }

        /// <summary>
        /// Get the builders to be executed before this builder
        /// </summary>
        public override IEnumerable<IBuilder> Prerequisites
        {
            get
            {
                if (!buildersReady && alias != null)
                {
                    foreach (var childRef in alias.References)
                    {
                        if (childRef.Type == reference.Type)
                        {
                            var builder = referenceBuilderFactory.CreateReferenceBuilder(childRef, project);
                            builders.Add(builder);
                        }
                    }

                    buildersReady = true;
                }

                return builders;
            }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var results = new HashSet<TargetRelativePath>();

            foreach (var builder in builders)
            {
                results.UnionWith(context.GetResults(builder));
            }

            return results;
        }


        /// <summary>
        /// Gets or sets the reference to be resolved
        /// </summary>
        public override Reference Reference
        {
            get { return reference; }
            set
            {
                if (reference != value)
                {
                    reference = value;
                    if (reference != null)
                    {
                        if (suite.HasParameters("aliases"))
                        {
                            var aliases = suite.GetParameters<ReferenceAliases>("aliases");
                            alias = aliases.Get(reference.Uri.Host);
                        }
                    }

                    builders.Clear();
                    buildersReady = false;
                }
            }
        }

        /// <summary>
        /// If <c>false</c>, the reference builder can be ignored as an optimization
        /// </summary>
        public override bool IsEffective
        {
            get
            {
                if (alias != null)
                    return alias.References.Any(r => r.Type == reference.Type);
                else
                    return false;
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
            return string.Format("[{0} ({1})]", reference.Uri, reference.Type);
        }


        public bool Equals(AliasReferenceBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(reference, other.reference);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AliasReferenceBuilder) obj);
        }

        public override int GetHashCode()
        {
            return (reference != null ? reference.GetHashCode() : 0);
        }

        public static bool operator ==(AliasReferenceBuilder left, AliasReferenceBuilder right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AliasReferenceBuilder left, AliasReferenceBuilder right)
        {
            return !Equals(left, right);
        }
    }
}