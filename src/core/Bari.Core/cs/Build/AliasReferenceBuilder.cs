using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// A reference builder implementation which is a placeholder for a set of real references
    /// given by a name.
    /// </summary>
    public class AliasReferenceBuilder: IReferenceBuilder, IEquatable<AliasReferenceBuilder>
    {
        private readonly Suite suite;
        private readonly Project project;
        private readonly IReferenceBuilderFactory referenceBuilderFactory;
        private readonly IList<IBuilder> builders = new List<IBuilder>();
        private Reference reference;
        private ReferenceAlias alias;

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
        public IDependencies Dependencies
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
        public string Uid
        {
            get { return string.Format("alias.{0}__{1}", reference.Uri.Host, reference.Type); }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            builders.Clear();

            if (alias != null && !context.Contains(this))
            {                
                foreach (var childRef in alias.References)
                {
                    if (childRef.Type == reference.Type)
                    {
                        var builder = referenceBuilderFactory.CreateReferenceBuilder(childRef, project);
                        builder.AddToContext(context);

                        builders.Add(builder);
                    }
                }

                context.AddBuilder(this, builders);
            }
            else
            {
                foreach (var dep in context.GetDependencies(this))
                    builders.Add(dep);
            }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var results = new HashSet<TargetRelativePath>();

            foreach (var builder in builders)
            {
                results.UnionWith(context.GetResults(builder));
            }

            return results;
        }

        /// <summary>
        /// Verifies if the builder is able to run. Can be used to fallback to cached results without getting en error.
        /// </summary>
        /// <returns>If <c>true</c>, the builder thinks it can run.</returns>
        public bool CanRun()
        {
            return true;
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
                {
                    if (suite.HasParameters("aliases"))
                    {
                        var aliases = suite.GetParameters<ReferenceAliases>("aliases");
                        alias = aliases.Get(reference.Uri.Host);
                    }
                }
            }
        }

        /// <summary>
        /// If <c>false</c>, the reference builder can be ignored as an optimization
        /// </summary>
        public bool IsEffective
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