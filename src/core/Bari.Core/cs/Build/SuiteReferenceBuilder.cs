using System;
using System.Collections.Generic;
using Bari.Core.Build.Dependencies;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// A <see cref="IReferenceBuilder"/> implementation for depending on another project within the suite
    /// 
    /// <para>
    /// The reference URIs are interpreted in the following way:
    /// 
    /// <example>suite://ModuleName/ProjectName</example>
    /// means the project called <c>ProjectName</c> in the given module.
    /// </para>
    /// </summary>
    public class SuiteReferenceBuilder : IReferenceBuilder, IEquatable<SuiteReferenceBuilder>
    {
        private readonly Suite suite;
        private readonly IEnumerable<IProjectBuilderFactory> projectBuilders;
        private Reference reference;
        private ISet<IBuilder> subtasks;
        private Project referencedProject;

        /// <summary>
        /// Gets the referenced project
        /// </summary>
        public Project ReferencedProject
        {
            get { return referencedProject; }
        }

        /// <summary>
        /// Constructs the reference builder
        /// </summary>
        /// <param name="suite">The suite being built </param>
        /// <param name="projectBuilders">Project builders available</param>
        public SuiteReferenceBuilder(Suite suite, IEnumerable<IProjectBuilderFactory> projectBuilders)
        {
            this.suite = suite;
            this.projectBuilders = projectBuilders;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return MultipleDependenciesHelper.CreateMultipleDependencies(subtasks); }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get { return reference.Uri.Host + "." + Reference.Uri.AbsolutePath.TrimStart('/'); }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            var moduleName = reference.Uri.Host;
            var projectName = reference.Uri.AbsolutePath.TrimStart('/');

            if (suite.HasModule(moduleName))
            {
                var module = suite.GetModule(moduleName);
                referencedProject = module.GetProjectOrTestProject(projectName);

                if (!context.Contains(this))
                {
                    if (referencedProject != null)
                    {
                        subtasks = new HashSet<IBuilder>();
                        context.AddBuilder(this, SubtaskGenerator(context));
                    }
                    else
                    {
                        throw new InvalidReferenceException(string.Format("Module {0} has no project called {1}",
                            moduleName, projectName));
                    }
                }
                else
                {
                    subtasks = new HashSet<IBuilder>(context.GetDependencies(this));
                }
            }
            else
            {
                throw new InvalidReferenceException(string.Format("Suite has no module called {0}", moduleName));
            }
        }

        private IEnumerable<IBuilder> SubtaskGenerator(IBuildContext context)
        {
            foreach (var projectBuilder in projectBuilders)
            {
                var builder = projectBuilder.AddToContext(context, new[] { referencedProject });
                if (builder != null)
                {
                    subtasks.Add(builder);
                    yield return builder;
                }
            }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var result = new HashSet<TargetRelativePath>();
            foreach (var subtask in subtasks)
            {
                var subResults = context.GetResults(subtask);
                result.UnionWith(subResults);
            }

            return result;
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
        /// If <c>false</c>, the reference builder can be ignored as an optimization
        /// </summary>
        public bool IsEffective
        {
            get { return true; }
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

        public bool Equals(SuiteReferenceBuilder other)
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
            return Equals((SuiteReferenceBuilder)obj);
        }

        public override int GetHashCode()
        {
            return (reference != null ? reference.GetHashCode() : 0);
        }

        public static bool operator ==(SuiteReferenceBuilder left, SuiteReferenceBuilder right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SuiteReferenceBuilder left, SuiteReferenceBuilder right)
        {
            return !Equals(left, right);
        }
    }
}