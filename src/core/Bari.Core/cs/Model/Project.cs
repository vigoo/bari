using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Bari.Core.Generic;
using Bari.Core.UI;

namespace Bari.Core.Model
{
    /// <summary>
    /// Represents a project of a module, which is a separate processable set of inputs creating one or more targets
    /// 
    /// <example>An example is a set of C# sources compiled into one assembly.</example>
    /// </summary>
    public class Project: IProjectParametersHolder, IPostProcessorsHolder
    {
        private readonly Module module;
        private readonly string name;
        private readonly IDictionary<string, SourceSet> sourceSets = new Dictionary<string, SourceSet>();
        private readonly ISet<Reference> references = new HashSet<Reference>();
        private readonly IDictionary<string, IProjectParameters> parameters = new Dictionary<string, IProjectParameters>();
        private readonly IDictionary<string, PostProcessDefinition> postProcessDefinitions = new Dictionary<string, PostProcessDefinition>();
        private ProjectType type = ProjectType.Library;
        private string version;

        /// <summary>
        /// Gets the module this project belongs to
        /// </summary>
        public Module Module
        {
            get { return module; }
        }

        /// <summary>
        /// Gets the project's name
        /// </summary>
        public string Name
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                
                return name;
            }
        }

        /// <summary>
        /// Gets or sets the project type
        /// </summary>
        public ProjectType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the project's version.
        /// 
        /// <para>Set this property to <c>null</c> to use the module's version in the
        /// <see cref="EffectiveVersion"/> property.</para>
        /// </summary>
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Gets the version of this project, which may come from the module's or suite's version if not explicitly
        /// specified in the <see cref="Version"/> property.
        /// </summary>
        public string EffectiveVersion
        {
            get { return version ?? module.EffectiveVersion; }
        }

        /// <summary>
        /// Gets the source sets belonging to this project
        /// </summary>
        public IEnumerable<SourceSet> SourceSets
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<SourceSet>>() != null);

                return sourceSets.Values;
            }
        }

        /// <summary>
        /// Gets the project references
        /// </summary>
        public IEnumerable<Reference> References
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Reference>>() != null);

                return references;
            }
        }

        /// <summary>
        /// Gets the postprocessors associated with this project
        /// </summary>
        public IEnumerable<PostProcessDefinition> PostProcessors
        {
            get { return postProcessDefinitions.Values; }
        }

        /// <summary>
        /// Gets the relative path to the target directory where this post processable item is compiled to
        /// </summary>
        public string RelativeTargetPath
        {
            get { return module.Name; }
        }

        /// <summary>
        /// Gets or sets the root directory of the project's sources
        /// </summary>
        public virtual IFileSystemDirectory RootDirectory
        {
            get { return module.RootDirectory.GetChildDirectory(name); }
        }

        /// <summary>
        /// Gets or sets the root directory of the project's sources
        /// </summary>
        public virtual string RelativeRootDirectory
        {
            get { return Path.Combine("src", Module.Name, name); }
        }

        /// <summary>
        /// Constructs the project model instance
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="module">The module the project belongs to</param>
        public Project(string name, Module module)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(module != null);

            this.name = name;
            this.module = module;
        }

        /// <summary>
        /// Gets a source set with the given type.
        /// 
        /// <para>If the requested source set does not exist yet, it is created as an empty set
        /// and added to the project.</para>
        /// </summary>
        /// <param name="setType">Source set type name</param>
        /// <returns>Returns the requested source set. Never returns <c>null</c>.</returns>
        public SourceSet GetSourceSet(string setType)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(setType));
            Contract.Ensures(Contract.Result<SourceSet>() != null);
            Contract.Ensures(Contract.Result<SourceSet>().Type == setType);
            Contract.Ensures(sourceSets.ContainsKey(setType));

            SourceSet result;
            if (!sourceSets.TryGetValue(setType, out result))
            {
                result = new SourceSet(setType);
                sourceSets.Add(setType, result);
            }

            Contract.Assume(result != null);
            return result;
        }

        /// <summary>
        /// Checks if the project has a source set with the given type and at least one files
        /// </summary>
        /// <param name="setType">The source set type name</param>
        /// <returns>Returns <c>true</c> if there is a source set with the given type name and at least one files</returns>
        public bool HasNonEmptySourceSet(string setType)
        {
            return sourceSets.ContainsKey(setType) &&
                   sourceSets[setType].Files.Any();
        }

        /// <summary>
        /// Adds a new reference to the set of project references
        /// </summary>
        /// <param name="reference">The new reference to be added</param>
        public void AddReference(Reference reference)
        {
            Contract.Requires(reference != null);
            Contract.Ensures(References.Contains(reference));

            references.Add(reference);
        }

        /// <summary>
        /// Checks whether a parameter block exist with the given name
        /// </summary>
        /// <param name="paramsName">Name of the parameter block</param>
        /// <returns>Returns <c>true</c> if a parameter block with the given name is applied to this model item</returns>
        public bool HasParameters(string paramsName)
        {
            return parameters.ContainsKey(paramsName) || module.HasParameters(paramsName);
        }

        /// <summary>
        /// Gets a parameter block by its name
        /// </summary>
        /// <typeparam name="TParams">The expected type of the parameter block</typeparam>
        /// <param name="paramsName">Name of the parameter block</param>
        /// <returns>Returns the parameter block</returns>
        public TParams GetParameters<TParams>(string paramsName)
            where TParams: IProjectParameters
        {
            IProjectParameters result;
            if (!parameters.TryGetValue(paramsName, out result))
                return module.GetParameters<TParams>(paramsName);
            else
                return (TParams)result;
        }

        /// <summary>
        /// Adds a new parameter block to this model item
        /// </summary>
        /// <param name="paramsName">Name of the parameter block</param>
        /// <param name="projectParameters">The parameter block to be added</param>
        public void AddParameters(string paramsName, IProjectParameters projectParameters)
        {
            parameters.Add(paramsName, projectParameters);
        }

        /// <summary>
        /// Adds a new postprocessor to this project
        /// </summary>
        /// <param name="postProcessDefinition">Post processor type and parameters</param>
        public void AddPostProcessor(PostProcessDefinition postProcessDefinition)
        {
            postProcessDefinitions.Add(postProcessDefinition.Name, postProcessDefinition);
        }

        /// <summary>
        /// Gets a registered post processor by its name
        /// </summary>
        /// <param name="key">Name of the post processor</param>
        /// <returns>Returns the post processor definition</returns>
        public PostProcessDefinition GetPostProcessor(string key)
        {
            return postProcessDefinitions[key];
        }

        public override string ToString()
        {
            return String.Format("Project {0}.{1}", Module.Name, name);
        }

        /// <summary>
        /// Checks for warnings in the project, and displays them through the given output interface
        /// </summary>
        /// <param name="output">Output interface</param>
        public void CheckForWarnings(IUserOutput output)
        {
            bool hasFiles = sourceSets.Values.Any(sourceSet => sourceSet.Files.Any());

            if (!hasFiles)
                output.Warning(String.Format("{0} has no source files", ToString()),
                    new[]
                    {
                        String.Format("The source files must be organized into source sets"),
                        String.Format("The source sets must be placed in {0}", RelativeRootDirectory)
                    });
        }
    }
}