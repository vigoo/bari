using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build.Dependencies;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// A <see cref="IReferenceBuilder"/> implementation for depending on another project within the same module
    /// 
    /// <para>
    /// The reference URIs are interpreted in the following way:
    /// 
    /// <example>module://ProjectName</example>
    /// means the project called <c>ProjectName</c> in the same module where the reference has been used.
    /// </para>
    /// </summary>
    public class ModuleReferenceBuilder: IReferenceBuilder
    {
        private readonly Module module;
        private readonly IEnumerable<IProjectBuilderFactory> projectBuilders;
        private Reference reference;
        private ISet<IBuilder> subtasks;

        /// <summary>
        /// Constructs the reference builder
        /// </summary>
        /// <param name="project">Current project to be used as context</param>
        /// <param name="projectBuilders">Project builders available</param>
        public ModuleReferenceBuilder(Project project, IEnumerable<IProjectBuilderFactory> projectBuilders)
        {
            module = project.Module;
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
            get { return module.Name + "." + Reference.Uri.Host; }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            if (module.HasProject(reference.Uri.Host))
            {
                var project = module.GetProject(reference.Uri.Host);

                subtasks = new HashSet<IBuilder>();
                foreach (var projectBuilder in projectBuilders)
                {
                    subtasks.Add(projectBuilder.AddToContext(context, new[] {project}));
                }

                context.AddBuilder(this, subtasks);
            }
            else
            {
                throw new InvalidReferenceException(string.Format("Module {0} has no project called {1}", module.Name,
                                                                  reference.Uri.Host));
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