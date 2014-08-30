using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Base class for YAML based loaders 
    /// </summary>
    public abstract class YamlModelLoaderBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(YamlModelLoaderBase));

        private readonly ISuiteFactory suiteFactory;
        private readonly IEnumerable<IYamlProjectParametersLoader> parametersLoaders;
        private readonly IUserOutput output;
        private readonly YamlParser parser;
        private readonly ReferenceLoader referenceLoader = new ReferenceLoader();

        /// <summary>
        /// Initializes the yaml loader
        /// </summary>
        /// <param name="suiteFactory">Factory interface to create new suite instances</param>
        /// <param name="parametersLoaders">Parameter loader implementations</param>
        /// <param name="output">Output interface to issue warnings</param>
        protected YamlModelLoaderBase(ISuiteFactory suiteFactory, IEnumerable<IYamlProjectParametersLoader> parametersLoaders, IUserOutput output)
        {
            Contract.Requires(suiteFactory != null);
            Contract.Requires(output != null);
            Contract.Ensures(this.suiteFactory == suiteFactory);
            Contract.Ensures(this.parametersLoaders == parametersLoaders);

            this.suiteFactory = suiteFactory;
            this.parametersLoaders = parametersLoaders;
            this.output = output;

            parser = new YamlParser();
        }

        /// <summary>
        /// Process an already loaded <c>YamlDocument</c> and returns the loaded suite model.
        /// </summary>
        /// <param name="yaml">The yaml document to process.</param>
        /// <returns>Returns a loaded model if succeeds. On error it throws an exception, never
        /// returns <c>null</c>.</returns>
        protected Suite LoadYaml(YamlDocument yaml)
        {
            Contract.Requires(yaml != null);
            Contract.Requires(yaml.RootNode != null);
            Contract.Ensures(Contract.Result<Suite>() != null);

            log.Debug("Processing YAML document...");

            var goals = new HashSet<Goal>(LoadGoals(yaml.RootNode));
            var suite = suiteFactory.CreateSuite(goals);

            parser.SetActiveGoal(suite.ActiveGoal);

            suite.Name = parser.GetScalarValue(yaml.RootNode, "suite", "Error reading the name of the suite");
            suite.Version = parser.GetOptionalScalarValue(yaml.RootNode, "version", null);
            suite.Copyright = parser.GetOptionalScalarValue(yaml.RootNode, "copyright", null);

            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(yaml.RootNode, "modules"))
            {
                var module = suite.GetModule(item.Key);

                if (item.Value != null)
                    LoadModule(module, item.Value);
            }

            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(yaml.RootNode, "products"))
            {
                var product = suite.GetProduct(item.Key);

                if (item.Value != null)
                    LoadProduct(suite, product, item.Value);
            }

            LoadParameters(suite, yaml.RootNode);

            log.Debug("Finished processing YAML document.");
            return suite;
        }

        private IEnumerable<Goal> LoadGoals(YamlNode rootNode)
        {
            var result = new Dictionary<string, Goal>();
            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(rootNode, "goals"))
            {
                var goal = LoadGoal(item.Key, item.Value, result);
                result.Add(goal.Name, goal);
            }

            return result.Values;
        }

        private Goal LoadGoal(string name, YamlNode value, IDictionary<string, Goal> loadedGoals)
        {
            var mappingNode = value as YamlMappingNode;
            if (mappingNode != null)
            {
                var incorporates = new List<Goal>();
                foreach (var pair in parser.EnumerateNamedNodesOf(value, "incorporates"))
                {
                    string childName = pair.Key;

                    Goal g;
                    if (!loadedGoals.TryGetValue(childName, out g))
                    {
                        g = new Goal(childName);
                    }

                    incorporates.Add(g);
                }

                return new Goal(name, incorporates);
            }
            else
            {
                return new Goal(name);
            }
        }

        private void LoadParameters(IProjectParametersHolder target, YamlNode node)
        {
            var mapping = node as YamlMappingNode;
            if (mapping != null)
            {
                foreach (var pair in parser.EnumerateNodesOf(mapping))
                {
                    if (new YamlScalarNode("postprocessors").Equals(pair.Key) &&
                        pair.Value is YamlMappingNode)
                    {
                        // skipping
                    }
                    else
                    {
                        TryAddParameters(target, pair.Key, pair.Value);
                    }
                }
            }
        }

        private void LoadProduct(Suite suite, Product product, YamlNode productNode)
        {
            Contract.Requires(product != null);
            Contract.Requires(productNode != null);

            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(productNode, "modules"))
            {
                if (suite.HasModule(item.Key))
                {
                    var module = suite.GetModule(item.Key);
                    product.AddModule(module);
                }
                else
                {
                    output.Warning(String.Format("The product {0} refers to a non-existing module {1}", product.Name, item.Key));
                }
            }

            SetProjectPostProcessors(product, productNode);
        }

        private void LoadModule(Module module, YamlNode moduleNode)
        {
            Contract.Requires(module != null);
            Contract.Requires(moduleNode != null);

            LoadModuleVersion(module, moduleNode);
            LoadModuleCopyright(module, moduleNode);
            LoadProjects(module, moduleNode);
            LoadTestProjects(module, moduleNode);
            LoadParameters(module, moduleNode);
            SetProjectPostProcessors(module, moduleNode);
        }

        private void LoadModuleVersion(Module module, YamlNode moduleNode)
        {
            module.Version = parser.GetOptionalScalarValue(moduleNode, "version", null);
        }

        private void LoadModuleCopyright(Module module, YamlNode moduleNode)
        {
            module.Copyright = parser.GetOptionalScalarValue(moduleNode, "copyright", null);
        }

        private void LoadTestProjects(Module module, YamlNode moduleNode)
        {
            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(moduleNode, "tests"))
            {
                var testProject = module.GetTestProject(item.Key);

                if (item.Value != null)
                    LoadProject(testProject, item.Value);
            }
        }

        private void LoadProjects(Module module, YamlNode moduleNode)
        {
            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(moduleNode, "projects"))
            {
                var project = module.GetProject(item.Key);

                if (item.Value != null)
                    LoadProject(project, item.Value);
            }
        }

        private void LoadProject(Project project, YamlNode projectNode)
        {
            Contract.Requires(project != null);
            Contract.Requires(projectNode != null);

            var mapping = projectNode as YamlMappingNode;
            if (mapping != null)
            {
                foreach (var pair in parser.EnumerateNodesOf(mapping))
                {
                    if (new YamlScalarNode("type").Equals(pair.Key) &&
                        pair.Value is YamlScalarNode)
                    {
                        SetProjectType(project, ((YamlScalarNode)pair.Value).Value);
                    }
                    else if (new YamlScalarNode("version").Equals(pair.Key) &&
                    pair.Value is YamlScalarNode)
                    {
                        project.Version = ((YamlScalarNode)pair.Value).Value;
                    }
                    else if (new YamlScalarNode("copyright").Equals(pair.Key) && pair.Value is YamlScalarNode)
                    {
                        project.Copyright = ((YamlScalarNode)pair.Value).Value;
                    }
                    else if (new YamlScalarNode("references").Equals(pair.Key) &&
                        pair.Value is YamlSequenceNode)
                    {
                        SetProjectReferences(project, ((YamlSequenceNode)pair.Value).Children);
                    }
                    else if (new YamlScalarNode("postprocessors").Equals(pair.Key) &&
                             pair.Value is YamlMappingNode)
                    {
                        // skipping
                    }
                    else
                    {
                        TryAddParameters(project, pair.Key, pair.Value);
                    }
                }
            }

            // Adding post processors
            SetProjectPostProcessors(project, projectNode);
        }

        private void SetProjectPostProcessors(IPostProcessorsHolder project, YamlNode postProcessorDefinitions)
        {
            Contract.Requires(project != null);
            Contract.Requires(postProcessorDefinitions != null);

            foreach (var pair in parser.EnumerateNamedNodesOf(postProcessorDefinitions, "postprocessors"))
            {
                var name = pair.Key;
                var value = pair.Value;

                PostProcessorId ppid;
                if (value == null)
                {
                    ppid = name;
                }
                else if (value is YamlMappingNode)
                {
                    ppid = parser.GetScalarValue(value, "type", "Post processor type is not specified");
                }
                else
                {
                    ppid = null;
                }

                if (ppid != null)
                {
                    var loader = parametersLoaders.FirstOrDefault(l => l.Supports(ppid.AsString));
                    
                    IProjectParameters param = null;
                    if (loader != null)
                    {
                        param = loader.Load(ppid.AsString, value, parser);                        
                    }

                    project.AddPostProcessor(new PostProcessDefinition(name, ppid)
                    {
                        Parameters = param
                    });
                }
            }
        }

        private void SetProjectReferences(Project project, IEnumerable<YamlNode> referenceNodes)
        {
            Contract.Requires(project != null);
            Contract.Requires(referenceNodes != null);

            foreach (var referenceNode in referenceNodes)
            {
                foreach (var reference in referenceLoader.LoadReference(referenceNode))
                    project.AddReference(reference);
            }
        }

        private void SetProjectType(Project project, string typeString)
        {
            Contract.Requires(project != null);
            Contract.Requires(typeString != null);

            switch (typeString.ToLowerInvariant())
            {
                case "executable":
                    project.Type = ProjectType.Executable;
                    break;
                case "windows-executable":
                    project.Type = ProjectType.WindowsExecutable;
                    break;
                case "static-library":
                case "staticlibrary":
                case "static":
                    project.Type = ProjectType.StaticLibrary;
                    break;
                default:
                    project.Type = ProjectType.Library;
                    break;
            }
        }

        private void TryAddParameters(IProjectParametersHolder target, YamlNode key, YamlNode value)
        {
            if (parametersLoaders != null)
            {
                var scalarKey = key as YamlScalarNode;

                if (scalarKey != null)
                {
                    string name = scalarKey.Value;
                    var loader = parametersLoaders.FirstOrDefault(l => l.Supports(name));
                    if (loader != null)
                    {
                        var param = loader.Load(name, value, parser);
                        target.AddParameters(name, param);
                    }
                }
            }
        }
    }
}