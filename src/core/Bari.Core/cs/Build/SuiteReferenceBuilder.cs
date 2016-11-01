using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build.Cache;
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
    /// 
    /// <example>module://ProjectName</example>
    /// means the project called <c>ProjectName</c> in the referring project's module.
    /// </para>
    /// </summary>
    [AggressiveCacheRestore]
    public class SuiteReferenceBuilder : ReferenceBuilderBase<SuiteReferenceBuilder>, IEquatable<SuiteReferenceBuilder>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SuiteReferenceBuilder));

        private readonly Project project;
        private readonly Suite suite;
        private readonly Module module;
        private readonly IEnumerable<IProjectBuilderFactory> projectBuilders;
        private Reference reference;
        private ISet<IBuilder> subtasks;
        private Project referencedProject;

        /// <summary>
        /// Gets the referenced project
        /// </summary>
        public Project ReferencedProject
        {
            get
            {
                CalculateReferencedProject();
                return referencedProject;
            }
        }

        /// <summary>
        /// Constructs the reference builder
        /// </summary>
        /// <param name="project">The project using this reference</param>
        /// <param name="projectBuilders">Project builders available</param>
        public SuiteReferenceBuilder(Project project, IEnumerable<IProjectBuilderFactory> projectBuilders)
        {
            this.project = project;
            module = project.Module;
            suite = project.Module.Suite;
            this.projectBuilders = projectBuilders;
        }

        private static Project CalculateReferencedProject(Suite suite, Module module, Reference r)
        {
            switch (r.Uri.Scheme)
            {
                case "suite":
                    {
                        var moduleName = r.Uri.Host;
                        var projectName = r.Uri.AbsolutePath.TrimStart('/');

                        if (suite.HasModule(moduleName))
                        {
                            module = suite.GetModule(moduleName);
                            var result = module.GetProjectOrTestProject(projectName);

                            if (result == null)
                                throw new InvalidReferenceException(string.Format("Module {0} has no project called {1}", moduleName, projectName));

                            return result;
                        }
                        else
                        {
                            throw new InvalidReferenceException(string.Format("Suite has no module called {0}", moduleName));
                        }
                    }
                case "module":
                    {
                        string projectName = r.Uri.Host;
                        var result = module.GetProjectOrTestProject(projectName);

                        if (result == null)
                            throw new InvalidReferenceException(string.Format("Module {0} has no project called {1}", module.Name, projectName));

                        return result;
                    }
                default:
                    throw new InvalidOperationException("Unsupported suite reference type: " + r.Uri.Scheme);
            }
        }

        private void CalculateReferencedProject()
        {
            if (referencedProject == null)
            {
                referencedProject = CalculateReferencedProject(suite, module, reference);
            }
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public override IDependencies Dependencies
        {
            get
            {
                EnsureSubtasksAvailable();
                return MultipleDependenciesHelper.CreateMultipleDependencies(subtasks);
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get
            {
                switch (reference.Uri.Scheme)
                {
                    case "suite":
                        return reference.Uri.Host + "." + Reference.Uri.AbsolutePath.TrimStart('/');
                    case "module":
                        return module.Name + "." + Reference.Uri.Host;
                    default:
                        throw new InvalidOperationException("Unsupported suite reference type: " + reference.Uri.Scheme);
                }
            }
        }

        /// <summary>
        /// Get the builders to be executed before this builder
        /// </summary>
        public override IEnumerable<IBuilder> Prerequisites
        {
            get
            {
                EnsureSubtasksAvailable();
                return subtasks;
            }
        }

        private void EnsureSubtasksAvailable()
        {
            if (subtasks == null)
            {
                CalculateReferencedProject();

                subtasks = new HashSet<IBuilder>(GenerateSubtasks());

                log.DebugFormat("Project {0}'s reference to {1} adds the following subtasks: {2}", project, referencedProject,
                    String.Join(", ", subtasks));
            }
        }

        private IEnumerable<IBuilder> GenerateSubtasks()
        {
            foreach (var projectBuilder in projectBuilders)
            {
                var builder = projectBuilder.Create(new[] { referencedProject });
                if (builder != null)
                {
                    yield return builder;
                }
            }
        }

        public override void AddPrerequisite(IBuilder target)
        {
            if (subtasks != null)
            {
                subtasks.Add(target);
            }

            base.AddPrerequisite(target);
        }

        public override void RemovePrerequisite(IBuilder target)
        {
            if (subtasks != null)
            {
                subtasks.Remove(target);
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
            foreach (var subtask in subtasks)
            {
                var subResults = context.GetResults(subtask);
                result.UnionWith(subResults);
            }

            // result contains the output of the full module - selecting only the referenced project's expected build output
            string expectedFileName = referencedProject.Name.ToLower();
            return new HashSet<TargetRelativePath>(result.Where(
                path => Path.GetFileNameWithoutExtension(path).ToLowerInvariant() == expectedFileName));
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
                    referencedProject = null;
                }
            }
        }

        public override BuilderName Name
        {
            get
            {
                return new BuilderName(project, "ref:" + reference.Uri); 
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
            return string.Format("[{0}]", reference.Uri);
        }

        public bool Equals(SuiteReferenceBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ReferencedProject, other.ReferencedProject);
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
            unchecked
            {
                if (reference != null)
                {
                    return ReferencedProject.GetHashCode() ^ 19983;
                }
                else
                {
                    return 0;
                }
            }
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