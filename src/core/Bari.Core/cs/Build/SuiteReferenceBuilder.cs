using System.Collections.Generic;
using System.Linq;
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
    public class SuiteReferenceBuilder : IReferenceBuilder
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
            get
            {
                if (subtasks.Count == 0)
                    return new NoDependencies();
                else if (subtasks.Count == 1)
                    return new SubtaskDependency(subtasks.First());
                else
                    return new MultipleDependencies(
                        subtasks.Select(subtask => new SubtaskDependency(subtask)));
            }
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

                if (referencedProject != null)
                {
                    subtasks = new HashSet<IBuilder>();
                    foreach (var projectBuilder in projectBuilders)
                    {
                        var builder = projectBuilder.AddToContext(context, new[] { referencedProject });
                        if (builder != null)
                            subtasks.Add(builder);
                    }

                    context.AddBuilder(this, subtasks);
                }
                else
                {
                    throw new InvalidReferenceException(string.Format("Module {0} has no project called {1}",
                                                                      moduleName,
                                                                      projectName));
                }
            }
            else
            {
                throw new InvalidReferenceException(string.Format("Suite has no module called {0}",
                                                                      moduleName));
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